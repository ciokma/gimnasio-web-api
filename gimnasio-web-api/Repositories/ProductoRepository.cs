using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace gimnasio_web_api.Repositories
{
    public class ProductoRepository : IRepository<Producto, int>
    {
        private readonly AppDbContext _context;

        public ProductoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _context.Producto.ToListAsync();
        }

        public async Task<Producto> GetByIdAsync(int id)
        {
            var producto = await _context.Producto.FindAsync(id);
            if (producto == null)
            {
                throw new KeyNotFoundException();
            }
            return producto;
        }

        public async Task AddAsync(Producto entity)
        {
            await _context.Producto.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Producto entity)
        {
            _context.Producto.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var producto = await _context.Producto.FindAsync(id);
            if (producto == null)
            {
                throw new KeyNotFoundException();
            }
            _context.Producto.Remove(producto);
            await _context.SaveChangesAsync();
        }
    }
}