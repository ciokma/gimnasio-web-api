using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using gimnasio_web_api.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VentaController : ControllerBase
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly ILogger<VentaController> _logger;
        private readonly AppDbContext _context;

        public VentaController(IVentaRepository ventaRepository, ILogger<VentaController> logger, AppDbContext context)
        {
            _ventaRepository = ventaRepository;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetFechasConVentas()
        {
            var fechasConVentas = await _ventaRepository.GetFechasConVentasAsync();

            return fechasConVentas.Any() ? Ok(fechasConVentas) : NotFound("No se encontraron fechas con ventas.");
        }

        [HttpGet("{fechaInicio}")]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentasPorRangoFechas([FromRoute] string fechaInicio, [FromQuery] string? fechaFin = null)
        {
            if (!DateTime.TryParse(fechaInicio, out DateTime fechaInicioParsed))
                return BadRequest("Formato de fecha inválido en 'fechaInicio'.");

            DateTime? fechaFinParsed = null;

            if (!string.IsNullOrEmpty(fechaFin))
            {
                if (!DateTime.TryParse(fechaFin, out DateTime fechaFinValue))
                    return BadRequest("Formato de fecha inválido en 'fechaFin'.");

                fechaFinParsed = fechaFinValue;
            }

            var ventas = await _ventaRepository.GetVentasPorRangoFechasAsync(fechaInicioParsed, fechaFinParsed);

            return ventas.Any() ? Ok(ventas) : NotFound("No se encontraron ventas en el rango de fechas especificado.");
        }

        [HttpGet("detalle/{id}")]
        public async Task<ActionResult<Venta>> GetVentaPorId(int id)
        {
            var venta = await _ventaRepository.GetVentaPorIdAsync(id);
            return venta != null ? Ok(venta) : NotFound($"No se encontró ninguna venta con el ID {id}.");
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

            venta.Nombre_vendedor = vendedor.Nombre;

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
                return BadRequest(ModelState);
            }

            await _ventaRepository.AddVentaAsync(venta);

            return CreatedAtAction(nameof(PostVenta), new { id = venta.Codigo_venta }, venta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVenta(int id, Venta venta)
        {
            if (id != venta.Codigo_venta)
                return BadRequest("El ID de la venta no coincide.");

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

            venta.Nombre_vendedor = vendedor.Nombre;

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
                return BadRequest(ModelState);
            }

            await _ventaRepository.UpdateVentaAsync(venta);

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            await _ventaRepository.DeleteVentaAsync(id);
            return NoContent();
        }
    }
}