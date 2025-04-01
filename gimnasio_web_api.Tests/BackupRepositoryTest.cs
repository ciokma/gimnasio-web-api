using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;

namespace gimnasio_web_api.Tests
{
    public class BackupRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public BackupRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private AppDbContext CreateDbContext()
        {
            return new AppDbContext(_options);
        }

        [Fact]
        public async Task GetBackupConfigAsync_ShouldReturnNull_WhenNoBackupExists()
        {
            var db = CreateDbContext();
            var repository = new BackupRepository(db);

            var result = await repository.GetBackupConfigAsync();

            Assert.Null(result);
        }

        [Fact]
        public async Task AddBackupConfigAsync_ShouldCreateBackup()
        {
            var db = CreateDbContext();
            var repository = new BackupRepository(db);

            var backup = new Backup
            {
                FechaRespaldo = DateTime.UtcNow,
                FrecuenciaRespaldo = "Diario",
                FechaRespaldoAnterior = null
            };

            await repository.AddBackupConfigAsync(backup);

            var result = await db.Backup.FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.Equal("Diario", result.FrecuenciaRespaldo);
        }

        [Fact]
        public async Task UpdateBackupConfigAsync_ShouldUpdateBackup()
        {
            var db = CreateDbContext();
            var repository = new BackupRepository(db);

            var backup = new Backup
            {
                FechaRespaldo = DateTime.UtcNow,
                FrecuenciaRespaldo = "Semanal",
                FechaRespaldoAnterior = null
            };

            await db.Backup.AddAsync(backup);
            await db.SaveChangesAsync();

            backup.FrecuenciaRespaldo = "Mensual";
            await repository.UpdateBackupConfigAsync(backup);

            var updatedBackup = await db.Backup.FirstOrDefaultAsync();

            Assert.NotNull(updatedBackup);
            Assert.Equal("Mensual", updatedBackup.FrecuenciaRespaldo);
        }
    }
}