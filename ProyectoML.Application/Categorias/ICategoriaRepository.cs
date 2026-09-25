using ProyectoML.Domain.Entities;

namespace ProyectoML.Application.Categorias;

public interface ICategoriaRepository
{
    Task<List<Categoria>> GetAllAsync();
    Task<bool> ExistsByNombreAsync(string nombre);

    Task<Categoria?> GetByIdAsync(int id);
    Task AddAsync(Categoria categoria);
}
