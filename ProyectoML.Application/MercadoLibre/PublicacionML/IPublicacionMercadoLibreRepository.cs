
namespace ProyectoML.Application.MercadoLibre.PublicacionML;

using ProyectoML.Domain.Entities;
public interface IPublicacionMercadoLibreRepository
{
    Task AddAsync(PublicacionMercadoLibre publicacion);

    Task UpdateAsync(PublicacionMercadoLibre publicacion);
    Task<List<PublicacionMercadoLibre>> GetAllAsync();
    Task<bool> ExistsByExternalId(string externalId);
    Task<PublicacionMercadoLibre?> GetByExternalIdAsync(string externalId);
    Task<PublicacionMercadoLibre?> GetByIdProductoAsync(int idProducto);
    Task<bool> ExistsAsync(int productoId, long userId);
   
}
