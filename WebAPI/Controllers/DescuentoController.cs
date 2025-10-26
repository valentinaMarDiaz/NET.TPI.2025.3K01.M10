using Application.Services;
using DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("descuentos")]
public class DescuentosController : ControllerBase
{
    private readonly DescuentoService _svc = new();

    // GET /descuentos?producto=...
    [HttpGet]
    public ActionResult<IEnumerable<DescuentoDTO>> GetAll([FromQuery] string? producto)
        => Ok(_svc.GetAll(producto));

    // GET /descuentos/{id}
    [HttpGet("{id:int}")]
    public ActionResult<DescuentoDTO> GetById(int id)
    {
        var dto = _svc.Get(id);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    // POST /descuentos
    [HttpPost]
    public ActionResult<DescuentoDTO> Create([FromBody] DescuentoCUDTO cu)
    {
        var dto = new DescuentoDTO
        {
            IdProducto = cu.IdProducto,
            FechaInicioUtc = cu.FechaInicioUtc,
            FechaCaducidadUtc = cu.FechaCaducidadUtc,
            Descripcion = cu.Descripcion,
            Codigo = cu.Codigo,
            Porcentaje = cu.Porcentaje
        };

        var creado = _svc.Add(dto);
        return CreatedAtAction(nameof(GetById), new { id = creado.IdDescuento }, creado);
    }

    // PUT /descuentos
    [HttpPut]
    public IActionResult Update([FromBody] DescuentoCUDTO cu)
    {
        var dto = new DescuentoDTO
        {
            IdDescuento = cu.IdDescuento,
            IdProducto = cu.IdProducto,
            FechaInicioUtc = cu.FechaInicioUtc,
            FechaCaducidadUtc = cu.FechaCaducidadUtc,
            Descripcion = cu.Descripcion,
            Codigo = cu.Codigo,
            Porcentaje = cu.Porcentaje
        };

        var ok = _svc.Update(dto);
        if (!ok) return NotFound();
        return NoContent();
    }

    // DELETE /descuentos/{id}
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var ok = _svc.Delete(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    // GET /descuentos/vigentes?texto=...
    [HttpGet("vigentes")]
    public ActionResult<IEnumerable<DescuentoDTO>> GetVigentes([FromQuery] string? texto)
        => Ok(_svc.GetVigentes(texto, DateTime.UtcNow));

    // GET /descuentos/validar?codigo=XYZ
    [HttpGet("validar")]
    public ActionResult<DescuentoDTO> Validar([FromQuery] string codigo)
    {
        var dto = _svc.GetByCodigoVigente(codigo);
        if (dto is null) return NotFound();
        return Ok(dto);
    }
}
