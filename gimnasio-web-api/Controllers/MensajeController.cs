using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MensajeController : ControllerBase
    {
        private readonly IRepository<Mensaje, int> _repository;
        private readonly ILogger<MensajeController> _logger;
        public MensajeController(IRepository<Mensaje, int> repository, ILogger<MensajeController> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetMensajes()
        {
            var mensajes = await _repository.GetAllAsync();
            return Ok(mensajes);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Mensaje>> GetMensaje(int id)
        {
            var mensaje = await _repository.GetByIdAsync(id);
            if(mensaje == null)
            {
                return NotFound();
            }
            return mensaje;
        }
        [HttpPost]
        public async Task<ActionResult<Mensaje>> PostMensaje(Mensaje mensaje)
        {
            await _repository.AddAsync(mensaje);
            return CreatedAtAction(nameof(GetMensajes), new {id = mensaje.Codigo}, mensaje);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMensaje(int id, Mensaje mensaje)
        {
            if (id != mensaje.Codigo)
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
            catch (DbUpdateConcurrencyException)
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