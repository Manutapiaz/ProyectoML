using ProyectoML.Domain.Exceptions;

namespace ProyectoML.Domain.Entities;

public class Categoria
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }

    private Categoria()
    {
        
    }
    public Categoria(string nombreCategoria)
    {
        if (string.IsNullOrEmpty(nombreCategoria)) throw new DomainException("Categoria no puede venir vacia");
        this.Nombre = nombreCategoria;
    }


}
