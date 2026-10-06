using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Infrastructure.MercadoLibre;

public class MercadoLibreOptions
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string AuthUrl { get; set; }
    public string ApiUrl { get; set; }
    public string RedirectUri { get; set; }

}
