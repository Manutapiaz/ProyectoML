
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

    public Task AddAsync(CredencialMercadoLibre credencialMercadoLibre)
    {        
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<CredencialMercadoLibre> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}
