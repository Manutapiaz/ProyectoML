using ProyectoML.Domain.Enums;
using ProyectoML.Domain.Exceptions;

namespace ProyectoML.Domain.Entities;

public class PublicacionMercadoLibre
{
    public int Id { get; private set; }
    public int ProductoId { get; private set; }
    public Producto Producto { get; private set; } = null!;
    public string ExternalId { get; private set; } = null!;
    public long UserId { get; private set; }
    public EstadoPublicacion Estado { get; private set; }
    public DateTime FechaPublicacion { get; private set; }
    public string? Permalink { get; private set; }

    private PublicacionMercadoLibre() { }

    public PublicacionMercadoLibre(
        int productoId,
        string externalId,
        long userId,
        EstadoPublicacion estado,
        DateTime fechaPublicacion,
        string? permalink)
    {
        if (productoId <= 0) throw new DomainException("El id del producto debe ser mayor a cero.");
        if (string.IsNullOrWhiteSpace(externalId)) throw new DomainException("El id externo es obligatorio.");
        if (userId <= 0) throw new DomainException("El id de usuario debe ser mayor a cero.");
        if (!Enum.IsDefined(estado)) throw new DomainException("Estado de publicación inválido.");

        ProductoId = productoId;
        ExternalId = externalId;
        UserId = userId;
        Estado = estado;
        FechaPublicacion = fechaPublicacion;
        Permalink = permalink;
    }

    public void CambiarEstado(EstadoPublicacion nuevoEstado)
    {
        if (!Enum.IsDefined(nuevoEstado)) throw new DomainException("Estado de publicación inválido.");
        Estado = nuevoEstado;
    }
}