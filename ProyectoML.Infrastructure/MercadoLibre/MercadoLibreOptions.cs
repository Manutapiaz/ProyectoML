using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Infrastructure.MercadoLibre;

public class MercadoLibreOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string AuthUrl { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;

}
