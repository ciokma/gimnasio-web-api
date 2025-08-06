using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BackupController : ControllerBase
    {
        private readonly IBackupRepository _backupRepository;

        public BackupController(IBackupRepository backupRepository)
        {
            _backupRepository = backupRepository;
        }

        // GET: api/Backup
        [HttpGet]
        public async Task<ActionResult<Backup>> GetBackupConfig()
        {
            var backup = await _backupRepository.GetBackupConfigAsync();

            if (backup == null)
            {
                return NotFound(new { message = "No hay configuración de respaldo.", data = (Backup?)null });
            }

            return Ok(backup);
        }

        // POST: api/Backup
        [HttpPost]
        public async Task<ActionResult<Backup>> CreateBackupConfig(Backup backup)
        {
            var existingBackup = await _backupRepository.GetBackupConfigAsync();

            if (existingBackup != null)
            {
                return Conflict("Ya existe una configuración de respaldo.");
            }

            await _backupRepository.AddBackupConfigAsync(backup);

            return CreatedAtAction(nameof(GetBackupConfig), backup);
        }

        // PUT: api/Backup
        [HttpPut]
        public async Task<IActionResult> UpdateBackupConfig(Backup backup)
        {
            var existingBackup = await _backupRepository.GetBackupConfigAsync();

            if (existingBackup == null)
            {
                return NotFound("No se encontró configuración de respaldo.");
            }

            await _backupRepository.UpdateBackupConfigAsync(backup);

            return NoContent();
        }
    }
}