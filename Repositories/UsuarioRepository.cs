using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace gimnasio_web_api.Repositories
{
    public class UsuarioRepository : IRepository<Usuarios>
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Usuarios entity)
        {
            await _context.Usuarios.AddAsync(entity);
            await _context.SaveChangesAsync(); 
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                throw new KeyNotFoundException();
   
            }
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Usuarios>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuarios> GetByIdAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
               throw new KeyNotFoundException();
            }
            return usuario;
        }
        /// <summary>
        /// Actualiza una entidad usuario en base de datos
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Usuarios entity)
        {
            _context.Usuarios.Update(entity);
            await _context.SaveChangesAsync();

        }
    }
}