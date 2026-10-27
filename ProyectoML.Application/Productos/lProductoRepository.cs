using ProyectoML.Domain.Entities;

namespace ProyectoML.Application.Productos;

public interface IProductoRepository
{
    Task<List<Producto>> GetAllAsync(int usuarioId);
    Task<Producto?> GetByIdAsync(int id, int usuarioId);
    Task<bool> ExistsBySKUAsync(string sku, int usuarioId);
    Task AddAsync(Producto producto);
    Task UpdateAsync(Producto producto);
}