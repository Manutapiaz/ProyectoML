using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoML.Application.Categorias;
using ProyectoML.Application.MercadoLibre.PublicacionML;
using System.Security.Claims;

namespace ProyectoML.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{

    private readonly CategoriaService _service;

    public CategoriasController(CategoriaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoriaResponse>>> GetAll()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var categorias = await _service.GetAllAsync(usuarioId);
        return Ok(categorias);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaResponse>> Create(CrearCategoriaRequest request)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var categoria = await _service.CreateAsync(request, usuarioId);
        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);

    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoriaResponse>> GetById(int id)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var categoria = await _service.GetByIdAsync(id, usuarioId);
        return Ok(categoria);

    }


    [HttpPut("{id:int}/categoria-ml")]
    public async Task<ActionResult<CategoriaResponse>> Update(int id, string idCategoriaMLC)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var categoria = await _service.UpdateAsync(id, idCategoriaMLC, usuarioId);
        return Ok(categoria);
    }


}
