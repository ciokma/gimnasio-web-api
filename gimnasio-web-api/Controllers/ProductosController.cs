using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        //private readonly AppDbContext _context;
        private readonly IRepository<Usuarios> _repository;
        public ProductosController(IRepository<Producto> repository)
        {
        {
            _repository = repository;
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProducto([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            return Ok(await _repository.GetPagedAsync(skip, take));
        }

        // GET: api/Productos/5
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

        // PUT: api/Productos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.CodigoProducto)
            {
                return BadRequest();
            }

            try
            {
                var existingProducto = await _repository.GetByIdAsync(id);

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
        // POST: api/Productos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            await _repository.AddAsync(producto);

            return CreatedAtAction("GetProducto", new { id = producto.CodigoProducto }, producto);
        }


        // DELETE: api/Productos/5
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
