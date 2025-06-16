using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Services;
using gimnasio_web_api.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConfiguracionesSistemaController : ControllerBase
    {
        private readonly IRepository<ConfiguracionesSistema, int> _repository;
        private readonly ConfiguracionService _configuracionService;
        public ConfiguracionesSistemaController(IRepository<ConfiguracionesSistema, int> repository, ConfiguracionService configuracionService)
        {
            _configuracionService = configuracionService;
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConfiguracionesSistema>>> GetAll()
        {
            var configuraciones = await _repository.GetAllAsync();
            return Ok(configuraciones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConfiguracionesSistema>> GetById(int id)
        {
            try
            {
                var configuracion = await _repository.GetByIdAsync(id);
                return Ok(configuracion);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ConfiguracionesSistema configuracion)
        {
            await _repository.AddAsync(configuracion);
            return CreatedAtAction(nameof(GetById), new { id = configuracion.Id }, configuracion);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ConfiguracionesSistema configuracion)
        {
            if (id != configuracion.Id)
            {
                return BadRequest("El ID del parámetro no coincide con el ID de la entidad.");
            }

            try
            {
                await _repository.UpdateAsync(configuracion);
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
        [HttpGet("usuarios-inactivos")]
        public async Task<ActionResult<List<UsuariosInactivosDto>>> ObtenerUsuariosInactivos()
        {
            var usuarios = await _configuracionService.ObtenerUsuariosInactivosAsync();
            return Ok(usuarios);
        }
    }
}