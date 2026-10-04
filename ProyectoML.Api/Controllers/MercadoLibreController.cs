using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProyectoML.Application.MercadoLibre;
using ProyectoML.Infrastructure.MercadoLibre;

namespace ProyectoML.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class MercadoLibreController : ControllerBase
{

    private readonly MercadoLibreAuthService _mercadoLibreAuthClient;

    public MercadoLibreController( MercadoLibreAuthService mercadoLibreAuthClient)
    {    
        _mercadoLibreAuthClient = mercadoLibreAuthClient;
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
}
