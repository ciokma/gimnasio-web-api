using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Tipo_PagosController : ControllerBase
    {
        private readonly IRepository<Tipo_Pagos> _repository;

        public Tipo_PagosController(IRepository<Tipo_Pagos> repository)
        {
            _repository = repository;
        }

        // GET: api/Tipo_Pagos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tipo_Pagos>>> GetTipoPagos()
        {
            var tipoPagos = await _repository.GetAllAsync();
            return Ok(tipoPagos);
        }

        // GET: api/Tipo_Pagos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Tipo_Pagos>> GetTipoPago(int id)
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

        // POST: api/Tipo_Pagos
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

        // PUT: api/Tipo_Pagos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoPago(int id, Tipo_Pagos tipoPago)
        {
            if (id != int.Parse(tipoPago.CodigoPago))
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

        // DELETE: api/Tipo_Pagos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoPago(int id)
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