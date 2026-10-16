using ProyectoML.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Application.MercadoLibre.PublicacionML;

public record PublicarProductoRequest(long UserId, List<AtributoPublicacionRequest> Atributos, string ListingTypeId = "gold_special");

public record AtributoPublicacionRequest(string Id, string? ValueId, string? ValueName);

public record PublicacionMercadoLibreDto(int Id, int ProductoId, string ExternalId, long UserId, string Estado, DateTime FechaPublicacion, string? Permalink);

public record ItemMercadoLibre(string FamilyName, string CategoryId, int Price, int AvailableQuantity, string ListingTypeId, List<string> Pictures,
    List<AtributoPublicacionRequest> Atributos);

public record ItemPublicado(string Id, EstadoPublicacion Estado, string? Permalink);


