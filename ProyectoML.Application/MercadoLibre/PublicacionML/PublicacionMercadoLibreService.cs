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
        // 1. Validaciones contra BD
        var producto = await _productoRepository.GetByIdAsync(productoId)
            ?? throw new NotFoundException($"El producto con Id {productoId} no fue encontrado.");

        if (await _publicacionRepository.ExistsAsync(productoId, request.UserId))
        {
            throw new ConflictException($"El producto con Id {productoId} ya está publicado en la cuenta {request.UserId}.");
        }

        // 2. Reglas de ML que Producto no garantiza
        if (string.IsNullOrWhiteSpace(request.CategoryId)) throw new DomainException("La categoría de Mercado Libre es obligatoria.");
        if (producto.Stock <= 0) throw new DomainException("No se puede publicar un producto sin stock.");
        if (producto.Precio != decimal.Truncate(producto.Precio)) throw new DomainException("El precio en CLP no puede tener decimales.");

        // 3. Token (lanza NotFoundException si la cuenta no está vinculada)
        var token = await _authService.ObtenerAccessTokenAsync(request.UserId);

        // 4. Atributos: se validan contra la categoría y el SKU lo ponemos nosotros
        var atributos = (request.Atributos ?? [])
            .Where(a => !string.Equals(a.Id, AtributoSku, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var atributosCategoria = await _mercadoLibreApiClient.ObtenerAtributosCategoriaAsync(request.CategoryId, token);
        ValidarAtributos(atributos, atributosCategoria);

        atributos.Add(new AtributoPublicacionRequest(AtributoSku, null, producto.SKU));

        // 5. Publicar en ML
        var item = new ItemMercadoLibre(
            producto.Nombre,
            request.CategoryId,
            (int)producto.Precio,
            producto.Stock,
            request.ListingTypeId,
            request.Pictures ?? [],
            atributos);

        var publicado = await _mercadoLibreApiClient.PublicarItemAsync(item, token);

        // 6. Guardar lo que respondió ML
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


    public async Task<PublicacionMercadoLibreDto> GetPublicacionByIdAsync(string externalId)
    {
        var publicacion = await _publicacionRepository.GetByExternalIdAsync(externalId);
        if (publicacion == null)
        {
            throw new NotFoundException($"La publicación con Id {externalId} no fue encontrada.");
        }
        return ToResponse(publicacion);
    }

    public async Task<List<PublicacionMercadoLibreDto>> GetAllAsync()
    {
        var publicaciones = await _publicacionRepository.GetAllAsync();
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

            // Si viene valueId, tiene que ser uno de los valores que ML ofrece para ese atributo
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
