using System.Collections.Generic;
using System.Threading.Tasks;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace gimnasio_web_api.Repositories
{
    public class Tipo_PagoRepository : IRepository<Tipo_Pagos, string>
    {
        private readonly AppDbContext _context;

        public Tipo_PagoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Tipo_Pagos entity)
        {
            await _context.Tipo_Pagos.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var tipoPago = await _context.Tipo_Pagos.FindAsync(id);
            if (tipoPago == null)
            {
                throw new KeyNotFoundException($"Tipo_Pago con Código {id} no encontrado.");
            }

            _context.Tipo_Pagos.Remove(tipoPago);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Tipo_Pagos>> GetAllAsync()
        {
            return await _context.Tipo_Pagos.ToListAsync();
        }

        public async Task<Tipo_Pagos> GetByIdAsync(string id)
        {
            var tipoPago = await _context.Tipo_Pagos.FindAsync(id);
            if (tipoPago == null)
            {
                throw new KeyNotFoundException($"Tipo_Pago con Código {id} no encontrado.");
            }
            return tipoPago;
        }

        public async Task UpdateAsync(Tipo_Pagos entity)
        {
            _context.Tipo_Pagos.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
