using ProyectoML.Domain.Entities;

namespace ProyectoML.Application.Productos;

public interface IProductoRepository
{
    Task<List<Producto>> GetAllAsync();
    Task<Producto?> GetByIdAsync(int id);
    Task<bool> ExistsBySKUAsync(string sku);
    Task AddAsync(Producto producto);
}