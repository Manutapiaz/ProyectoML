using ProyectoML.Application.Exceptions;
using ProyectoML.Domain.Entities;

namespace ProyectoML.Application.Categorias;

public class CategoriaService
{
    private readonly ICategoriaRepository _repository;

    public CategoriaService(ICategoriaRepository repository) => _repository = repository;

    public async Task<List<CategoriaResponse>> GetAllAsync()
    {
        var categorias = await _repository.GetAllAsync();

        return categorias
            .Select(c => new CategoriaResponse(c.Id, c.Nombre))
            .ToList();
    }

    public async Task<CategoriaResponse> CreateAsync(CrearCategoriaRequest request)
    {
        if (await _repository.ExistsByNombreAsync(request.Nombre))
            throw new ConflictException($"Ya existe una categoría con el nombre '{request.Nombre}'");

        var categoria = new Categoria(request.Nombre);

        await _repository.AddAsync(categoria);

        return new CategoriaResponse(categoria.Id, categoria.Nombre);
    }

    public async Task<CategoriaResponse> GetByIdAsync(int id)
    {
        var categoria = await _repository.GetByIdAsync(id);
        if (categoria == null)
            throw new NotFoundException("Categoría no encontrada");

        return new CategoriaResponse(categoria.Id, categoria.Nombre);
    }
}
