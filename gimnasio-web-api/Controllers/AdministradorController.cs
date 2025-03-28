using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministradorController : ControllerBase
    {
        
        private readonly IAdministradorRepository _repository;

        public AdministradorController(IAdministradorRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Administrador>>> GetAdministradores()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Administrador>> GetAdministrador(int id)
        {
            var admin = await _repository.GetByIdAsync(id);
            if (admin == null)
                return NotFound();
            return Ok(admin);
        }

        [HttpPost]
        public async Task<ActionResult> CrearAdministrador(Administrador administrador)
        {
            // Cifrar la contraseña antes de guardarla
            administrador.Clave = BCrypt.Net.BCrypt.HashPassword(administrador.Clave);

            // Guardar el administrador con la clave cifrada
            await _repository.AddAsync(administrador);

            return Ok(administrador);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarAdministrador(int id, Administrador administrador)
        {
            if (id != administrador.Id)
                return BadRequest();
            await _repository.UpdateAsync(administrador);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarAdministrador(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
