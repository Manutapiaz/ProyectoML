using ProyectoML.Domain.Enums;

namespace ProyectoML.Application.MercadoLibre.PublicacionML;

public record ItemMercadoLibre(
    string FamilyName,
    string CategoryId,
    int Price,
    int AvailableQuantity,
    string ListingTypeId,
    List<string> Pictures,
    List<AtributoPublicacionRequest> Atributos);

public record ItemPublicado(string Id, EstadoPublicacion Estado, string? Permalink);
