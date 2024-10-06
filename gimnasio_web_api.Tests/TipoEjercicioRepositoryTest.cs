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
    public class Tipo_EjercicioRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public Tipo_EjercicioRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TipoEjercicioDb")
                .Options;
        }

        private AppDbContext CreateDbContext()
        {
            return new AppDbContext(_options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTipoEjercicio()
        {
            var db = CreateDbContext();
            var repository = new Tipo_EjercicioRepository(db);
            var tipoEjercicio = new Tipo_Ejercicio
            {
                Descripcion = "Yoga",
                Costo = 20,
                Activo = true
            };
            await repository.AddAsync(tipoEjercicio);
            var result = db.Tipo_Ejercicio.SingleOrDefault(x => x.Descripcion == "Yoga");
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldGetTipoEjercicio()
        {
            var db = CreateDbContext();
            var repository = new Tipo_EjercicioRepository(db);
            var tipoEjercicio = new Tipo_Ejercicio
            {
                Descripcion = "Pilates",
                Costo = 25,
                Activo = true
            };
            await db.Tipo_Ejercicio.AddAsync(tipoEjercicio);
            await db.SaveChangesAsync();

            var result = await repository.GetByIdAsync(tipoEjercicio.Codigo);
            Assert.NotNull(result);
            Assert.Equal("Pilates", result.Descripcion);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
        {
            var db = CreateDbContext();
            var repository = new Tipo_EjercicioRepository(db);
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => repository.GetByIdAsync(999)
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldGetAllAsync()
        {
            var db = CreateDbContext();
            var repository = new Tipo_EjercicioRepository(db);
            var tipoEjercicio1 = new Tipo_Ejercicio
            {
                Descripcion = "Zumba",
                Costo = 30,
                Activo = true
            };
            var tipoEjercicio2 = new Tipo_Ejercicio
            {
                Descripcion = "Crossfit",
                Costo = 35,
                Activo = true
            };
            await db.Tipo_Ejercicio.AddRangeAsync(tipoEjercicio1, tipoEjercicio2);
            await db.SaveChangesAsync();
            var result = await repository.GetAllAsync();
            Assert.NotNull(result);
            Assert.True(result.Count() >= 2);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTipoEjercicio()
        {
            var db = CreateDbContext();
            var repository = new Tipo_EjercicioRepository(db);
            var tipoEjercicio = new Tipo_Ejercicio
            {
                Descripcion = "Boxing",
                Costo = 40,
                Activo = true
            };
            await db.Tipo_Ejercicio.AddAsync(tipoEjercicio);
            await db.SaveChangesAsync();
            tipoEjercicio.Costo = 45;
            await repository.UpdateAsync(tipoEjercicio);
            var result = db.Tipo_Ejercicio.Find(tipoEjercicio.Codigo);
            Assert.NotNull(result);
            Assert.Equal(45, result.Costo);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteTipoEjercicio()
        {
            var db = CreateDbContext();
            var repository = new Tipo_EjercicioRepository(db);
            var tipoEjercicio = new Tipo_Ejercicio
            {
                Descripcion = "Stretching",
                Costo = 15,
                Activo = true
            };
            await db.Tipo_Ejercicio.AddAsync(tipoEjercicio);
            await db.SaveChangesAsync();
            await repository.DeleteAsync(tipoEjercicio.Codigo);
            var result = db.Tipo_Ejercicio.Find(tipoEjercicio.Codigo);
            Assert.Null(result);
        }
    }
}
