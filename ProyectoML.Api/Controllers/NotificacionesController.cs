using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoML.Application.MercadoLibre.NotificacionML;
namespace ProyectoML.Api.Controllers;


[ApiController]
[Route("api/mercadolibre/notificaciones")]
[AllowAnonymous]
public class NotificacionesController : ControllerBase
{

    private readonly NotificacionMercadoLibreService _notificacionService;

    public NotificacionesController(NotificacionMercadoLibreService notificacionService)
    {
        _notificacionService = notificacionService;
    }


    [HttpPost]
    public async Task<IActionResult> HookOrdenAsync([FromBody] NotificationRequest request)
    {

        await _notificacionService.ProcesarNotificacionAsync(request);
        return Ok();
    }




}
