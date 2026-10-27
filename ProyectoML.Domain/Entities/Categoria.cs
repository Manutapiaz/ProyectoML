using ProyectoML.Domain.Exceptions;

namespace ProyectoML.Domain.Entities;

public class Categoria
{
    public int Id { get; private set; }

    public string? IdCategoriaMLC { get; private set; }

    public string Nombre { get; private set; }
    public int UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; }
    private Categoria()
    {

    }
    public Categoria(string nombreCategoria, string? idCategoriaMLC, int usuarioId)
    {
        if (string.IsNullOrWhiteSpace(nombreCategoria)) throw new DomainException("Categoria no puede venir vacia");
        this.Nombre = nombreCategoria;
        this.IdCategoriaMLC = idCategoriaMLC;
        this.UsuarioId = usuarioId;

    }

    public void AsignarCategoriaMLC(string idCategoriaMLC)
    {
        if (string.IsNullOrWhiteSpace(idCategoriaMLC)) throw new DomainException("IdCategoriaMLC no puede venir vacio");
        this.IdCategoriaMLC = idCategoriaMLC;
    }

}
