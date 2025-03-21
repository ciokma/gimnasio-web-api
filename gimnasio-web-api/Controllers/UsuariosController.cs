using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Data;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace gimnasioNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagePath;
        private readonly string _defaultImageName = "Default.png";

        //new changes
        private readonly IRepository<Usuarios, int> _repository;

        public UsuariosController(AppDbContext context, IRepository<Usuarios, int> repository)
        {
            _context = context;
            _repository = repository;
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<List<Usuarios>> GetUsuariosPorLetra(string letra)
        {
            Console.WriteLine($"Received letter: {letra}");
            if (string.IsNullOrEmpty(letra) || letra.Length != 1)
            {
                return new List<Usuarios>();
            }

            var usuarios = await _repository.GetAllAsync();

            var usuariosFiltrados = usuarios
                .Where(u => !string.IsNullOrEmpty(u.Nombres) && u.Nombres[0].ToString().ToUpper() == letra.ToUpper())
                .ToList();

            return usuariosFiltrados;
        }
        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUsuarios(int id)
        {
            //var usuario = await _context.Usuarios.FindAsync(id);
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuarios(int id, [FromForm] Usuarios usuarios, IFormFile? foto)
        {
            if (id != usuarios.Codigo)
            {
                return BadRequest($"El ID del usuario no coincide: {id} se esperaba {usuarios.Codigo}.");
            }

            //var existingUser = await _context.Usuarios.FindAsync(id);
            var existingUser = await _repository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.Nombres = usuarios.Nombres;
            existingUser.Apellidos = usuarios.Apellidos;
            existingUser.Telefono = usuarios.Telefono;
            existingUser.Activo = usuarios.Activo;
            existingUser.Observaciones = usuarios.Observaciones;
            existingUser.FechaIngreso = usuarios.FechaIngreso ?? existingUser.FechaIngreso;

            if (foto != null)
            {
                var imagePath = _imagePath ?? throw new InvalidOperationException("La ruta de la imagen no está configurada.");
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(foto.FileName);
                var filePath = Path.Combine(imagePath, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(existingUser.Foto) && existingUser.Foto != _defaultImageName)
                {
                    var oldFilePath = Path.Combine(imagePath, existingUser.Foto);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                existingUser.Foto = newFileName;
            }

            try
            {
                //await _context.SaveChangesAsync();
                await _repository.UpdateAsync(existingUser);
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
                var filePath = Path.Combine(_imagePath ?? string.Empty, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                usuario.Foto = newFileName;
            }
            else
            {
                var defaultImagePath = Path.Combine(_imagePath ?? string.Empty, _defaultImageName);
                if (!System.IO.File.Exists(defaultImagePath))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "La imagen predeterminada no existe." });
                }

                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(defaultImagePath);
                var newFilePath = Path.Combine(_imagePath ?? string.Empty, newFileName);
                System.IO.File.Copy(defaultImagePath, newFilePath);

                usuario.Foto = newFileName;
            }

            //_context.Usuarios.Add(usuario);

            try
            {
                //await _context.SaveChangesAsync();
                await _repository.AddAsync(usuario);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al agregar el usuario: " + ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error inesperado: " + ex.Message });
            }

            return CreatedAtAction("GetUsuarios", new { id = usuario.Codigo }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarios(int id)
        {
            //var usuario = await _context.Usuarios.FindAsync(id);
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(usuario.Foto) && usuario.Foto != _defaultImageName)
            {
                var filePath = Path.Combine(_imagePath ?? string.Empty, usuario.Foto);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            await _repository.DeleteAsync(usuario.Codigo);

            return Ok();
        }
        /*
        private bool UsuariosExists(int id)
        {
            return _context.Usuarios.Any(e => e.Codigo == id);
        }
        */
        [HttpGet("PrimeraLetraUsuarios")]
        public async Task<ActionResult<List<UsuariosLetraDto>>> GetUsuariosPorLetraAsync()
        {
            var resultado = await _context.Usuarios
                .Where(u => !string.IsNullOrEmpty(u.Nombres) && System.Text.RegularExpressions.Regex.IsMatch(u.Nombres, "^[A-Za-z]"))
                .GroupBy(u => u.Nombres.Substring(0, 1).ToUpper())
                .Select(g => new UsuariosLetraDto
                {
                    PrimeraLetra = g.Key,
                    Cantidad = g.Count()
                })
                .OrderBy(r => r.PrimeraLetra)
                .ToListAsync();

            return Ok(resultado);
        }
        [HttpGet("BuscarUsuarios")]
        public async Task<ActionResult<List<Usuarios>>> BuscarUsuarios(string? nombres = null, string? apellidos = null)
        {
            if (string.IsNullOrEmpty(nombres) && string.IsNullOrEmpty(apellidos))
            {
                return BadRequest("Debe proporcionar al menos un valor para 'nombres' o 'apellidos'.");
            }

            var usuariosQuery = _context.Usuarios.AsQueryable();
            if (!string.IsNullOrEmpty(nombres))
            {
                usuariosQuery = usuariosQuery.Where(u => u.Nombres.Contains(nombres));
            }

            if (!string.IsNullOrEmpty(apellidos))
            {
                usuariosQuery = usuariosQuery.Where(u => u.Apellidos.Contains(apellidos));
            }
            var usuariosFiltrados = await usuariosQuery.ToListAsync();

            return Ok(usuariosFiltrados);
        }
    }
}