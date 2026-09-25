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

    public Task<Categoria?> GetByIdAsync(int id)
        => _context.Categorias.FirstOrDefaultAsync(c => c.Id == id);


    public Task<bool> ExistsByNombreAsync(string nombre)
        => _context.Categorias.AnyAsync(c => c.Nombre == nombre);


    public Task<List<Categoria>> GetAllAsync()
        => _context.Categorias.OrderBy(c => c.Nombre).ToListAsync();
}
