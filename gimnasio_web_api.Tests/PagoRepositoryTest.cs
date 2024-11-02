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
    public class PagoRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public PagoRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("PagoDb")
                .Options;
        }

        private AppDbContext CreateDbContext()
        {
            return new AppDbContext(_options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddPago()
        {
            var db = CreateDbContext();
            var repository = new PagoRepository(db);

            var pago = new Pago
            {
                CodigoUsuario = 1,
                MesesPagados = 2,
                MesesPagadosA = 3,
                FechaPago = DateTime.Now,
                Monto = 100.50m,
                DetallePago = "Pago de prueba"
            };

            await repository.AddAsync(pago);

            var result = db.Pagos.SingleOrDefault(x => x.DetallePago == "Pago de prueba");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldGetPago()
        {
            var db = CreateDbContext();
            var repository = new PagoRepository(db);

            var pago = new Pago
            {
                CodigoUsuario = 1,
                MesesPagados = 2,
                MesesPagadosA = 3,
                FechaPago = DateTime.Now,
                Monto = 100.50m,
                DetallePago = "Pago de prueba"
            };

            await db.Pagos.AddAsync(pago);
            await db.SaveChangesAsync();

            var result = await repository.GetByIdAsync(pago.CodigoPago);

            Assert.NotNull(result);
            Assert.Equal("Pago de prueba", result.DetallePago);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
        {
            var db = CreateDbContext();
            var repository = new PagoRepository(db);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => repository.GetByIdAsync(999)
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldGetAllPagos()
        {
            var db = CreateDbContext();
            var repository = new PagoRepository(db);

            var pago1 = new Pago
            {
                CodigoUsuario = 1,
                MesesPagados = 2,
                MesesPagadosA = 3,
                FechaPago = DateTime.Now,
                Monto = 100.50m,
                DetallePago = "Pago 1"
            };
            var pago2 = new Pago
            {
                CodigoUsuario = 2,
                MesesPagados = 1,
                MesesPagadosA = 2,
                FechaPago = DateTime.Now,
                Monto = 200.00m,
                DetallePago = "Pago 2"
            };

            await db.Pagos.AddRangeAsync(pago1, pago2);
            await db.SaveChangesAsync();

            var result = await repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.True(result.Count() >= 2);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePago()
        {
            var db = CreateDbContext();
            var repository = new PagoRepository(db);

            var pago = new Pago
            {
                CodigoUsuario = 1,
                MesesPagados = 2,
                MesesPagadosA = 3,
                FechaPago = DateTime.Now,
                Monto = 100.50m,
                DetallePago = "Pago de prueba"
            };

            await db.Pagos.AddAsync(pago);
            await db.SaveChangesAsync();

            pago.DetallePago = "Pago actualizado";

            await repository.UpdateAsync(pago);

            var result = db.Pagos.Find(pago.CodigoPago);

            Assert.NotNull(result);
            Assert.Equal("Pago actualizado", result.DetallePago);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeletePago()
        {
            var db = CreateDbContext();
            var repository = new PagoRepository(db);

            var pago = new Pago
            {
                CodigoUsuario = 1,
                MesesPagados = 2,
                MesesPagadosA = 3,
                FechaPago = DateTime.Now,
                Monto = 100.50m,
                DetallePago = "Pago de prueba"
            };

            await db.Pagos.AddAsync(pago);
            await db.SaveChangesAsync();

            await repository.DeleteAsync(pago.CodigoPago);

            var result = db.Pagos.Find(pago.CodigoPago);

            Assert.Null(result);
        }
    }
}