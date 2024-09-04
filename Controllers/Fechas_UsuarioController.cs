using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;

namespace gimnasioNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Fechas_UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Fechas_UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Fechas_Usuario
        [HttpGet]
        public async Task<ActionResult<List<object>>> GetFechasUsuarios([FromQuery] int? usuarioId)
        {
            try
            {
                var query = _context.Fechas_Usuarios.AsQueryable();

                if (usuarioId.HasValue)
                {
                    query = query.Where(f => f.UsuarioId == usuarioId.Value);
                }

                var fechasUsuarios = await query
                    .Include(f => f.Usuario)
                    .ToListAsync();

                var fechasUsuariosDto = fechasUsuarios.Select(f => new
                {
                    f.Id,
                    f.UsuarioId,
                    f.FechaPago,
                    f.FechaPagoA,
                    f.FechaVencimiento,
                    Usuario = new
                    {
                        f.Usuario!.Codigo,
                        f.Usuario.Nombres,
                        f.Usuario.Apellidos,
                        f.Usuario.Telefono,
                        f.Usuario.Foto,
                        f.Usuario.FechaIngreso,
                        f.Usuario.Activo,
                        f.Usuario.Observaciones
                    }
                }).ToList();

                return Ok(fechasUsuariosDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Fechas_Usuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetFechasUsuario(int id)
        {
            var fechasUsuario = await _context.Fechas_Usuarios
                .Include(f => f.Usuario)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fechasUsuario == null)
            {
                return NotFound();
            }

            var fechasUsuarioDto = new
            {
                fechasUsuario.Id,
                fechasUsuario.UsuarioId,
                fechasUsuario.FechaPago,
                fechasUsuario.FechaPagoA,
                fechasUsuario.FechaVencimiento,
                Usuario = new
                {
                    fechasUsuario.Usuario!.Codigo,
                    fechasUsuario.Usuario.Nombres,
                    fechasUsuario.Usuario.Apellidos,
                    fechasUsuario.Usuario.Telefono,
                    fechasUsuario.Usuario.Foto,
                    fechasUsuario.Usuario.FechaIngreso,
                    fechasUsuario.Usuario.Activo,
                    fechasUsuario.Usuario.Observaciones
                }
            };

            return Ok(fechasUsuarioDto);
        }

        // POST: api/Fechas_Usuario
        [HttpPost]
        public async Task<ActionResult<Fechas_Usuario>> PostFechasUsuario([FromBody] Fechas_Usuario fechasUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios.FindAsync(fechasUsuario.UsuarioId);
            if (usuario == null)
            {
                ModelState.AddModelError("UsuarioId", "UsuarioId es requerido y debe ser un usuario existente.");
                return BadRequest(ModelState);
            }

            if (fechasUsuario.FechaPago > fechasUsuario.FechaVencimiento)
            {
                ModelState.AddModelError("FechaPago", "La fecha de pago no puede ser mayor que la fecha de vencimiento.");
                return BadRequest(ModelState);
            }

            try
            {
                fechasUsuario.Usuario = usuario;
                _context.Fechas_Usuarios.Add(fechasUsuario);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetFechasUsuario), new { id = fechasUsuario.Id }, fechasUsuario);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        // DELETE: api/Fechas_Usuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFechasUsuario(int id)
        {
            var fechasUsuario = await _context.Fechas_Usuarios.FindAsync(id);
            if (fechasUsuario == null)
            {
                return NotFound();
            }

            _context.Fechas_Usuarios.Remove(fechasUsuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FechasUsuarioExists(int id)
        {
            return _context.Fechas_Usuarios.Any(e => e.Id == id);
        }
    }
}
