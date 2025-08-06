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
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
                Texto = "Primer Mensaje",
                Emisor = "USUARIO",
                Leido = false
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
                Texto = "Segundo Mensaje",
                Emisor = "USUARIO",
                Leido = false
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

            await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.GetByIdAsync(999));
        }

        [Fact]
        public async Task GetByEmisorSistemaCrossfitAsync_ShouldReturnOnlySistemaCrossfitMessages()
        {
            var db = CreateDbContext();
            var repository = new MensajeRepository(db);

            await db.Mensaje.AddRangeAsync(
                new Mensaje { Texto = "Mensaje 1", Emisor = "SISTEMA-CROSSFIT", Leido = false },
                new Mensaje { Texto = "Mensaje 2", Emisor = "USUARIO", Leido = false }
            );
            await db.SaveChangesAsync();

            var result = await repository.GetByEmisorSistemaCrossfitAsync();

            Assert.Single(result);
            Assert.All(result, m => Assert.Equal("SISTEMA-CROSSFIT", m.Emisor));
        }

        [Fact]
        public async Task GetAllExceptSistemaCrossfitAsync_ShouldReturnNonSistemaCrossfitMessages()
        {
            var db = CreateDbContext();
            var repository = new MensajeRepository(db);

            await db.Mensaje.AddRangeAsync(
                new Mensaje { Texto = "Mensaje 1", Emisor = "SISTEMA-CROSSFIT", Leido = false },
                new Mensaje { Texto = "Mensaje 2", Emisor = "ENTRENADOR", Leido = false },
                new Mensaje { Texto = "Mensaje 3", Emisor = "ADMIN", Leido = false }
            );
            await db.SaveChangesAsync();

            var result = await repository.GetAllExceptSistemaCrossfitAsync();

            Assert.Equal(2, result.Count());
            Assert.All(result, m => Assert.NotEqual("SISTEMA-CROSSFIT", m.Emisor));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateMessage()
        {
            var db = CreateDbContext();
            var repository = new MensajeRepository(db);

            var mensaje = new Mensaje
            {
                Texto = "Cuarto Mensaje",
                Emisor = "USUARIO",
                Leido = false
            };

            await db.Mensaje.AddAsync(mensaje);
            await db.SaveChangesAsync();

            mensaje.Texto = "Mensaje Actualizado";
            await repository.UpdateAsync(mensaje);

            var result = db.Mensaje.Find(mensaje.Codigo);
            Assert.NotNull(result);
            Assert.Equal("Mensaje Actualizado", result.Texto);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteMessage()
        {
            var db = CreateDbContext();
            var repository = new MensajeRepository(db);

            var mensaje = new Mensaje
            {
                Texto = "Sexto Mensaje",
                Emisor = "USUARIO",
                Leido = false
            };

            await db.Mensaje.AddAsync(mensaje);
            await db.SaveChangesAsync();

            await repository.DeleteAsync(mensaje.Codigo);

            var result = db.Mensaje.Find(mensaje.Codigo);
            Assert.Null(result);
        }
        [Fact]
        public async Task GetNumberMessagesAsync_ShouldReturnCorrectCountsAndIds()
        {
            var db = CreateDbContext();
            var repository = new MensajeRepository(db);

            var mensaje1 = new Mensaje { Texto = "Mensaje 1", Emisor = "SISTEMA-CROSSFIT", Leido = false };
            var mensaje2 = new Mensaje { Texto = "Mensaje 2", Emisor = "SISTEMA", Leido = false };
            var mensaje3 = new Mensaje { Texto = "Mensaje 3", Emisor = "ENTRENADOR", Leido = false };
            var mensaje4 = new Mensaje { Texto = "Mensaje 4", Emisor = "USUARIO", Leido = false };

            await db.Mensaje.AddRangeAsync(mensaje1, mensaje2, mensaje3, mensaje4);
            await db.SaveChangesAsync();
            var (sistemaCount, sistemaIds, usuarioCount, usuarioIds) = await repository.GetNumberMessagesAsync();

            Assert.Equal(2, sistemaCount);
            Assert.Equal(2, usuarioCount);

            Assert.Contains(mensaje1.Codigo, sistemaIds);
            Assert.Contains(mensaje2.Codigo, sistemaIds);

            Assert.Contains(mensaje3.Codigo, usuarioIds);
            Assert.Contains(mensaje4.Codigo, usuarioIds);
        }
        [Fact]
        public async Task MarkMessagesAsReadAsync_ShouldMarkMessagesAsRead()
        {
            var db = CreateDbContext();
            var repository = new MensajeRepository(db);

            var mensaje1 = new Mensaje { Texto = "Mensaje 1", Emisor = "SISTEMA-CROSSFIT", Leido = false };
            var mensaje2 = new Mensaje { Texto = "Mensaje 2", Emisor = "USUARIO", Leido = false };

            await db.Mensaje.AddRangeAsync(mensaje1, mensaje2);
            await db.SaveChangesAsync();

            await repository.MarkMessagesAsReadAsync(new List<int> { mensaje1.Codigo, mensaje2.Codigo });

            var updatedMensaje1 = await repository.GetByIdAsync(mensaje1.Codigo);
            var updatedMensaje2 = await repository.GetByIdAsync(mensaje2.Codigo);

            Assert.True(updatedMensaje1.Leido);
            Assert.True(updatedMensaje2.Leido);
        }
    }
}