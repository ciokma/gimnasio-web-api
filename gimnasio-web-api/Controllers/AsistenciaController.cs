using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Data;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsistenciaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AsistenciaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerUsuarioInfo(int id)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.Codigo == id)
                .Select(u => new UsuarioDto
                {
                    Codigo = u.Codigo,
                    Nombres = u.Nombres,
                    Apellidos = u.Apellidos,
                    Foto = u.Foto
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var ultimoPagoYFecha = await ObtenerUltimaInformacionPago(id);

            var resultado = new AsistenciaDto
            {
                Usuario = usuario,
                UltimaFechaUsuario = ultimoPagoYFecha?.UltimaFechaUsuario,
                UltimoPago = ultimoPagoYFecha?.UltimoPago
            };

            return Ok(resultado);
        }
        private async Task<AsistenciaDto> ObtenerUltimaInformacionPago(int usuarioId)
        {
            var ultimaFechaUsuario = await _context.Fechas_Usuarios
                .Where(fu => fu.UsuarioId == usuarioId)
                .OrderByDescending(fu => fu.FechaPago)
                .FirstOrDefaultAsync();
            var ultimoPago = await _context.Pagos
                .Where(p => p.CodigoUsuario == usuarioId)
                .OrderByDescending(p => p.FechaPago)
                .FirstOrDefaultAsync();

            if (ultimaFechaUsuario == null || ultimoPago == null)
            {
                return null;
            }

            var pagoDto = new PagoDto(
                ultimoPago.CodigoPago,
                ultimoPago.CodigoUsuario,
                ultimoPago.MesesPagados,
                ultimoPago.MesesPagadosA,
                ultimoPago.FechaPago,
                ultimoPago.Monto,
                ultimoPago.DetallePago,
                ultimoPago.IntervaloPago
            );

            return new AsistenciaDto
            {
                UltimaFechaUsuario = new FechasUsuarioDto
                {
                    FechaPago = ultimaFechaUsuario.FechaPago,
                    FechaVencimiento = ultimaFechaUsuario.FechaVencimiento
                },
                UltimoPago = pagoDto
            };
        }
    }
}