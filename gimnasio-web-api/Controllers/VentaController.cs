using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Data;
using gimnasio_web_api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public VentaController(AppDbContext context)
        {
            _context = context;
        }
         [HttpGet("fechas")]
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
        [HttpGet("Ventas_Fecha")]
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
    }
}