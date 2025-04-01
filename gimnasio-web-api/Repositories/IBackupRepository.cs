using gimnasio_web_api.Models;
using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Data;
using System.Threading.Tasks;

namespace gimnasio_web_api.Repositories
{
    public interface IBackupRepository
    {
        Task<Backup?> GetBackupConfigAsync();
        Task AddBackupConfigAsync(Backup backup);
        Task UpdateBackupConfigAsync(Backup backup);
    }
}