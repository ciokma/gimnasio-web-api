using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;

namespace gimnasio_web_api.Tests
{
    public class VentaRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public VentaRepositoryTests()
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
        public async Task AddVentaAsync_ShouldAddVenta()
        {
            var db = CreateDbContext();
            var repository = new VentaRepository(db);

            var venta = new Venta
            {
                Fecha_venta = DateTime.UtcNow,
                Nombre_vendedor = "Juan Pérez",
                CodigoProducto = 1,
                Total = 500.00M
            };

            await repository.AddVentaAsync(venta);

            var result = db.Venta.SingleOrDefault(v => v.Nombre_vendedor == "Juan Pérez");

            Assert.NotNull(result);
            Assert.Equal(500.00M, result.Total);
        }

        [Fact]
        public async Task GetVentaPorIdAsync_ShouldReturnVenta()
        {
            var db = CreateDbContext();
            var repository = new VentaRepository(db);

            var venta = new Venta
            {
                Fecha_venta = DateTime.UtcNow,
                Nombre_vendedor = "Ana López",
                CodigoProducto = 2,
                Total = 300.00M
            };

            await db.Venta.AddAsync(venta);
            await db.SaveChangesAsync();

            var result = await repository.GetVentaPorIdAsync(venta.Codigo_venta);

            Assert.NotNull(result);
            Assert.Equal("Ana López", result.Nombre_vendedor);
        }

        [Fact]
        public async Task GetFechasConVentasAsync_ShouldReturnFechas()
        {
            var db = CreateDbContext();
            var repository = new VentaRepository(db);

            var fechaVenta = DateTime.UtcNow.Date;

            await db.Venta.AddRangeAsync(
                new Venta { Fecha_venta = fechaVenta, Nombre_vendedor = "Carlos", CodigoProducto = 1, Total = 200M },
                new Venta { Fecha_venta = fechaVenta.AddDays(-1), Nombre_vendedor = "Luisa", CodigoProducto = 2, Total = 150M }
            );
            await db.SaveChangesAsync();

            var fechas = await repository.GetFechasConVentasAsync();

            Assert.Contains(fechaVenta, fechas);
            Assert.Contains(fechaVenta.AddDays(-1), fechas);
        }

        [Fact]
        public async Task GetVentasPorRangoFechasAsync_ShouldReturnCorrectVentas()
        {
            var db = CreateDbContext();
            var repository = new VentaRepository(db);

            var fechaInicio = DateTime.UtcNow.Date;
            var fechaFin = fechaInicio.AddDays(2);

            await db.Venta.AddRangeAsync(
                new Venta { Fecha_venta = fechaInicio, Nombre_vendedor = "Pedro", CodigoProducto = 1, Total = 300M },
                new Venta { Fecha_venta = fechaFin, Nombre_vendedor = "Sofia", CodigoProducto = 2, Total = 500M }
            );
            await db.SaveChangesAsync();

            var ventas = await repository.GetVentasPorRangoFechasAsync(fechaInicio, fechaFin);

            Assert.Equal(2, ventas.Count());
        }

        [Fact]
        public async Task DeleteVentaAsync_ShouldRemoveVenta()
        {
            var db = CreateDbContext();
            var repository = new VentaRepository(db);

            var venta = new Venta
            {
                Fecha_venta = DateTime.UtcNow,
                Nombre_vendedor = "Roberto",
                CodigoProducto = 1,
                Total = 450M
            };

            await db.Venta.AddAsync(venta);
            await db.SaveChangesAsync();

            await repository.DeleteVentaAsync(venta.Codigo_venta);

            var result = await db.Venta.FindAsync(venta.Codigo_venta);

            Assert.Null(result);
        }
    }
}