using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Data;

[Route("api/[controller]")]
[ApiController]
public class TipoPagosController : ControllerBase
{
    private readonly AppDbContext _context;

    public TipoPagosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tipo_Pagos>>> GetTipoPagos()
    {
        return await _context.Tipo_Pagos.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Tipo_Pagos>> GetTipoPago(string id)
    {
        var tipoPago = await _context.Tipo_Pagos.FindAsync(id);

        if (tipoPago == null)
        {
            return NotFound();
        }

        return tipoPago;
    }

    [HttpPost]
    public async Task<ActionResult<Tipo_Pagos>> PostTipoPago(Tipo_Pagos tipoPago)
    {
        _context.Tipo_Pagos.Add(tipoPago);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTipoPago), new { id = tipoPago.CodigoPago }, tipoPago);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTipoPago(string id, Tipo_Pagos tipoPago)
    {
        if (id != tipoPago.CodigoPago)
        {
            return BadRequest();
        }

        _context.Entry(tipoPago).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTipoPago(string id)
    {
        var tipoPago = await _context.Tipo_Pagos.FindAsync(id);
        if (tipoPago == null)
        {
            return NotFound();
        }

        _context.Tipo_Pagos.Remove(tipoPago);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
