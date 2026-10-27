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
        var productos = await _service.GetAllAsync();
        return Ok(productos);
    }

    [HttpPost]
    public async Task<ActionResult<ProductoResponse>> Create(CrearProductoRequest request)
    {
        var producto = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductoResponse>> GetById(int id)
    {
        var producto = await _service.GetByIdAsync(id);
        return Ok(producto);
    }


    [HttpGet("{id:int}/imagenes")]
    public async Task<ActionResult<List<ProductoImagenResponse>>> GetImagenes(int id)
    {
        var imagenes = await _service.GetImagenesAsync(id);
        return Ok(imagenes);
    }

    [HttpPost("{id:int}/imagenes")]
    public async Task<ActionResult<ProductoImagenResponse>> AddImagen(int id, AgregarImagenRequest request)
    {
        var imagen = await _service.AgregarImagenAsync(id, request);
        return StatusCode(StatusCodes.Status201Created, imagen);
    }

    [HttpDelete("{id:int}/imagenes/{imagenId:int}")]
    public async Task<IActionResult> DeleteImagen(int id, int imagenId)
    {
        await _service.QuitarImagenAsync(id, imagenId);
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
