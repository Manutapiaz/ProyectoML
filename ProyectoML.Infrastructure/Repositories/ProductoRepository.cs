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

    public Task AddAsync(Producto producto)
    {
        _context.Productos.Add(producto);
        return _context.SaveChangesAsync();
    }
    public Task UpdateAsync(Producto producto)
        => _context.SaveChangesAsync();
 
    public Task<Producto?> GetByIdAsync(int id, int usuarioId)
        => _context.Productos.Include(p => p.Categoria).Include(p => p.Imagenes).FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);     

    public Task<bool> ExistsBySKUAsync(string sku, int usuarioId)    
        => _context.Productos.AnyAsync(p => p.SKU == sku && p.UsuarioId == usuarioId);
    public Task<List<Producto>> GetAllAsync(int usuarioId)
     => _context.Productos.Include(p => p.Categoria).Include(p => p.Imagenes).
        Where(p => p.UsuarioId == usuarioId).OrderBy(p => p.Nombre).ToListAsync();

}
