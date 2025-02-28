using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Data;
using gimnasio_web_api.Repositories;
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

            if (!fechasConVentas.Any())
            {
                return NotFound("No se encontraron fechas con ventas.");
            }

            return Ok(fechasConVentas);
        }
        [HttpGet("{fecha}")]
        public ActionResult<IEnumerable<Venta>> GetVentasPorFecha([FromQuery] DateTime fecha)
        {
            var ventas = _context.Venta
                                 .Where(v => v.Fecha_venta.Date == fecha.Date)
                                 .OrderBy(v => v.Fecha_venta)
                                 .ToList();

            if (!ventas.Any())
            {
                return NotFound("No se encontraron ventas para la fecha especificada.");
            }

            return Ok(ventas);
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
    }
}