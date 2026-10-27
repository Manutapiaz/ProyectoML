using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoML.Application.MercadoLibre;
using System.Security.Claims;

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

    [HttpGet("login")]
    public IActionResult Login()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        string url = _mercadoLibreAuthClient.ObtenerUrlLogin(usuarioId);
        return Ok(new { url });
    }

    [AllowAnonymous]
    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code, string state)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest("El código de autorización es requerido.");
        }
        return Ok(await _mercadoLibreAuthClient.ProcesarCallbackAsync(code, state));
    }

    [HttpGet("cuenta/{userId:long}")]
    public async Task<ActionResult<DataUserDto>> Cuenta(long userId)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var cuenta = await _mercadoLibreApiClienteService.ObtenerCuentaAsync(userId, usuarioId);
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
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var atributos = await _mercadoLibreApiClienteService.ObtenerAtributosCategoriaAsync(categoryId, userId, usuarioId);
        return Ok(atributos);
    }
}
