

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



    public async Task<List<ProductoResponse>> GetAllAsync()
    {
        var productos = await _productoRepository.GetAllAsync();
        return productos.Select(p => ToResponse(p)).ToList();
    }

    public async Task<ProductoResponse> GetByIdAsync(int id)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto == null) throw new NotFoundException($"Producto con id {id} no encontrado");
        return ToResponse(producto);
    }

    public async Task<ProductoResponse> CreateAsync(CrearProductoRequest request)
    {
        if (await _productoRepository.ExistsBySKUAsync(request.SKU))
        {
            throw new ConflictException($"Ya existe un producto con el SKU {request.SKU}");
        }

        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId) ?? throw new NotFoundException($"No existe una categoría con id {request.CategoriaId}");

        var producto = new Producto(
            request.SKU,
            request.Nombre,
            request.Descripcion ?? string.Empty,
            request.Precio,
            categoria
        );



        if (request.StockInicial != 0)
            producto.RegistrarMovimiento(request.StockInicial, MotivoMovimiento.Ingreso, "Stock inicial");


        request.Imagenes?.ForEach(url => producto.AgregarImagen(url));


        await _productoRepository.AddAsync(producto);

        return ToResponse(producto);

    }

    public async Task<List<ProductoImagenResponse>> GetImagenesAsync(int productoId)
    {
        var producto = await ObtenerProductoAsync(productoId);
        return producto.Imagenes.Select(ToImagenResponse).ToList();
    }

    public async Task<ProductoImagenResponse> AgregarImagenAsync(int productoId, AgregarImagenRequest request)
    {
        var producto = await ObtenerProductoAsync(productoId);

        producto.AgregarImagen(request.Url);
        await _productoRepository.UpdateAsync(producto);
        return ToImagenResponse(producto.Imagenes.Last());
    }

    public async Task QuitarImagenAsync(int productoId, int imagenId)
    {
        var producto = await ObtenerProductoAsync(productoId);

        producto.QuitarImagen(imagenId);
        await _productoRepository.UpdateAsync(producto);
    }

    private async Task<Producto> ObtenerProductoAsync(int id)
        => await _productoRepository.GetByIdAsync(id)
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
