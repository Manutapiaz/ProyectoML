using ProyectoML.Application.Exceptions;
using ProyectoML.Application.MercadoLibre;
using ProyectoML.Domain.Entities;

namespace ProyectoML.Application.Categorias;

public class CategoriaService
{
    private readonly ICategoriaRepository _repository;


    public CategoriaService(ICategoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CategoriaResponse>> GetAllAsync()
    {
        var categorias = await _repository.GetAllAsync();

        return categorias
            .Select(ToResponse)
            .ToList();
    }

    public async Task<CategoriaResponse> CreateAsync(CrearCategoriaRequest request)
    {
        if (await _repository.ExistsByNombreAsync(request.Nombre))
            throw new ConflictException($"Ya existe una categoría con el nombre '{request.Nombre}'");

        var categoria = new Categoria(request.Nombre, request.IdCategoriaMLC);
        await _repository.AddAsync(categoria);
        return ToResponse(categoria);
    }
    public async Task<CategoriaResponse> UpdateAsync(int id, string idCategoriaMLC)
    {
        var categoria = await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"Categoría con ID {id} no encontrada");

        categoria.AsignarCategoriaMLC(idCategoriaMLC);
        await _repository.UpdateAsync(categoria);
        return ToResponse(categoria);
    }


    public async Task<CategoriaResponse> GetByIdAsync(int id)
    {
        var categoria = await _repository.GetByIdAsync(id);
        if (categoria == null)
            throw new NotFoundException("Categoría no encontrada");

        return ToResponse(categoria);
    }

    private static CategoriaResponse ToResponse(Categoria c) =>
        new(c.Id, c.Nombre, c.IdCategoriaMLC);


}
