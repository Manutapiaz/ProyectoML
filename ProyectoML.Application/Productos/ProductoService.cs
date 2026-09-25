

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
            request.Descripcion??string.Empty,
            request.Precio,
            categoria
        );


        if (request.StockInicial != 0)
            producto.RegistrarMovimiento(request.StockInicial, MotivoMovimiento.Ingreso, "Stock inicial");

        await _productoRepository.AddAsync(producto);
     
        return ToResponse(producto);
        
    }

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
            p.FechaActualizacion
        );
}
