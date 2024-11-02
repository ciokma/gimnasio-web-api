using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace gimnasio_web_api.Repositories
{
    public class PagoRepository : IRepository<Pago, int>
    {
        private readonly AppDbContext _context;

        public PagoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Pago entity)
        {
            await _context.Pagos.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);

            if (pago == null)
            {
                throw new KeyNotFoundException();
            }

            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Pago>> GetAllAsync()
        {
            return await _context.Pagos.Include(p => p.Usuario).ToListAsync();
        }

        public async Task<Pago> GetByIdAsync(int id)
        {
            var pago = await _context.Pagos.Include(p => p.Usuario).FirstOrDefaultAsync(p => p.CodigoPago == id);

            if (pago == null)
            {
                throw new KeyNotFoundException();
            }
            return pago;
        }

        public async Task UpdateAsync(Pago entity)
        {
            _context.Pagos.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}