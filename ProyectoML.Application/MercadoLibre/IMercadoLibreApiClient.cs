using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Application.MercadoLibre;

public  interface IMercadoLibreApiClient
{
    Task<DataUserDto> ObtenerCuentaAsync(string token);
}
