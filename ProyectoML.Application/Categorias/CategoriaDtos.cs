namespace ProyectoML.Application.Categorias;

public record CrearCategoriaRequest(string Nombre, string? IdCategoriaMLC);
public record CategoriaResponse(int Id, string Nombre, string? IdCategoriaMLC = null);

