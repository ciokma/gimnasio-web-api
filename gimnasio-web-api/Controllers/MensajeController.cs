using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using Microsoft.EntityFrameworkCore;
namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajeController : ControllerBase
    {
        private readonly IRepository<Mensaje, int> _repository;
        public MensajeController(IRepository<Mensaje, int> repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetMensajes()
        {
            var mensajes = await _repository.GetAllAsync();
            return Ok(mensajes);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Mensaje>> GetProducto(int id)
        {
            var mensaje = await _repository.GetByIdAsync(id);
            if(GetProducto == null)
            {
                return NotFound();
            }
            return mensaje;
        }
        [HttpPost]
        public async Task<ActionResult<Mensaje>> PostProducto(Mensaje mensaje)
        {
            await _repository.AddAsync(mensaje);
            return CreatedAtAction(nameof(GetMensajes), new {id = mensaje.Codigo}, mensaje);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPorducto(int id, Mensaje mensaje)
        {
            if (id !=mensaje.Codigo)
            {
                return BadRequest();
            }
            try
            {
                await _repository.UpdateAsync(mensaje);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch(DbUpdateConcurrencyException)
            {
                return Conflict("Error de concurrencia al actualizar el mensaje.");
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMensaje(int id)
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