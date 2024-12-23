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
    public class MensajeRepositoryTest
    {
        private readonly DbContextOptions<AppDbContext> _options;
        public MensajeRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("MensajeDb")
                .Options;
        }
        private AppDbContext CreateDbContext()
        {
            return new AppDbContext(_options);
        }
        [Fact]
        public async Task AddAsync_ShouldAddMessage()
        {
            var db = CreateDbContext();
            var repository = new MensajeRepository(db);
            var mensaje = new Mensaje
            {
                Texto = "Primer Mensaje"
            };
            await repository.AddAsync(mensaje);
            var result = db.Mensaje.SingleOrDefault(x => x.Texto == "Primer Mensaje");
            Assert.NotNull(result);
        }
        [Fact]
        public async Task AddAsync_ShouldGetMessage()
        {
            var db = CreateDbContext();
            var repository = new MensajeRepository(db);
            var mensaje = new Mensaje
            {
                Texto = "Segundo Mensaje"
            };
            await db.Mensaje.AddAsync(mensaje);
            await db.SaveChangesAsync();
            var result = await repository.GetByIdAsync(mensaje.Codigo);
            Assert.NotNull(result);
            Assert.Equal("Segundo Mensaje", result.Texto);
        }
        [Fact]
        public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
        {
            var db = CreateDbContext();
            var repository = new MensajeRepository(db);
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => repository.GetByIdAsync(999)
            );
        }
        [Fact]
        public async Task GetAllAsync_ShouldGetAllAsync()
        {
            var db = CreateDbContext();
            var repository = new MensajeRepository(db);
            var mensaje1 = new Mensaje
            {
                Texto = "Tercer Mensaje"
            };
            var mensaje2 = new Mensaje
            {
                Texto = "Cuarto Mensaje"
            };
            var mensaje3 = new Mensaje 
            {
                Texto = "Quinto Mensaje"
            };
            await db.Mensaje.AddRangeAsync(mensaje1, mensaje2, mensaje3);
            await db.SaveChangesAsync();
            var result = await repository.GetAllAsync();
            Assert.NotNull(result);
            Assert.True(result.Count() >= 3);
        }
        [Fact]
        public async Task UpdateAsync_ShouldUpdateMessage()
        {
            var db = CreateDbContext();
            var repository = new MensajeRepository(db);
            var mensaje = new Mensaje
            {
                Texto = "Cuarto Mensaje"
            };
            await db.Mensaje.AddAsync(mensaje);
            await db.SaveChangesAsync();
            mensaje.Texto = "Quinto Mensaje";
            await repository.UpdateAsync(mensaje);
            var result = db.Mensaje.Find(mensaje.Codigo);
            Assert.NotNull(result);
            Assert.Equal("Quinto Mensaje", result.Texto);
        }
        [Fact]
        public async Task DeleteAsync_ShouldDeleteMessage()
        {
            var db = CreateDbContext();
            var repository = new MensajeRepository(db);
            var mensaje = new Mensaje
            {
                Texto = "Sexto Mensaje"
            };
            await db.Mensaje.AddAsync(mensaje);
            await db.SaveChangesAsync();
            await repository.DeleteAsync(mensaje.Codigo);
            var result = db.Mensaje.Find(mensaje.Codigo);
            Assert.Null(result);
        }
    }
}