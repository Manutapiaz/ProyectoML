using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoML.Application.MercadoLibre.PublicacionML;

namespace ProyectoML.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PublicacionesController : ControllerBase
{
    private readonly PublicacionMercadoLibreService _publicacionService;

    public PublicacionesController(PublicacionMercadoLibreService publicacionService)
    {
        _publicacionService = publicacionService;
    }

    [HttpPost("{externalId}/sincronizar")]
    public async Task<ActionResult<PublicacionMercadoLibreDto>> SincronizarAsync(string externalId)
    {
        var publicacion = await _publicacionService.SincronizarAsync(externalId);
        return Ok(publicacion);
    }

    [HttpGet]
    public async Task<ActionResult<List<PublicacionMercadoLibreDto>>> GetAllPublicaciones([FromQuery] long userId)
    {
        var publicaciones = await _publicacionService.GetAllAsync(userId);
        return Ok(publicaciones);
    }

    [HttpGet("{externalId}")]
    public async Task<ActionResult<PublicacionMercadoLibreDto>> GetPublicacionByIdAsync(string externalId)
    {
        var publicacion = await _publicacionService.GetPublicacionByIdAsync(externalId);
        return Ok(publicacion);
    }
}
