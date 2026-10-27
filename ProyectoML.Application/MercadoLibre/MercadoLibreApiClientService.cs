
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

    public async Task<DataUserDto> ObtenerCuentaAsync(long userId, int usuarioId)
    {
       
        var token = await _mercadoLibreAuthService.ObtenerAccessTokenAsync(userId, usuarioId);
        return await _mercadoLibreApiClient.ObtenerCuentaAsync(token);
    }

    public async Task<List<SugerenciaCategoria>> SugerirCategoriaAsync(string nombreProducto)
    {
        return await _mercadoLibreApiClient.SugerirCategoriaAsync(nombreProducto);
    }

    public async Task<List<AtributoCategoria>> ObtenerAtributosCategoriaAsync(string categoryId, long userId, int usuarioId)
    {
        var token = await _mercadoLibreAuthService.ObtenerAccessTokenAsync(userId, usuarioId);
        return await _mercadoLibreApiClient.ObtenerAtributosCategoriaAsync(categoryId, token);
    }
}
