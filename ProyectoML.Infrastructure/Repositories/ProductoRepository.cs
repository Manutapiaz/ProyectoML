using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProyectoML.Application.Exceptions;
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
    public async Task UpdateAsync(Producto producto)
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        //busco si es indice uno o constrint unico y envio msj que ya existe
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            _context.ChangeTracker.Clear();
            throw new ConflictException("El registro ya existe.");
        }
    }

    public Task<Producto?> GetByIdAsync(int id, int usuarioId)
        => _context.Productos.Include(p => p.Usuario).Include(p => p.Categoria).Include(p => p.Imagenes).FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);

    public Task<bool> ExistsBySKUAsync(string sku, int usuarioId)
        => _context.Productos.AnyAsync(p => p.SKU == sku && p.UsuarioId == usuarioId);
    public Task<List<Producto>> GetAllAsync(int usuarioId)
     => _context.Productos.Include(p => p.Usuario).Include(p => p.Categoria).Include(p => p.Imagenes).
        Where(p => p.UsuarioId == usuarioId).OrderBy(p => p.Nombre).ToListAsync();

    public Task<bool> ExisteMovimientoAsync(int productoId, string referencia)
        => _context.MovimientoStock.AnyAsync(m => m.ProductoId == productoId && m.Referencia == referencia);

}
