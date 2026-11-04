

using ProyectoML.Application.Auth;
using ProyectoML.Application.Categorias;
using ProyectoML.Application.Exceptions;
using ProyectoML.Application.MercadoLibre;
using ProyectoML.Application.MercadoLibre.PublicacionML;
using ProyectoML.Domain.Entities;
using ProyectoML.Domain.Enums;
using ProyectoML.Domain.Exceptions;
using System.Diagnostics;

namespace ProyectoML.Application.Productos;

public class ProductoService
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    private readonly MercadoLibreAuthService _authService;
    private readonly IPublicacionMercadoLibreRepository _publicacionRepository;
    private readonly IMercadoLibreApiClient _mercadoLibreApiClient;

    public ProductoService(
        IProductoRepository productoRepository,
        ICategoriaRepository categoriaRepository,
        IPublicacionMercadoLibreRepository publicacionRepository,
        IMercadoLibreApiClient mercadoLibreApiClient,
        MercadoLibreAuthService mercadoLibreAuthService)
    {
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
        _publicacionRepository = publicacionRepository;
        _mercadoLibreApiClient = mercadoLibreApiClient;
        _authService = mercadoLibreAuthService;
    }

    public async Task<List<ProductoResponse>> GetAllAsync(int usuarioId)
    {
        var productos = await _productoRepository.GetAllAsync(usuarioId);
        return productos.Select(p => ToResponse(p)).ToList();
    }

    public async Task<ProductoResponse> GetByIdAsync(int id, int usuarioId)
    {
        var producto = await _productoRepository.GetByIdAsync(id, usuarioId);
        if (producto == null) throw new NotFoundException($"Producto con id {id} no encontrado");
        return ToResponse(producto);
    }

    public async Task<ProductoResponse> CreateAsync(CrearProductoRequest request, int usuarioId)
    {
        if (await _productoRepository.ExistsBySKUAsync(request.SKU, usuarioId))
        {
            throw new ConflictException($"Ya existe un producto con el SKU {request.SKU}");
        }

        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId, usuarioId) ?? throw new NotFoundException($"No existe una categoría con id {request.CategoriaId}");

        var producto = new Producto(
            request.SKU,
            request.Nombre,
            request.Descripcion ?? string.Empty,
            request.Precio,
            categoria,
            usuarioId
        );

        if (request.StockInicial != 0)
            producto.RegistrarMovimiento(request.StockInicial, MotivoMovimiento.Ingreso, "Stock inicial");

        request.Imagenes?.ForEach(url => producto.AgregarImagen(url));

        await _productoRepository.AddAsync(producto);

        return ToResponse(producto);

    }
  
    public async Task<ProductoResponse> CambiarPrecioAsync(int id, int usuarioId, int precio)
    {
        var producto = await _productoRepository.GetByIdAsync(id, usuarioId) ?? throw new NotFoundException($"No existe un producto con id {id}");
        producto.CambiarPrecio(precio);
        await _productoRepository.UpdateAsync(producto);

        var publicacion = await _publicacionRepository.GetByIdProductoAsync(id);
        if (publicacion == null || publicacion.Estado == EstadoPublicacion.Cerrada)
            return ToResponse(producto);

        var token = await _authService.ObtenerAccessTokenAsync(publicacion.UserId, usuarioId);
        await _mercadoLibreApiClient.ActualizarItemAsync(publicacion.ExternalId, (int)producto.Precio, null, token);

        return ToResponse(producto);
    }

    public async Task<ProductoResponse> AjustarStockAsync(int id, int usuarioId, int stock)
    {
        if (stock == 0) throw new DomainException("La cantidad no puede ser 0");

        var producto = await _productoRepository.GetByIdAsync(id, usuarioId) ?? throw new NotFoundException($"No existe un producto con id {id}");
        producto.RegistrarMovimiento(stock, MotivoMovimiento.Ajuste, "Ajuste Stock");
        await _productoRepository.UpdateAsync(producto);

        var publicacion = await _publicacionRepository.GetByIdProductoAsync(id);
        if (publicacion == null || publicacion.Estado == EstadoPublicacion.Cerrada)
            return ToResponse(producto);

        var token = await _authService.ObtenerAccessTokenAsync(publicacion.UserId, usuarioId);
        await _mercadoLibreApiClient.ActualizarItemAsync(publicacion.ExternalId, null, producto.Stock, token);

        return ToResponse(producto);
    }




    public async Task<List<ProductoImagenResponse>> GetImagenesAsync(int productoId, int usuarioId)
    {
        var producto = await ObtenerProductoAsync(productoId, usuarioId);
        return producto.Imagenes.Select(ToImagenResponse).ToList();
    }

    public async Task<ProductoImagenResponse> AgregarImagenAsync(int productoId, AgregarImagenRequest request, int usuarioId)
    {
        var producto = await ObtenerProductoAsync(productoId, usuarioId);

        producto.AgregarImagen(request.Url);
        await _productoRepository.UpdateAsync(producto);
        return ToImagenResponse(producto.Imagenes.Last());
    }

    public async Task QuitarImagenAsync(int productoId, int imagenId, int usuarioId)
    {
        var producto = await ObtenerProductoAsync(productoId, usuarioId);

        producto.QuitarImagen(imagenId);
        await _productoRepository.UpdateAsync(producto);
    }

    private async Task<Producto> ObtenerProductoAsync(int id, int usuarioId)
        => await _productoRepository.GetByIdAsync(id, usuarioId)
            ?? throw new NotFoundException($"Producto con id {id} no encontrado");

    private static ProductoImagenResponse ToImagenResponse(ProductoImagen i) => new(i.Id, i.Url, i.FechaCreacion);

    private static ProductoResponse ToResponse(Producto p) => new(
            p.Id,
            p.SKU,
            p.Nombre,
            p.Descripcion,
            p.Stock,
            p.Precio,
            p.CategoriaId,
            p.Categoria.Nombre,
            p.Activo,
            p.FechaCreacion,
            p.FechaActualizacion,
            p.Imagenes.Select(ToImagenResponse).ToList()
        );
}
