using System.Collections.Generic;
using System.Threading.Tasks;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace gimnasio_web_api.Repositories
{
    public class Fechas_UsuarioRepository : IRepository<Fechas_Usuario, int>
    {
        private readonly AppDbContext _context;

        public Fechas_UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Fechas_Usuario>> GetAllAsync()
        {
            return await _context.Fechas_Usuarios.ToListAsync();
        }

        public async Task<Fechas_Usuario> GetByIdAsync(int id)
        {
            var result = await _context.Fechas_Usuarios.FindAsync(id);
            if (result == null)
            {
                throw new KeyNotFoundException($"No se encontró un Fechas_Usuario con el ID {id}");
            }
            return result;
        }


        public async Task AddAsync(Fechas_Usuario entity)
        {
            await _context.Fechas_Usuarios.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Fechas_Usuario entity)
        {
            _context.Fechas_Usuarios.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Fechas_Usuarios.FindAsync(id);
            if (entity != null)
            {
                _context.Fechas_Usuarios.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Usuarios?> GetUsuarioByCodigoAsync(int usuarioCodigo)
        {
            return await _context.Usuarios
                                .FirstOrDefaultAsync(u => u.Codigo == usuarioCodigo);
        }

        public bool Exists(int id)
        {
            return _context.Fechas_Usuarios.Any(e => e.Id == id);
        }
    }
}