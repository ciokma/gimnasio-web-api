using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;

namespace gimnasioNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagePath;
        private readonly string _defaultImageName = "Default.png";

        public UsuariosController(AppDbContext context)
        {
            _context = context;
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUsuarios(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        [HttpPut("{id}")]
public async Task<IActionResult> PutUsuarios(int id, [FromForm] Usuarios usuarios, IFormFile? foto)
{
    if (id != usuarios.Codigo)  // Verifica si el ID en la URL coincide con el ID del usuario
    {
        return BadRequest("El ID del usuario no coincide."+id+ "y se espera"+ usuarios.Codigo);
    }

    var existingUser = await _context.Usuarios.FindAsync(id);
    if (existingUser == null)
    {
        return NotFound();
    }

    // Actualiza la información del usuario
    existingUser.Nombres = usuarios.Nombres;
    existingUser.Apellidos = usuarios.Apellidos;
    existingUser.Telefono = usuarios.Telefono;
    existingUser.Activo = usuarios.Activo;
    existingUser.Observaciones = usuarios.Observaciones;
    existingUser.FechaIngreso = usuarios.FechaIngreso ?? existingUser.FechaIngreso;  // Manejo de la fecha de ingreso

    // Manejo de la foto
    if (foto != null)
    {
        var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(foto.FileName);
        var filePath = Path.Combine(_imagePath, newFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await foto.CopyToAsync(stream);
        }

        // Elimina la foto antigua si es necesario
        if (!string.IsNullOrEmpty(existingUser.Foto) && existingUser.Foto != _defaultImageName)
        {
            var oldFilePath = Path.Combine(_imagePath, existingUser.Foto);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
        }

        existingUser.Foto = newFileName;
    }

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, $"Error al actualizar el usuario: {ex.Message}");
    }
    catch (Exception ex)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, $"Error inesperado: {ex.Message}");
    }

    return NoContent();
}


        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<Usuarios>> PostUsuarios([FromForm] Usuarios usuario, IFormFile? foto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (foto != null)
            {
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(foto.FileName);
                var filePath = Path.Combine(_imagePath, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                usuario.Foto = newFileName;
            }
            else
            {
                // Copiar la imagen predeterminada si no existe
                var defaultImagePath = Path.Combine(_imagePath, _defaultImageName);
                if (!System.IO.File.Exists(defaultImagePath))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "La imagen predeterminada no existe." });
                }

                // Copiar la imagen predeterminada al directorio de imágenes del usuario
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(defaultImagePath);
                var newFilePath = Path.Combine(_imagePath, newFileName);
                System.IO.File.Copy(defaultImagePath, newFilePath);

                usuario.Foto = newFileName;
            }

            _context.Usuarios.Add(usuario);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al agregar el usuario: " + ex.Message });
            }

            return CreatedAtAction(nameof(GetUsuarios), new { id = usuario.Codigo }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarios(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_imagePath, usuario.Foto);
            if (System.IO.File.Exists(filePath) && usuario.Foto != _defaultImageName)
            {
                System.IO.File.Delete(filePath);
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuariosExists(int id)
        {
            return _context.Usuarios.Any(e => e.Codigo == id);
        }

        // GET: api/Fechas_Usuario/5
        [HttpGet("Fechas_Usuario/{id}")]
        public async Task<ActionResult<Fechas_Usuario>> GetFechasUsuario(int id)
        {
            var fechasUsuario = await _context.Fechas_Usuarios.FindAsync(id);

            if (fechasUsuario == null)
            {
                return NotFound();
            }

            return fechasUsuario;
        }
    }
}
