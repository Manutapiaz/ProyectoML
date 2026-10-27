using Microsoft.EntityFrameworkCore;
using ProyectoML.Application.Categorias;
using ProyectoML.Domain.Entities;
using ProyectoML.Infrastructure.Persistence;


namespace ProyectoML.Infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;
    public CategoriaRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
    }

    public Task UpdateAsync(Categoria categoria)
        => _context.SaveChangesAsync();

    public Task<Categoria?> GetByIdAsync(int id, int usuarioId)
        => _context.Categorias.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);


    public Task<bool> ExistsByNombreAsync(string nombre, int usuarioId)
        => _context.Categorias.AnyAsync(c => c.Nombre == nombre && c.UsuarioId == usuarioId);


    public Task<List<Categoria>> GetAllAsync(int usuarioId)
        => _context.Categorias.Where(c => c.UsuarioId == usuarioId).OrderBy(c => c.Nombre).ToListAsync();
}
