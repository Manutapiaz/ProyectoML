using Microsoft.AspNetCore.Mvc;
using ProyectoML.Application.MercadoLibre;

namespace ProyectoML.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class MercadoLibreController : ControllerBase
{

    private readonly MercadoLibreAuthService _mercadoLibreAuthClient;

    private readonly MercadoLibreApiClientService _mercadoLibreApiClienteService;

    public MercadoLibreController(MercadoLibreAuthService mercadoLibreAuthClient, MercadoLibreApiClientService mercadoLibreApiClienteService)
    {
        _mercadoLibreAuthClient = mercadoLibreAuthClient;
        _mercadoLibreApiClienteService = mercadoLibreApiClienteService;
    }

    // en el controller
    [HttpGet("login")]
    public IActionResult Login() => Redirect(_mercadoLibreAuthClient.ObtenerUrlLogin());

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest("El código de autorización es requerido.");
        }
        return Ok(await _mercadoLibreAuthClient.ProcesarCallbackAsync(code));
    }

    [HttpGet("cuenta/{userId:long}")]
    public async Task<ActionResult<DataUserDto>> Cuenta(long userId)
    {
        var cuenta = await _mercadoLibreApiClienteService.ObtenerCuentaAsync(userId);
        return Ok(cuenta);
    }

    [HttpGet("sugerencias")]
    public async Task<ActionResult<List<SugerenciaCategoria>>> SugerenciasCategoria(string nombreProducto)
    {
        var sugerencias = await _mercadoLibreApiClienteService.SugerirCategoriaAsync(nombreProducto);
        return Ok(sugerencias);
    }

    [HttpGet("categorias/{categoryId}/atributos")]
    public async Task<ActionResult<List<AtributoCategoria>>> AtributosCategoria(string categoryId, long userId)
    {
        var atributos = await _mercadoLibreApiClienteService.ObtenerAtributosCategoriaAsync(categoryId, userId);
        return Ok(atributos);
    }
}
