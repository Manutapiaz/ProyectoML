using Microsoft.EntityFrameworkCore;
using ProyectoML.Application.MercadoLibre.PublicacionML;
using ProyectoML.Domain.Entities;
using ProyectoML.Infrastructure.Persistence;


namespace ProyectoML.Infrastructure.Repositories;

public class PublicacionMercadoLibreRepository : IPublicacionMercadoLibreRepository
{

    private readonly AppDbContext _context;

    public PublicacionMercadoLibreRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PublicacionMercadoLibre publicacion)
    {
        _context.PublicacionesMercadoLibre.Add(publicacion);
        await _context.SaveChangesAsync();

    }
    public Task UpdateAsync(PublicacionMercadoLibre publicacion)
       => _context.SaveChangesAsync();


    public Task<bool> ExistsByExternalId(string externalId)
    {
        return _context.PublicacionesMercadoLibre.AnyAsync(p => p.ExternalId == externalId);
    }

    public Task<List<PublicacionMercadoLibre>> GetAllAsync()
    {
        return _context.PublicacionesMercadoLibre.ToListAsync();
    }

    public Task<PublicacionMercadoLibre?> GetByExternalIdAsync(string externalId)
    {
        return _context.PublicacionesMercadoLibre.FirstOrDefaultAsync(p => p.ExternalId == externalId);
    }
    public Task<PublicacionMercadoLibre?> GetByIdProductoAsync(int idProducto)
    {
        return _context.PublicacionesMercadoLibre.FirstOrDefaultAsync(p => p.ProductoId == idProducto);
    }


    public Task<bool> ExistsAsync(int productoId, long userId)
    {
        return _context.PublicacionesMercadoLibre.AnyAsync(p => p.ProductoId == productoId && p.UserId == userId);
    }

}
