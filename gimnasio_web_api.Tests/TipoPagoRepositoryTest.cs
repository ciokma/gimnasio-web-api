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
    public class Tipo_PagoRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public Tipo_PagoRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private AppDbContext CreateDbContext()
        {
            return new AppDbContext(_options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTipoPago()
        {
            var db = CreateDbContext();
            var repository = new Tipo_PagoRepository(db);
            var tipoPago = new Tipo_Pagos
            {
                CodigoPago = "01",
                Descripcion = "Pago mensual",
                Monto = 50.00M
            };
            await repository.AddAsync(tipoPago);
            var result = db.Tipo_Pagos.SingleOrDefault(x => x.CodigoPago == "01");
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldGetTipoPago()
        {
            var db = CreateDbContext();
            var repository = new Tipo_PagoRepository(db);
            var tipoPago = new Tipo_Pagos
            {
                CodigoPago = "PA",
                Descripcion = "Pago anual",
                Monto = 500.00M
            };
            await db.Tipo_Pagos.AddAsync(tipoPago);
            await db.SaveChangesAsync();

            var result = await repository.GetByIdAsync("PA");
            Assert.NotNull(result);
            Assert.Equal("Pago anual", result.Descripcion);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
        {
            var db = CreateDbContext();
            var repository = new Tipo_PagoRepository(db);
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => repository.GetByIdAsync("99")
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldGetAllAsync()
        {
            var db = CreateDbContext();
            var repository = new Tipo_PagoRepository(db);
            var tipoPago1 = new Tipo_Pagos
            {
                CodigoPago = "01",
                Descripcion = "Pago mensual",
                Monto = 50.00M
            };
            var tipoPago2 = new Tipo_Pagos
            {
                CodigoPago = "PO",
                Descripcion = "Pago Otro",
                Monto = 500.00M
            };
            await db.Tipo_Pagos.AddRangeAsync(tipoPago1, tipoPago2);
            await db.SaveChangesAsync();
            var result = await repository.GetAllAsync();
            Assert.NotNull(result);
            Assert.True(result.Count() >= 2);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTipoPago()
        {
            var db = CreateDbContext();
            var repository = new Tipo_PagoRepository(db);
            var tipoPago = new Tipo_Pagos
            {
                CodigoPago = "03",
                Descripcion = "Pago trimestral",
                Monto = 150.00M
            };
            await db.Tipo_Pagos.AddAsync(tipoPago);
            await db.SaveChangesAsync();
            tipoPago.Monto = 140.00M;
            await repository.UpdateAsync(tipoPago);
            var result = db.Tipo_Pagos.Find(tipoPago.CodigoPago);
            Assert.NotNull(result);
            Assert.Equal(140.00M, result.Monto);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteTipoPago()
        {
            var db = CreateDbContext();
            var repository = new Tipo_PagoRepository(db);
            var tipoPago = new Tipo_Pagos
            {
                CodigoPago = "04",
                Descripcion = "Pago semestral",
                Monto = 300.00M
            };
            await db.Tipo_Pagos.AddAsync(tipoPago);
            await db.SaveChangesAsync();
            await repository.DeleteAsync("04");
            var result = db.Tipo_Pagos.Find(tipoPago.CodigoPago);
            Assert.Null(result);
        }
    }
}