using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Data;
using System.Threading.Tasks;

namespace gimnasio_web_api.Repositories
{
    public class BackupRepository : IBackupRepository
    {
        private readonly AppDbContext _context;

        public BackupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Backup?> GetBackupConfigAsync()
        {
            return await _context.Backup.FirstOrDefaultAsync();
        }

        public async Task AddBackupConfigAsync(Backup backup)
        {
            _context.Backup.Add(backup);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBackupConfigAsync(Backup backup)
        {
            var existingBackup = await _context.Backup.FirstOrDefaultAsync();

            if (existingBackup != null)
            {
                existingBackup.ProximoRespaldo = backup.ProximoRespaldo;
                existingBackup.FrecuenciaRespaldo = backup.FrecuenciaRespaldo;
                existingBackup.FechaRespaldoAnterior = backup.FechaRespaldoAnterior;

                _context.Entry(existingBackup).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}