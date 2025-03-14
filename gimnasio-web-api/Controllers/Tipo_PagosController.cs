using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Tipo_PagosController : ControllerBase
    {
        private readonly IRepository<Tipo_Pagos, string> _repository;

        public Tipo_PagosController(IRepository<Tipo_Pagos, string> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tipo_Pagos>>> GetTipoPagos()
        {
            var tipoPagos = await _repository.GetAllAsync();
            return Ok(tipoPagos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tipo_Pagos>> GetTipoPago(string id)
        {
            try
            {
                var tipoPago = await _repository.GetByIdAsync(id);
                return Ok(tipoPago);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Tipo_Pagos>> PostTipoPago(Tipo_Pagos tipoPago)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.AddAsync(tipoPago);
            return CreatedAtAction(nameof(GetTipoPago), new { id = tipoPago.CodigoPago }, tipoPago);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoPago(string id, Tipo_Pagos tipoPago)
        {
            if (id != tipoPago.CodigoPago)
            {
                return BadRequest();
            }

            try
            {
                await _repository.UpdateAsync(tipoPago);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoPago(string id)
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
