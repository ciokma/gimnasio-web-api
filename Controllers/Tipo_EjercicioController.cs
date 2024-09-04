using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Data;

[Route("api/[controller]")]
[ApiController]
public class Tipo_EjercicioController : ControllerBase
{
    private readonly AppDbContext _context;

    public Tipo_EjercicioController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tipo_Ejercicio>>> GetTipoEjercicios()
    {
        return await _context.Tipo_Ejercicio.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Tipo_Ejercicio>> GetTipoEjercicio(int id)
    {
        var tipoEjercicio = await _context.Tipo_Ejercicio.FindAsync(id);

        if (tipoEjercicio == null)
        {
            return NotFound();
        }

        return tipoEjercicio;
    }

    [HttpPost]
    public async Task<ActionResult<Tipo_Ejercicio>> PostTipoEjercicio(Tipo_Ejercicio tipoEjercicio)
    {
        _context.Tipo_Ejercicio.Add(tipoEjercicio);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTipoEjercicio), new { id = tipoEjercicio.Codigo }, tipoEjercicio);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTipoEjercicio(int id, Tipo_Ejercicio tipoEjercicio)
    {
        if (id != tipoEjercicio.Codigo)
        {
            return BadRequest();
        }

        _context.Entry(tipoEjercicio).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTipoEjercicio(int id)
    {
        var tipoEjercicio = await _context.Tipo_Ejercicio.FindAsync(id);
        if (tipoEjercicio == null)
        {
            return NotFound();
        }

        _context.Tipo_Ejercicio.Remove(tipoEjercicio);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
