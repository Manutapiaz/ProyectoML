using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProyectoML.Application.Productos;
using ProyectoML.Infrastructure.MercadoLibre;

namespace ProyectoML.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly ProductoService _service;
 

    public ProductosController(ProductoService service)
    {
        _service = service;
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
}
