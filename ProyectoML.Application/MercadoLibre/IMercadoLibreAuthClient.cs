using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Application.MercadoLibre;

public interface IMercadoLibreAuthClient
{
    string GetAuthorizationUrl();
    Task<TokenResult> GetAccessTokenAsync(string code);
    Task<TokenResult> RefreshAccessTokenAsync(string refreshToken);
}
