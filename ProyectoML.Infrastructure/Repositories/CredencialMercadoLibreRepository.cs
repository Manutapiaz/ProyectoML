
using Microsoft.EntityFrameworkCore;
using ProyectoML.Application.MercadoLibre;
using ProyectoML.Domain.Entities;
using ProyectoML.Infrastructure.Persistence;


namespace ProyectoML.Infrastructure.Repositories;

public class CredencialMercadoLibreRepository : ICredencialMercadoLibreRepository
{
    private readonly AppDbContext _context;

    public CredencialMercadoLibreRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CredencialMercadoLibre credencialMercadoLibre)
    {
        _context.CredencialesMercadoLibre.Add(credencialMercadoLibre);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public Task<bool> ExistsByIdAsync(long id)
          => _context.CredencialesMercadoLibre.AnyAsync(c => c.UserId == id);

    public Task<CredencialMercadoLibre?> GetByUserIdAsync(long userId)
        => _context.CredencialesMercadoLibre.FirstOrDefaultAsync(c => c.UserId == userId);


}
