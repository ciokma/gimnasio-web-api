using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;
namespace gimnasio_web_api.Repositories
{
    public class Tipo_EjercicioRepository : IRepository<Tipo_Ejercicio>
    {
        private readonly AppDbContext _context;
        public Tipo_EjercicioRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Tipo_Ejercicio entity)
        {
            await _context.Tipo_Ejercicio.AddAsync(entity);
            await _context.SaveChangesAsync(); 
        }
        public async Task DeleteAsync(int id)
        {
            var tipo_ejercicio = await _context.Tipo_Ejercicio.FindAsync(id);

            if (tipo_ejercicio == null)
            {
                throw new KeyNotFoundException();
            }

            _context.Tipo_Ejercicio.Remove(tipo_ejercicio);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Tipo_Ejercicio>> GetAllAsync()
        {
            return await _context.Tipo_Ejercicio.ToListAsync();
        }
        public async Task<Tipo_Ejercicio> GetByIdAsync(int id)
        {
            var tipo_ejercicio = await _context.Tipo_Ejercicio.FindAsync(id);

            if (tipo_ejercicio == null)
            {
               throw new KeyNotFoundException();
            }
            return tipo_ejercicio;
        }
        public async Task UpdateAsync(Tipo_Ejercicio entity)
        {
            _context.Tipo_Ejercicio.Update(entity);
            await _context.SaveChangesAsync();

        }
    }
}