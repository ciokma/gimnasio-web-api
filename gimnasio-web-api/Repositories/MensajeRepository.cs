using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace gimnasio_web_api.Repositories
{
    public class MensajeRepository : IRepository<Mensaje, int>
    {
        private readonly AppDbContext _context;
        public MensajeRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Mensaje>> GetAllAsync()
        {
            return await _context.Mensaje.ToListAsync();
        }
        public async Task<Mensaje> GetByIdAsync(int id)
        {
            var mensaje = await _context.Mensaje.FindAsync(id);
            if(mensaje == null)
            {
                throw new KeyNotFoundException();
            }
            return mensaje;
        }
        public async Task AddAsync(Mensaje entity)
        {
            await _context.Mensaje.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Mensaje entity)
        {
            _context.Mensaje.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var mensaje = await _context.Mensaje.FindAsync(id);
            if (mensaje == null)
            {
                throw new KeyNotFoundException();
            }
            _context.Mensaje.Remove(mensaje);
            await _context.SaveChangesAsync();
        }
    }
}