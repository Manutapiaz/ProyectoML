using ProyectoML.Domain.Enums;
namespace ProyectoML.Domain.Entities;

public class MovimientoStock
{
    public int Id { get; private set; }

    public int ProductoId { get; private set; }
    public Producto Producto { get; private set; }

    public int Cantidad { get; private set; }

    public MotivoMovimiento Motivo { get; private set; }

    public string? Referencia { get; private set; }

    public DateTime Fecha { get; private set; }

    private MovimientoStock()
    {
        
    }
    internal MovimientoStock(Producto producto, int cantida, MotivoMovimiento motivo, string referencia)
    {
        this.Producto = producto ;
        this.Cantidad = cantida;
        this.Motivo = motivo;
        this.Referencia = referencia;
        this.Fecha = DateTime.UtcNow;

    }
}

