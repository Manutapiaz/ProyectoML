using Microsoft.AspNetCore.DataProtection;
using ProyectoML.Application.Auth;
using System.Security.Cryptography;
namespace ProyectoML.Infrastructure.Auth;

public class AuthStateProtector : IAuthStateProtector
{

    private readonly ITimeLimitedDataProtector _protector;

    public AuthStateProtector(IDataProtectionProvider provider)
    {
        _protector = provider
            .CreateProtector("MercadoLibre.OAuthState")
            .ToTimeLimitedDataProtector();
    }

    public int? Desproteger(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return null;

        try
        {
            var valor = _protector.Unprotect(state);
            return int.TryParse(valor, out var usuarioId) ? usuarioId : null;
        }
        catch (CryptographicException)
        {
            return null;
        }
    }

    public string Proteger(int usuarioId)
    {
        return _protector.Protect(usuarioId.ToString(), TimeSpan.FromMinutes(10));
    }
}
