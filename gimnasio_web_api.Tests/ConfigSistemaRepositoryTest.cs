using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace gimnasio_web_api.Tests
{
    private readonly DbContextOptions<AppDbContext> _options;
    public ConfigSistemaRepositoryTest()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("ConfigSistemaDb")
            .Options;
    }
    private AppDbContext CreateDbContext()
    {
        return new AppDbContext(_options);
    }
    [Fact]
    public async Task AddAsync_ShouldAddConfig()
    {
        var db = CreateDbContext();
        var repository = new ConfigSistemaRepository(db);
        var config = new ConfigSistema
        {
            Nombre = "Configuración de Prueba",
            Valor = "Valor de Prueba"
        };
        await repository.AddAsync(config);
        var result = db.ConfigSistema.SingleOrDefault(x => x.Nombre == "Configuración de Prueba");
        Assert.NotNull(result);
    }
    [Fact]
    public async Task GetByIdAsync_ShouldGetConfig()
    {
        var db = CreateDbContext();
        var repository = new ConfigSistemaRepository(db);
        var config = new ConfigSistema
        {
            Nombre = "Configuración de Prueba",
            Valor = "Valor de Prueba"
        };
        await db.ConfigSistema.AddAsync(config);
        await db.SaveChangesAsync();
        var result = await repository.GetByIdAsync(config.Id);
        Assert.NotNull(result);
        Assert.Equal("Configuración de Prueba", result.Nombre);
    }
    [Fact]
    public async Task UpdateAsync_ShouldUpdateConfig()
    {
        var db = CreateDbContext();
        var repository = new ConfigSistemaRepository(db);
        var config = new ConfigSistema
        {
            Nombre = "Configuración de Prueba",
            Valor = "Valor de Prueba"
        };
        await db.ConfigSistema.AddAsync(config);
        await db.SaveChangesAsync();

        config.Valor = "Nuevo Valor de Prueba";
        await repository.UpdateAsync(config);

        var updatedConfig = await repository.GetByIdAsync(config.Id);
        Assert.Equal("Nuevo Valor de Prueba", updatedConfig.Valor);
    }
    [Fact]
    public async Task GetAllAsync_ShouldGetAllConfigs()
    {
        var db = CreateDbContext();
        var repository = new ConfigSistemaRepository(db);
        var config1 = new ConfigSistema
        {
            Nombre = "Configuración 1",
            Valor = "Valor 1"
        };
        var config2 = new ConfigSistema
        {
            Nombre = "Configuración 2",
            Valor = "Valor 2"
        };
        await db.ConfigSistema.AddRangeAsync(config1, config2);
        await db.SaveChangesAsync();

        var result = await repository.GetAllAsync();
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task DeleteAsync_ShouldDeleteConfig()
    {
        var db = CreateDbContext();
        var repository = new ConfigSistemaRepository(db);
        var config = new ConfigSistema
        {
            Nombre = "Configuración a Eliminar",
            Valor = "Valor a Eliminar"
        };
        await db.ConfigSistema.AddAsync(config);
        await db.SaveChangesAsync();

        await repository.DeleteAsync(config.Id);
        var deletedConfig = await db.ConfigSistema.FindAsync(config.Id);
        Assert.Null(deletedConfig);
    }
}