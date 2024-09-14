using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Tipo_EjercicioController : ControllerBase
    {
        private readonly IRepository<Tipo_Ejercicio> _repository;

        public Tipo_EjercicioController(IRepository<Tipo_Ejercicio> repository)
        {
            _repository = repository;
        }

        // GET: api/Tipo_Ejercicio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tipo_Ejercicio>>> GetTipoEjercicios()
        {
            var tipoEjercicios = await _repository.GetAllAsync();
            return Ok(tipoEjercicios);
        }

        // GET: api/Tipo_Ejercicio/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Tipo_Ejercicio>> GetTipoEjercicio(int id)
        {
            try
            {
                var tipoEjercicio = await _repository.GetByIdAsync(id);
                return Ok(tipoEjercicio);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: api/Tipo_Ejercicio
        [HttpPost]
        public async Task<ActionResult<Tipo_Ejercicio>> PostTipoEjercicio(Tipo_Ejercicio tipoEjercicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.AddAsync(tipoEjercicio);

            return CreatedAtAction(nameof(GetTipoEjercicio), new { id = tipoEjercicio.Codigo }, tipoEjercicio);
        }

        // PUT: api/Tipo_Ejercicio/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoEjercicio(int id, Tipo_Ejercicio tipoEjercicio)
        {
            if (id != tipoEjercicio.Codigo)
            {
                return BadRequest();
            }

            try
            {
                await _repository.UpdateAsync(tipoEjercicio);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Tipo_Ejercicio/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoEjercicio(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}