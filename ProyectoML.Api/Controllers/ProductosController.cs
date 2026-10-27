using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoML.Application.MercadoLibre.PublicacionML;
using ProyectoML.Application.Productos;
using System.Security.Claims;


namespace ProyectoML.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly ProductoService _service;

    private readonly PublicacionMercadoLibreService _publicacionService;
    public ProductosController(ProductoService service, PublicacionMercadoLibreService publicacionMercadoLibreService)
    {
        _service = service;
        _publicacionService = publicacionMercadoLibreService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductoResponse>>> GetAll()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productos = await _service.GetAllAsync(usuarioId);
        return Ok(productos);
    }

    [HttpPost]
    public async Task<ActionResult<ProductoResponse>> Create(CrearProductoRequest request)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var producto = await _service.CreateAsync(request, usuarioId);
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductoResponse>> GetById(int id)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var producto = await _service.GetByIdAsync(id, usuarioId);
        return Ok(producto);
    }


    [HttpGet("{id:int}/imagenes")]
    public async Task<ActionResult<List<ProductoImagenResponse>>> GetImagenes(int id)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var imagenes = await _service.GetImagenesAsync(id, usuarioId);
        return Ok(imagenes);
    }

    [HttpPost("{id:int}/imagenes")]
    public async Task<ActionResult<ProductoImagenResponse>> AddImagen(int id, AgregarImagenRequest request)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var imagen = await _service.AgregarImagenAsync(id, request, usuarioId);
        return StatusCode(StatusCodes.Status201Created, imagen);
    }

    [HttpDelete("{id:int}/imagenes/{imagenId:int}")]
    public async Task<IActionResult> DeleteImagen(int id, int imagenId)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.QuitarImagenAsync(id, imagenId, usuarioId);
        return NoContent();
    }

    [HttpPost("{id:int}/publicaciones")]
    public async Task<ActionResult<PublicacionMercadoLibreDto>> Publicar(int id, PublicarProductoRequest request)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var publicacion = await _publicacionService.PublicarAsync(id, request,usuarioId);
        return StatusCode(StatusCodes.Status201Created, publicacion);
    }

}
