using ProyectoML.Domain.Entities;

namespace ProyectoML.Application.Productos;

public record CrearProductoRequest(string SKU, string Nombre, string? Descripcion, decimal Precio, int CategoriaId, int StockInicial = 0);

public record ProductoResponse(int Id, string SKU, string Nombre, string? Descripcion, int Stock, decimal Precio, int CategoriaId, string CategoriaNombre, bool Activo, DateTime FechaCreacion, DateTime FechaActualizacion);