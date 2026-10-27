using ProyectoML.Domain.Entities;

namespace ProyectoML.Application.Categorias;

public interface ICategoriaRepository
{
    Task<List<Categoria>> GetAllAsync(int usuarioId);
    Task<bool> ExistsByNombreAsync(string nombre, int usuarioId);

    Task<Categoria?> GetByIdAsync(int id, int usuarioId);
    Task AddAsync(Categoria categoria);
    Task UpdateAsync(Categoria categoria);
}
