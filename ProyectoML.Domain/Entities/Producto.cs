using ProyectoML.Domain.Enums;
using ProyectoML.Domain.Exceptions;
namespace ProyectoML.Domain.Entities;

public class Producto
{
    public int Id { get; private set; }
    public string SKU { get; private set; } = null!;
    public string Nombre { get; private set; } = null!;
    public string? Descripcion { get; private set; }
    public decimal Precio { get; private set; }
    public int Stock { get; private set; }
    public int CategoriaId { get; private set; }
    public Categoria Categoria { get; private set; } = null!;

    public bool Activo { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaActualizacion { get; private set; }
    public int UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; } = null!;
    private Producto()
    {

    }
    public Producto(string sku, string nombre, string descripcion, decimal precio, Categoria categoria, int usuarioId)
    {
        if (string.IsNullOrWhiteSpace(sku)) throw new DomainException("SKU no puede ser vacio");

        this.SKU = sku;
        this.Nombre = nombre;
        this.Descripcion = descripcion;
        this.Stock = 0;
        this.Categoria = categoria;
        this.Activo = true;
        this.FechaCreacion = DateTime.UtcNow;
        this.FechaActualizacion = DateTime.UtcNow;
        this.UsuarioId = usuarioId;
        this.CambiarPrecio(precio);
    }

    private readonly List<MovimientoStock> _movimientos = new();
    public IReadOnlyCollection<MovimientoStock> Movimientos => _movimientos;

    public void RegistrarMovimiento(int cantidad, MotivoMovimiento motivo, string referencia)
    {
        if ((Stock + cantidad) < 0) throw new DomainException("Stock no puede ser menor a 0");

        if (motivo == MotivoMovimiento.Ingreso && cantidad <= 0) throw new DomainException("cantidad en motivo Ingreso debe ser mayor a 0");
        if (motivo == MotivoMovimiento.Venta && cantidad  >= 0) throw new DomainException("cantidad en motivo Ventas debe ser menor a 0");

        this.Stock += cantidad;
        this.FechaActualizacion = DateTime.UtcNow;
        _movimientos.Add(new MovimientoStock(
            this,
            cantidad,
            motivo,
            referencia));
    }
    public void CambiarPrecio(decimal precio)
    {
        if (precio <= 0) throw new DomainException("Precio debe ser mayor a 0");
        this.Precio = precio;
        this.FechaActualizacion = DateTime.UtcNow;
    }

    private readonly List<ProductoImagen> _imagenes = new();
    public IReadOnlyCollection<ProductoImagen> Imagenes => _imagenes;
    public const int MaxImagenes = 10;
    public void AgregarImagen(string url)
    {
        if (_imagenes.Count >= MaxImagenes)
        {
            throw new DomainException($"No se pueden agregar mas de {MaxImagenes} imagenes");
        }
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? result)
            || (result.Scheme != Uri.UriSchemeHttp && result.Scheme != Uri.UriSchemeHttps))
        {
            throw new DomainException("La url de la imagen no es valida");
        }

        if (_imagenes.Any(n => n.Url == url))
        {
            throw new DomainException("La imagen ya existe");
        }

       
        _imagenes.Add(new ProductoImagen(this, url));
        this.FechaActualizacion = DateTime.UtcNow;
    }

    public void QuitarImagen(int imagenId)
    {
        var imagen = _imagenes.FirstOrDefault(i => i.Id == imagenId)
            ?? throw new DomainException($"El producto no tiene una imagen con id {imagenId}");
        _imagenes.Remove(imagen);
        this.FechaActualizacion = DateTime.UtcNow;
    }
}

