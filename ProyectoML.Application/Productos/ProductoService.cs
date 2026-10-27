

using ProyectoML.Application.Categorias;
using ProyectoML.Application.Exceptions;
using ProyectoML.Domain.Entities;
using ProyectoML.Domain.Enums;

namespace ProyectoML.Application.Productos;

public class ProductoService
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public ProductoService(IProductoRepository productoRepository, ICategoriaRepository categoriaRepository)
    {
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
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

        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId ,usuarioId) ?? throw new NotFoundException($"No existe una categoría con id {request.CategoriaId}");

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
