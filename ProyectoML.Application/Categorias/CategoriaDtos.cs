namespace ProyectoML.Application.Categorias;

public record CrearCategoriaRequest(string Nombre);
public record CategoriaResponse(int Id, string Nombre);