namespace ProyectoML.Application.MercadoLibre.PublicacionML;

public record PublicarProductoRequest(
    long UserId,
    string CategoryId,
    List<string> Pictures,
    List<AtributoPublicacionRequest> Atributos,
    string ListingTypeId = "gold_special");

public record AtributoPublicacionRequest(string Id, string? ValueId, string? ValueName);
