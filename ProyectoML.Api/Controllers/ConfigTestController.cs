using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProyectoML.Infrastructure.MercadoLibre;

namespace ProyectoML.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigTestController : ControllerBase
{

    private readonly IOptions<MercadoLibreOptions>  _configuration;


    public ConfigTestController(IOptions<MercadoLibreOptions> configuration)
    {
        _configuration = configuration;
    }


    [HttpGet]
    public IActionResult GetConfig()
    {
      
        return Ok(_configuration.Value);
    }
}
