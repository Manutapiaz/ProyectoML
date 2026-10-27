using ProyectoML.Application.Auth;
using ProyectoML.Application.Exceptions;
using ProyectoML.Domain.Entities;
using System.Security.Claims;

namespace ProyectoML.Application.MercadoLibre;

public class MercadoLibreAuthService
{
    private readonly ICredencialMercadoLibreRepository _repository;
    private readonly IMercadoLibreAuthClient _authClient;

    private readonly IAuthStateProtector _authStateProtector;

    public MercadoLibreAuthService(
        ICredencialMercadoLibreRepository repository,
        IMercadoLibreAuthClient authClient,
         IAuthStateProtector authStateProtector)
    {
        _repository = repository;
        _authClient = authClient;
        _authStateProtector = authStateProtector;
    }

    public string ObtenerUrlLogin(int usuarioId)
    {
        string state = _authStateProtector.Proteger(usuarioId);

        return _authClient.GetAuthorizationUrl(state);
    }

    public async Task<string> ProcesarCallbackAsync(string code, string state)
    {
        int IdUsuario = _authStateProtector.Desproteger(state) ?? throw new AuthException("Error state invalido");
        var token = await _authClient.GetAccessTokenAsync(code);
        var credencial = await _repository.GetByUserIdAsync(token.UserId);

        if (credencial is null)
        {
            credencial = new CredencialMercadoLibre(token.AccessToken, token.ExpiresIn, token.UserId, token.RefreshToken, IdUsuario);
            await _repository.AddAsync(credencial);
            return "Cuenta de Mercado Libre vinculada correctamente";
        }
        if (credencial.UsuarioId != IdUsuario)
            throw new ConflictException("Esta cuenta de Mercado Libre ya está vinculada a otro usuario");

        credencial.ActualizarCredenciales(token.AccessToken, token.ExpiresIn, token.RefreshToken);
        await _repository.SaveChangesAsync();
        return "Credenciales de Mercado Libre actualizadas";
    }

    public async Task<bool> PerteneceAUsuarioAsync(long userId, int usuarioId)
    {
        var credencial = await _repository.GetByUserIdAsync(userId);
        return credencial != null && credencial.UsuarioId == usuarioId;
    }

    public async Task<string> ObtenerAccessTokenAsync(long userId, int usuarioId)
    {
     

        var credencial = await _repository.GetByUserIdAsync(userId);

        if (credencial == null || credencial.UsuarioId != usuarioId)
        {
            throw new NotFoundException($"No hay credenciales para el usuario {userId}. Autoriza la app primero.");
        }

        if (!credencial.EstaExpirada())
            return credencial.AccessToken;

        var token = await _authClient.RefreshAccessTokenAsync(credencial.RefreshToken);

        credencial.ActualizarCredenciales(token.AccessToken, token.ExpiresIn, token.RefreshToken);
        await _repository.SaveChangesAsync();

        return credencial.AccessToken;
    }
}