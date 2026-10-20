using ProyectoML.Domain.Exceptions;

namespace ProyectoML.Domain.Entities;

public class ProductoImagen
{
    public int Id { get; private set; }

    public int ProductoId { get; private set; }

    public Producto Producto { get; private set; }

    public string Url { get; private set; }

    public DateTime FechaCreacion { get; private set; }

    private ProductoImagen()
    {

    }

    public ProductoImagen(Producto producto, string url)
    {
        if (string.IsNullOrWhiteSpace(producto?.Nombre)) throw new DomainException("Producto no puede venir vacio");
        if (string.IsNullOrWhiteSpace(url)) throw new DomainException("URL no puede venir vacia");
        this.Producto = producto;
        this.Url = url;
        this.FechaCreacion = DateTime.UtcNow;
    }


}
