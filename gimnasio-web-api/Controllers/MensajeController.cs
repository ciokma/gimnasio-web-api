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
    //[Authorize]
    public class MensajeController : ControllerBase
    {
        private readonly IMensajeRepository _repository;
        private readonly ILogger<MensajeController> _logger;
        public MensajeController(IMensajeRepository repository, ILogger<MensajeController> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        [HttpGet("emisor/sistema-crossfit")]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetMensajesSistemaCrossfit()
        {
            var mensajes = await _repository.GetByEmisorSistemaCrossfitAsync();
            return Ok(mensajes);
        }

        [HttpGet("emisor/otros")]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetMensajesExceptoSistemaCrossfit()
        {
            var mensajes = await _repository.GetAllExceptSistemaCrossfitAsync();
            return Ok(mensajes);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Mensaje>> GetMensaje(int id)
        {
            var mensaje = await _repository.GetByIdAsync(id);
            if (mensaje == null)
            {
                return NotFound();
            }
            return mensaje;
        }
        [HttpPost]
        public async Task<ActionResult<Mensaje>> PostMensaje(Mensaje mensaje)
        {
            await _repository.AddAsync(mensaje);
            return CreatedAtAction(nameof(GetMensajesExceptoSistemaCrossfit), new { id = mensaje.Codigo }, mensaje);
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
        [HttpGet("no-leidos")]
        public async Task<ActionResult<object>> GetEstadisticasMensajes()
        {
            var (sistemaCount, sistemaIds, usuarioCount, usuarioIds) = await _repository.GetNumberMessagesAsync();

            var result = new
            {
                Sistema = new
                {
                    Total = sistemaCount,
                    MensajesIds = sistemaIds
                },
                Usuarios = new
                {
                    Total = usuarioCount,
                    MensajesIds = usuarioIds
                }
            };

            return Ok(result);
        }
        [HttpPut("marcar-leidos")]
        public async Task<IActionResult> MarcarMensajesLeidos([FromBody] List<int> mensajeIds)
        {
            if (mensajeIds == null || !mensajeIds.Any())
                return BadRequest("No se proporcionaron IDs de mensajes.");

            await _repository.MarkMessagesAsReadAsync(mensajeIds);
            return NoContent();
        }
    }
}