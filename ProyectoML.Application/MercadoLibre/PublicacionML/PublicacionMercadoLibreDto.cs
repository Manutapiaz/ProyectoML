using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Application.MercadoLibre.PublicacionML;


public record PublicacionMercadoLibreDto(
    int Id,
    int ProductoId,
    string ExternalId,
    long UserId,
    string Estado,
    DateTime FechaPublicacion,
    string? Permalink
);
