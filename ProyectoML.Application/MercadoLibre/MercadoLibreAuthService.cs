using ProyectoML.Application.Exceptions;
using ProyectoML.Domain.Entities;

namespace ProyectoML.Application.MercadoLibre;

public class MercadoLibreAuthService
{
    private readonly ICredencialMercadoLibreRepository _repository;
    private readonly IMercadoLibreAuthClient _authClient;

    public MercadoLibreAuthService(ICredencialMercadoLibreRepository repository, IMercadoLibreAuthClient authClient)
    {
        _repository = repository;
        _authClient = authClient;
    }

    public string ObtenerUrlLogin() => _authClient.GetAuthorizationUrl();

    public async Task<string> ProcesarCallbackAsync(string code)
    {
        var token = await _authClient.GetAccessTokenAsync(code);

        var credencial = await _repository.GetByUserIdAsync(token.UserId);

        if (credencial is null)
        {
            credencial = new CredencialMercadoLibre(token.AccessToken, token.ExpiresIn, token.UserId, token.RefreshToken);
            await _repository.AddAsync(credencial);
            return "Cuenta de Mercado Libre vinculada correctamente";
        }

        credencial.ActualizarCredenciales(token.AccessToken, token.ExpiresIn, token.RefreshToken);
        await _repository.SaveChangesAsync();
        return "Credenciales de Mercado Libre actualizadas";
    }

    public async Task<string> ObtenerAccessTokenAsync(long userId)
    {
        var credencial = await _repository.GetByUserIdAsync(userId)
            ?? throw new NotFoundException($"No hay credenciales para el usuario {userId}. Autoriza la app primero.");

        if (!credencial.EstaExpirada())
            return credencial.AccessToken;

        var token = await _authClient.RefreshAccessTokenAsync(credencial.RefreshToken);

        credencial.ActualizarCredenciales(token.AccessToken, token.ExpiresIn, token.RefreshToken);
        await _repository.SaveChangesAsync();

        return credencial.AccessToken;
    }
}