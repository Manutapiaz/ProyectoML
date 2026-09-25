using Microsoft.EntityFrameworkCore;
using ProyectoML.Application.Productos;
using ProyectoML.Domain.Entities;
using ProyectoML.Infrastructure.Persistence;

namespace ProyectoML.Infrastructure.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly AppDbContext _context;
    public ProductoRepository(AppDbContext context)
    {
        _context = context;
    }
    public Task<List<Producto>> GetAllAsync()
        => _context.Productos.Include(p => p.Categoria).OrderBy(p => p.Nombre).ToListAsync();

    public Task<Producto?> GetByIdAsync(int id)
        => _context.Productos.FirstOrDefaultAsync(p => p.Id == id);

    public Task AddAsync(Producto producto)
    {
        _context.Productos.Add(producto);
        return _context.SaveChangesAsync();
    }

    public Task<bool> ExistsBySKUAsync(string sku)    
        => _context.Productos.AnyAsync(p => p.SKU == sku);
    
}
