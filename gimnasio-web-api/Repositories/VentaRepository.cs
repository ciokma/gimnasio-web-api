using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Models;
using gimnasio_web_api.Data;

namespace gimnasio_web_api.Repositories
{
    public class VentaRepository : IVentaRepository
    {
        private readonly AppDbContext _context;

        public VentaRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Venta>> GetAllAsync()
        {
            return await _context.Venta.ToListAsync();
        }

        public async Task<Venta> GetByIdAsync(int id)
        {
            return await _context.Venta.FindAsync(id);
        }

        public async Task AddAsync(Venta entity) => await AddVentaAsync(entity);

        public async Task UpdateAsync(Venta entity) => await UpdateVentaAsync(entity);

        public async Task DeleteAsync(int id) => await DeleteVentaAsync(id);
        public async Task<IEnumerable<DateTime>> GetFechasConVentasAsync()
        {
            return await _context.Venta
                                 .Select(v => v.Fecha_venta.Date)
                                 .Distinct()
                                 .OrderBy(fecha => fecha)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Venta>> GetVentasPorRangoFechasAsync(DateTime fechaInicio, DateTime? fechaFin = null)
        {
            var query = _context.Venta.AsQueryable();

            if (fechaFin.HasValue)
            {
                query = query.Where(v => v.Fecha_venta.Date >= fechaInicio.Date && v.Fecha_venta.Date <= fechaFin.Value.Date);
            }
            else
            {
                query = query.Where(v => v.Fecha_venta.Date == fechaInicio.Date);
            }

            return await query.OrderBy(v => v.Fecha_venta).ToListAsync();
        }

        public async Task<Venta> GetVentaPorIdAsync(int id)
        {
            return await _context.Venta.FindAsync(id);
        }

        public async Task AddVentaAsync(Venta venta)
        {
            _context.Venta.Add(venta);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVentaAsync(Venta venta)
        {
            _context.Venta.Update(venta);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVentaAsync(int id)
        {
            var venta = await _context.Venta.FindAsync(id);
            if (venta != null)
            {
                _context.Venta.Remove(venta);
                await _context.SaveChangesAsync();
            }
        }
    }
}