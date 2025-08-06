using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace gimnasio_web_api.Tests
{
    public class ConfigSistemaRepositoryTest
    {
        private DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        private AppDbContext CreateDbContext()
        {
            var options = CreateNewContextOptions();
            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddConfig()
        {
            var db = CreateDbContext();
            var repository = new ConfiguracionesSistemaRepository(db);
            var config = new ConfiguracionesSistema
            {
                Nombre = "Configuración de Prueba",
                Valor = 1,
                Descripcion = "Prueba de configuración",
                Estado = true
            };
            await repository.AddAsync(config);
            var result = db.ConfiguracionesSistema.SingleOrDefault(x => x.Nombre == "Configuración de Prueba");
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldGetConfig()
        {
            var db = CreateDbContext();
            var repository = new ConfiguracionesSistemaRepository(db);
            var config = new ConfiguracionesSistema
            {
                Nombre = "Configuración de Prueba",
                Valor = 2,
                Descripcion = "Prueba de configuración",
                Estado = false
            };
            await db.ConfiguracionesSistema.AddAsync(config);
            await db.SaveChangesAsync();
            var result = await repository.GetByIdAsync(config.Id);
            Assert.NotNull(result);
            Assert.Equal("Configuración de Prueba", result.Nombre);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateConfig()
        {
            var db = CreateDbContext();
            var repository = new ConfiguracionesSistemaRepository(db);
            var config = new ConfiguracionesSistema
            {
                Nombre = "Configuración de Prueba",
                Valor = 3,
                Descripcion = "Prueba de configuración",
                Estado = true
            };
            await db.ConfiguracionesSistema.AddAsync(config);
            await db.SaveChangesAsync();

            config.Valor = 10;
            await repository.UpdateAsync(config);

            var updatedConfig = await repository.GetByIdAsync(config.Id);
            Assert.Equal(10, updatedConfig.Valor);
        }

        [Fact]
        public async Task GetAllAsync_ShouldGetAllConfigs()
        {
            var db = CreateDbContext();
            var repository = new ConfiguracionesSistemaRepository(db);
            var config1 = new ConfiguracionesSistema
            {
                Nombre = "Configuración 1",
                Valor = 4,
                Descripcion = "Primera configuración",
                Estado = false
            };
            var config2 = new ConfiguracionesSistema
            {
                Nombre = "Configuración 2",
                Valor = 5,
                Descripcion = "Segunda configuración",
                Estado = true
            };
            await db.ConfiguracionesSistema.AddRangeAsync(config1, config2);
            await db.SaveChangesAsync();

            var result = await repository.GetAllAsync();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteConfig()
        {
            var db = CreateDbContext();
            var repository = new ConfiguracionesSistemaRepository(db);
            var config = new ConfiguracionesSistema
            {
                Nombre = "Configuración a Eliminar",
                Valor = 6,
                Descripcion = "Configuración para eliminar",
                Estado = true
            };
            await db.ConfiguracionesSistema.AddAsync(config);
            await db.SaveChangesAsync();

            await repository.DeleteAsync(config.Id);
            var deletedConfig = await db.ConfiguracionesSistema.FindAsync(config.Id);
            Assert.Null(deletedConfig);
        }
    }
}