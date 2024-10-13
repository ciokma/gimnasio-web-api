using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using gimnasio_web_api.Data;

namespace gimnasio_web_api.Tests
{
    public class FechasUsuarioRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public FechasUsuarioRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("FechasUsuarioDb_" + Guid.NewGuid())
                .Options;
        }

        private AppDbContext CreateDbContext()
        {
            return new AppDbContext(_options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddFechaUsuario()
        {
            var db = CreateDbContext();
            var repository = new Fechas_UsuarioRepository(db);

            var fechaUsuario = new Fechas_Usuario
            {
                UsuarioId = 1,
                FechaPago = DateTime.Now,
                FechaPagoA = DateTime.Now.AddDays(30),
                FechaVencimiento = DateTime.Now.AddDays(60)
            };

            await repository.AddAsync(fechaUsuario);
            await db.SaveChangesAsync();

            var result = await db.Fechas_Usuarios.FindAsync(fechaUsuario.Id);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldGetFechaUsuario()
        {
            var db = CreateDbContext();
            var repository = new Fechas_UsuarioRepository(db);

            var fechaUsuario = new Fechas_Usuario
            {
                UsuarioId = 1,
                FechaPago = DateTime.Now,
                FechaPagoA = DateTime.Now.AddDays(30),
                FechaVencimiento = DateTime.Now.AddDays(60)
            };

            await db.Fechas_Usuarios.AddAsync(fechaUsuario);
            await db.SaveChangesAsync();

            var result = await repository.GetByIdAsync(fechaUsuario.Id);

            Assert.NotNull(result);
            Assert.Equal(fechaUsuario.UsuarioId, result.UsuarioId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
        {
            var db = CreateDbContext();
            var repository = new Fechas_UsuarioRepository(db);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.GetByIdAsync(999));
        }

        [Fact]
        public async Task GetAllAsync_ShouldGetAllFechasUsuarios()
        {
            var db = CreateDbContext();
            var repository = new Fechas_UsuarioRepository(db);

            var fecha1 = new Fechas_Usuario
            {
                UsuarioId = 1,
                FechaPago = DateTime.Now,
                FechaPagoA = DateTime.Now.AddDays(30),
                FechaVencimiento = DateTime.Now.AddDays(60)
            };

            var fecha2 = new Fechas_Usuario
            {
                UsuarioId = 2,
                FechaPago = DateTime.Now,
                FechaPagoA = DateTime.Now.AddDays(30),
                FechaVencimiento = DateTime.Now.AddDays(60)
            };

            await db.Fechas_Usuarios.AddRangeAsync(fecha1, fecha2);
            await db.SaveChangesAsync();

            var allFechasInDb = await db.Fechas_Usuarios.ToListAsync();
            Assert.Equal(2, allFechasInDb.Count);

            var result = await repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.True(result.Count() >= 2);
        }


        [Fact]
        public async Task UpdateAsync_ShouldUpdateFechaUsuario()
        {
            var db = CreateDbContext();
            var repository = new Fechas_UsuarioRepository(db);

            var fechaUsuario = new Fechas_Usuario
            {
                UsuarioId = 1,
                FechaPago = DateTime.Now,
                FechaPagoA = DateTime.Now.AddDays(30),
                FechaVencimiento = DateTime.Now.AddDays(60)
            };

            await db.Fechas_Usuarios.AddAsync(fechaUsuario);
            await db.SaveChangesAsync();

            fechaUsuario.FechaVencimiento = DateTime.Now.AddDays(90);
            await repository.UpdateAsync(fechaUsuario);
            await db.SaveChangesAsync();

            var result = await db.Fechas_Usuarios.FindAsync(fechaUsuario.Id);
            Assert.NotNull(result);
            Assert.Equal(DateTime.Now.AddDays(90).Date, result.FechaVencimiento?.Date);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteFechaUsuario()
        {
            var db = CreateDbContext();
            var repository = new Fechas_UsuarioRepository(db);

            var fechaUsuario = new Fechas_Usuario
            {
                UsuarioId = 1,
                FechaPago = DateTime.Now,
                FechaPagoA = DateTime.Now.AddDays(30),
                FechaVencimiento = DateTime.Now.AddDays(60)
            };

            await db.Fechas_Usuarios.AddAsync(fechaUsuario);
            await db.SaveChangesAsync();

            await repository.DeleteAsync(fechaUsuario.Id);
            await db.SaveChangesAsync();

            var result = await db.Fechas_Usuarios.FindAsync(fechaUsuario.Id);
            Assert.Null(result);
        }
    }
}