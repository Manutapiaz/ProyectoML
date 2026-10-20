using ProyectoML.Application.Exceptions;
using ProyectoML.Application.Productos;
using ProyectoML.Domain.Entities;
using ProyectoML.Domain.Exceptions;


namespace ProyectoML.Application.MercadoLibre.PublicacionML;

public class PublicacionMercadoLibreService
{
    private const string AtributoSku = "SELLER_SKU";

    private readonly IPublicacionMercadoLibreRepository _publicacionRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly MercadoLibreAuthService _authService;
    private readonly IMercadoLibreApiClient _mercadoLibreApiClient;

    public PublicacionMercadoLibreService(
        IPublicacionMercadoLibreRepository publicacionRepository,
        IProductoRepository productoRepository,
        MercadoLibreAuthService authService,
        IMercadoLibreApiClient mercadoLibreApiClient)
    {
        _publicacionRepository = publicacionRepository;
        _productoRepository = productoRepository;
        _authService = authService;
        _mercadoLibreApiClient = mercadoLibreApiClient;
    }

    public async Task<PublicacionMercadoLibreDto> PublicarAsync(int productoId, PublicarProductoRequest request)
    {

        var producto = await _productoRepository.GetByIdAsync(productoId)
            ?? throw new NotFoundException($"El producto con Id {productoId} no fue encontrado.");

        if (await _publicacionRepository.ExistsAsync(productoId, request.UserId))
        {
            throw new ConflictException($"El producto con Id {productoId} ya está publicado en la cuenta {request.UserId}.");
        }
            
        if (producto.Stock <= 0) throw new DomainException("No se puede publicar un producto sin stock.");
        if (producto.Precio != decimal.Truncate(producto.Precio)) throw new DomainException("El precio en CLP no puede tener decimales.");       
        if(string.IsNullOrEmpty(producto.Categoria.IdCategoriaMLC)) throw new DomainException($"La categoría {producto.Categoria.Nombre} no tiene un ID de categoría en Mercado Libre asignado.");
        if (producto.Imagenes.Count < 1) throw new DomainException($"El producto debe tener al menos 1 Imagen registrada");

    
        var token = await _authService.ObtenerAccessTokenAsync(request.UserId);
      
        var atributos = (request.Atributos ?? [])
            .Where(a => !string.Equals(a.Id, AtributoSku, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var atributosCategoria = await _mercadoLibreApiClient.ObtenerAtributosCategoriaAsync(producto.Categoria.IdCategoriaMLC, token);
        ValidarAtributos(atributos, atributosCategoria);

        atributos.Add(new AtributoPublicacionRequest(AtributoSku, null, producto.SKU));

        var item = new ItemMercadoLibre(
            producto.Nombre,
            producto.Categoria.IdCategoriaMLC,
            (int)producto.Precio,
            producto.Stock,
            request.ListingTypeId,
            producto.Imagenes.Select(n => n.Url).ToList() ?? [],
            atributos);

        var publicado = await _mercadoLibreApiClient.PublicarItemAsync(item, token);

        var publicacion = new PublicacionMercadoLibre(
            productoId,
            publicado.Id,
            request.UserId,
            publicado.Estado,
            DateTime.UtcNow,
            publicado.Permalink);

        await _publicacionRepository.AddAsync(publicacion);

        return ToResponse(publicacion);
    }

    public async Task<PublicacionMercadoLibreDto> SincronizarAsync(string externalId)
    {
        var publicacion = await _publicacionRepository.GetByExternalIdAsync(externalId) ?? throw new NotFoundException("No existe la publicacion ingresada");
        var token = await _authService.ObtenerAccessTokenAsync(publicacion.UserId);
        var publicacionUpdate = await _mercadoLibreApiClient.BuscarPublicacionAsync(publicacion.ExternalId, token);
        publicacion.CambiarEstado(publicacionUpdate.Estado);
        await _publicacionRepository.UpdateAsync(publicacion);
        return ToResponse(publicacion);
    }

    public async Task<PublicacionMercadoLibreDto> GetPublicacionByIdAsync(string externalId)
    {
        var publicacion = await _publicacionRepository.GetByExternalIdAsync(externalId);
        if (publicacion == null)
        {
            throw new NotFoundException($"La publicación con Id {externalId} no fue encontrada.");
        }
        return ToResponse(publicacion);
    }

    public async Task<List<PublicacionMercadoLibreDto>> GetAllAsync(long userId)
    {
        var publicaciones = await _publicacionRepository.GetAllAsync(userId);
        return publicaciones.Select(p => ToResponse(p)).ToList();
    }

    private static void ValidarAtributos(List<AtributoPublicacionRequest> enviados, List<AtributoCategoria> atributosCategoria)
    {
        foreach (var atributo in enviados)
        {
            if (string.IsNullOrWhiteSpace(atributo.Id))
                throw new DomainException("Todos los atributos deben tener id.");

            if (string.IsNullOrWhiteSpace(atributo.ValueId) && string.IsNullOrWhiteSpace(atributo.ValueName))
                throw new DomainException($"El atributo {atributo.Id} debe tener valueId o valueName.");

            var definicion = atributosCategoria.FirstOrDefault(a => a.Id.Equals(atributo.Id, StringComparison.OrdinalIgnoreCase));
            if (definicion is not null
                && !string.IsNullOrWhiteSpace(atributo.ValueId)
                && definicion.Values.Count > 0
                && !definicion.Values.Any(v => v.Id == atributo.ValueId))
            {
                throw new DomainException($"El valor {atributo.ValueId} no es válido para el atributo {atributo.Id}.");
            }
        }

        var faltantes = atributosCategoria
            .Where(a => a.Required)
            .Where(a => !enviados.Any(e => e.Id.Equals(a.Id, StringComparison.OrdinalIgnoreCase)))
            .Select(a => a.Id)
            .ToList();

        if (faltantes.Count > 0)
            throw new DomainException($"Faltan atributos obligatorios de la categoría: {string.Join(", ", faltantes)}.");
    }

    private static PublicacionMercadoLibreDto ToResponse(PublicacionMercadoLibre p) => new(
              p.Id,
              p.ProductoId,
              p.ExternalId,
              p.UserId,
              p.Estado.ToString(),
              p.FechaPublicacion,
              p.Permalink);
}
