using ProyectoML.Application.MercadoLibre.NotificacionML;
using ProyectoML.Application.MercadoLibre.PublicacionML;
using ProyectoML.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Application.MercadoLibre;

public interface IMercadoLibreApiClient
{
    Task<DataUserDto> ObtenerCuentaAsync(string token);
    Task<List<SugerenciaCategoria>> SugerirCategoriaAsync(string nombreProducto);
    Task<List<AtributoCategoria>> ObtenerAtributosCategoriaAsync(string categoryId, string token);
    Task<ItemPublicado> PublicarItemAsync(ItemMercadoLibre item, string token);
    Task<ItemPublicado> BuscarPublicacionAsync(string idProductoMLC, string token);
    Task<Orden> ObtenerOrdenAsync(long orderId, string token);
}
