
using ProyectoML.Domain.Exceptions;

namespace ProyectoML.Domain.Entities;

public class CredencialMercadoLibre
{

    public string AccessToken { get; private set; }

    public DateTime ExpireIn { get; private set; }

    public long UserId { get; private set; }

    public string RefreshToken { get; private set; }


    private CredencialMercadoLibre()
    {

    }

    public CredencialMercadoLibre(string accessToken, long expireIn, long userId, string refreshToken)
    {
        if (string.IsNullOrEmpty(accessToken)) throw new DomainException("AccessToken no puede venir vacio");
        if (expireIn <= 0) throw new DomainException("ExpireIn debe ser mayor a 0");
        if (userId <= 0) throw new DomainException("UserId debe ser mayor a 0");
        if (string.IsNullOrEmpty(refreshToken)) throw new DomainException("RefreshToken no puede venir vacio");
        this.AccessToken = accessToken;
        this.ExpireIn = DateTime.UtcNow.AddSeconds(expireIn);
        this.UserId = userId;
        this.RefreshToken = refreshToken;
    }

    public void ActualizarCredenciales(string accessToken, int expireIn, string refreshToken)
    {
        if (string.IsNullOrEmpty(accessToken)) throw new DomainException("AccessToken no puede venir vacio");
        if (expireIn <= 0) throw new DomainException("ExpireIn debe ser mayor a 0");
        if (string.IsNullOrEmpty(refreshToken)) throw new DomainException("RefreshToken no puede venir vacio");
        this.AccessToken = accessToken;
        this.ExpireIn = DateTime.UtcNow.AddSeconds(expireIn);
        this.RefreshToken = refreshToken;
    }

    public bool EstaExpirada()
    {
        return DateTime.UtcNow >= this.ExpireIn;
    }  

}
