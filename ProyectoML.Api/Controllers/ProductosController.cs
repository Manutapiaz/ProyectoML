using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProyectoML.Application.MercadoLibre.PublicacionML;
using ProyectoML.Application.Productos;
using ProyectoML.Infrastructure.MercadoLibre;

namespace ProyectoML.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly ProductoService _service;
    private readonly PublicacionMercadoLibreService _publicacionService;

    public ProductosController(ProductoService service, PublicacionMercadoLibreService publicacionService)
    {
        _service = service;
        _publicacionService = publicacionService;
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

    [HttpPost("{id:int}/publicaciones")]
    public async Task<ActionResult<PublicacionMercadoLibreDto>> Publicar(int id, PublicarProductoRequest request)
    {
        var publicacion = await _publicacionService.PublicarAsync(id, request);
        return StatusCode(StatusCodes.Status201Created, publicacion);
    }
}
