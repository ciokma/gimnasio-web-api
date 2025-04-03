using gimnasio_web_api.Data;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace gimnasio_web_api.Repositories
{
    public class AdministradorRepository : IAdministradorRepository
    {
        private readonly AppDbContext _context;

        public AdministradorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Administrador>> GetAllAsync()
        {
            return await _context.Administradores.ToListAsync();
        }

        public async Task<Administrador> GetByIdAsync(int id)
        {
            var administrador = await _context.Administradores.FindAsync(id);
            if (administrador == null)
                throw new KeyNotFoundException($"No se encontró un Administrador con ID {id}");

            return administrador;
        }

        public async Task<Administrador?> GetByUsernameAsync(string username)
        {
            return await _context.Administradores.FirstOrDefaultAsync(a => a.Usuario == username);
        }

        public async Task AddAsync(Administrador administrador)
        {
            await _context.Administradores.AddAsync(administrador);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Administrador administrador)
        {
            _context.Administradores.Update(administrador);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var administrador = await GetByIdAsync(id);
            if (administrador != null)
            {
                _context.Administradores.Remove(administrador);
                await _context.SaveChangesAsync();
            }
        }

  
    }
}
