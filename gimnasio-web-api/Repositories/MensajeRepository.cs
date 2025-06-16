using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace gimnasio_web_api.Repositories
{
    public class MensajeRepository : IMensajeRepository
    {
        private readonly AppDbContext _context;
        public MensajeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Mensaje>> GetByEmisorSistemaCrossfitAsync()
        {
            return await _context.Mensaje
                .Where(m => m.Emisor == "SISTEMA-CROSSFIT" || m.Emisor == "SISTEMA")
                .ToListAsync();
        }

        public async Task<IEnumerable<Mensaje>> GetAllExceptSistemaCrossfitAsync()
        {
            return await _context.Mensaje
                .Where(m => m.Emisor != "SISTEMA-CROSSFIT" && m.Emisor != "SISTEMA")
                .ToListAsync();
        }
        public async Task<(int sistemaCount, List<int> sistemaIds, int usuarioCount, List<int> usuarioIds)> GetNumberMessagesAsync()
        {
            var sistemaQuery = _context.Mensaje
                .Where(m => (m.Emisor == "SISTEMA-CROSSFIT" || m.Emisor == "SISTEMA") && !m.Leido);

            var usuarioQuery = _context.Mensaje
                .Where(m => m.Emisor != "SISTEMA-CROSSFIT" && m.Emisor != "SISTEMA" && !m.Leido);

            var sistemaCount = await sistemaQuery.CountAsync();
            var sistemaIds = await sistemaQuery.Select(m => m.Codigo).ToListAsync();

            var usuarioCount = await usuarioQuery.CountAsync();
            var usuarioIds = await usuarioQuery.Select(m => m.Codigo).ToListAsync();

            return (sistemaCount, sistemaIds, usuarioCount, usuarioIds);
        }
        public async Task<Mensaje> GetByIdAsync(int id)
        {
            var mensaje = await _context.Mensaje.FindAsync(id);
            if (mensaje == null)
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
        public async Task MarkMessagesAsReadAsync(List<int> mensajeIds)
        {
            var mensajes = await _context.Mensaje.Where(m => mensajeIds.Contains(m.Codigo)).ToListAsync();
            foreach (var mensaje in mensajes)
            {
                mensaje.Leido = true;
            }
            await _context.SaveChangesAsync();
        }
    }
}