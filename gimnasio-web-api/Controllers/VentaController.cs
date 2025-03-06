using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VentaController> _logger;
        public VentaController(AppDbContext context, ILogger<VentaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<DateTime>> GetFechasConVentas()
        {
            var fechasConVentas = _context.Venta
                                           .Select(v => v.Fecha_venta.Date)
                                           .Distinct()
                                           .OrderBy(fecha => fecha)
                                           .ToList();

            return fechasConVentas.Any() ? Ok(fechasConVentas) : NotFound("No se encontraron fechas con ventas.");
        }

        [HttpGet("{fechaInicio}")]
        public ActionResult<IEnumerable<Venta>> GetVentasPorRangoFechas([FromRoute] string fechaInicio, [FromQuery] string? fechaFin = null)
        {
            if (!DateTime.TryParse(fechaInicio, out DateTime fechaInicioParsed))
            {
                return BadRequest("Formato de fecha inválido en 'fechaInicio'.");
            }

            if (string.IsNullOrEmpty(fechaFin))
            {
                var ventasUnicaFecha = _context.Venta
                    .Where(v => v.Fecha_venta.Date == fechaInicioParsed.Date)
                    .OrderBy(v => v.Fecha_venta)
                    .ToList();

                return ventasUnicaFecha.Any() ? Ok(ventasUnicaFecha) : NotFound("No se encontraron ventas para la fecha especificada.");
            }

            if (!DateTime.TryParse(fechaFin, out DateTime fechaFinParsed))
            {
                return BadRequest("Formato de fecha inválido en 'fechaFin'.");
            }

            if (fechaInicioParsed > fechaFinParsed)
            {
                return BadRequest("La 'fechaInicio' no puede ser mayor que 'fechaFin'.");
            }

            var ventasRango = _context.Venta
                .Where(v => v.Fecha_venta.Date >= fechaInicioParsed.Date && v.Fecha_venta.Date <= fechaFinParsed.Date)
                .OrderBy(v => v.Fecha_venta)
                .ToList();

            return ventasRango.Any() ? Ok(ventasRango) : NotFound("No se encontraron ventas en el rango de fechas especificado.");
        }

        [HttpPost]
        public async Task<ActionResult<Venta>> PostVenta(Venta venta)
        {
            if (string.IsNullOrEmpty(venta.Nombre_vendedor))
            {
                return BadRequest("La clave del vendedor es requerida.");
            }

            var vendedor = await _context.Administrador
                                            .FirstOrDefaultAsync(a => a.Clave == venta.Nombre_vendedor);

            if (vendedor == null)
            {
                return NotFound("Vendedor no encontrado con esa clave.");
            }

            _logger.LogInformation("Datos recibidos: {@Venta}", venta);

            venta.Nombre_vendedor = vendedor.Nombre;

            if (string.IsNullOrEmpty(venta.Nombre_vendedor))
            {
                _logger.LogWarning("El campo Nombre_vendedor no se asignó correctamente.");
                return BadRequest("El campo Nombre_vendedor no se ha asignado correctamente.");
            }

            if (venta.CodigoProducto == 0)
            {
                return BadRequest("Debe proporcionar un producto para registrar la venta.");
            }

            var producto = await _context.Producto
                                            .FirstOrDefaultAsync(p => p.CodigoProducto == venta.CodigoProducto);

            if (producto == null)
            {
                return BadRequest("El producto proporcionado no fue encontrado.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El modelo no es válido: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            _context.Venta.Add(venta);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Venta registrada exitosamente: {@Venta}", venta);

            return CreatedAtAction(nameof(PostVenta), new { id = venta.Codigo_venta }, venta);
        }
        [HttpGet("detalle/{id}")]
        public async Task<ActionResult<Venta>> GetVentaPorId(int id)
        {
            var venta = await _context.Venta.FindAsync(id);

            if (venta == null)
            {
                return NotFound($"No se encontró ninguna venta con el ID {id}.");
            }

            return Ok(venta);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVenta(int id, Venta venta)
        {
            if (id != venta.Codigo_venta)
            {
                return BadRequest("El ID de la venta no coincide.");
            }

            var ventaExistente = await _context.Venta.FindAsync(id);
            if (ventaExistente == null)
            {
                return NotFound("Venta no encontrada.");
            }

            ventaExistente.Fecha_venta = venta.Fecha_venta;
            ventaExistente.Nombre_vendedor = venta.Nombre_vendedor;
            ventaExistente.CodigoProducto = venta.CodigoProducto;
            ventaExistente.Total = venta.Total;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al actualizar la venta.");
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _context.Venta.FindAsync(id);
            if (venta == null)
            {
                return NotFound("Venta no encontrada.");
            }

            _context.Venta.Remove(venta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}