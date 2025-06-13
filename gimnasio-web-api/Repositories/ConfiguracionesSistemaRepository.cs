using System.Collections.Generic;
using System.Threading.Tasks;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace gimnasio_web_api.Repositories
{
    public class ConfiguracionesSistemaRepository : IRepository<ConfiguracionesSistema, int>
    {
        private readonly AppDbContext _context;
        public ConfiguracionesSistemaRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ConfiguracionesSistema entity)
        {
            _context.ConfiguracionesSistema.Add(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<ConfiguracionesSistema> GetByIdAsync(int id)
        {
            var configuracion = await _context.ConfiguracionesSistema.FindAsync(id);
            if (configuracion == null)
            {
                throw new KeyNotFoundException($"No se encontró una configuración con el ID {id}");
            }
            return configuracion;
        }
        public async Task UpdateAsync(ConfiguracionesSistema entity)
        {
            var configuracion = await _context.ConfiguracionesSistema.FindAsync(entity.Id);
            if (configuracion == null)
            {
                throw new KeyNotFoundException($"No se encontró una configuración con el ID {entity.Id}");
            }
            _context.Entry(configuracion).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ConfiguracionesSistema>> GetAllAsync()
        {
            return await _context.ConfiguracionesSistema.ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var configuracion = await _context.ConfiguracionesSistema.FindAsync(id);
            if (configuracion == null)
            {
                throw new KeyNotFoundException($"No se encontró una configuración con el ID {id}");
            }
            _context.ConfiguracionesSistema.Remove(configuracion);
            await _context.SaveChangesAsync();
        }
    }
}