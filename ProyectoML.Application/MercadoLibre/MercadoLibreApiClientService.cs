namespace ProyectoML.Application.MercadoLibre;

public class MercadoLibreApiClientService
{
    private readonly IMercadoLibreApiClient _mercadoLibreApiClient;
    private readonly MercadoLibreAuthService _mercadoLibreAuthService;
    public MercadoLibreApiClientService(IMercadoLibreApiClient mercadoLibreApiClient, MercadoLibreAuthService mercadoLibreAuthService)
    {
        _mercadoLibreApiClient = mercadoLibreApiClient;
        _mercadoLibreAuthService = mercadoLibreAuthService;
    }

    public async Task<DataUserDto> ObtenerCuentaAsync(long userId)
    {
        var token = await _mercadoLibreAuthService.ObtenerAccessTokenAsync(userId);
        return await _mercadoLibreApiClient.ObtenerCuentaAsync(token);
    }

}
