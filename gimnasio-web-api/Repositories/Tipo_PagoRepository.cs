using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;
namespace gimnasio_web_api.Repositories
{
    public class Tipo_PagoRepository : IRepository<Tipo_Pagos>
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
        public async Task DeleteAsync(int id)
        {
            var tipo_pagos = await _context.Tipo_Pagos.FindAsync(id);

            if (tipo_pagos == null)
            {
                throw new KeyNotFoundException();
   
            }
            _context.Tipo_Pagos.Remove(tipo_pagos);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Tipo_Pagos>> GetAllAsync()
        {
            return await _context.Tipo_Pagos.ToListAsync();
        }
        public async Task<Tipo_Pagos> GetByIdAsync(int id)
        {
            var tipo_pagos = await _context.Tipo_Pagos.FindAsync(id);

            if (tipo_pagos == null)
            {
               throw new KeyNotFoundException();
            }
            return tipo_pagos;
        }
        public async Task UpdateAsync(Tipo_Pagos entity)
        {
            _context.Tipo_Pagos.Update(entity);
            await _context.SaveChangesAsync();

        }
    }
}