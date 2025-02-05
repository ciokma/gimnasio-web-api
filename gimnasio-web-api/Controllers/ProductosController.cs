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
    public class ProductosController : ControllerBase
    {
        private readonly IRepository<Producto, int> _repository;

        public ProductosController(IRepository<Producto, int> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            var productos = await _repository.GetAllAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _repository.GetByIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return producto;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.CodigoProducto)
            {
                return BadRequest();
            }

            try
            {
                await _repository.UpdateAsync(producto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Error de concurrencia al actualizar el producto.");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            await _repository.AddAsync(producto);
            return CreatedAtAction(nameof(GetProducto), new { id = producto.CodigoProducto }, producto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
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
