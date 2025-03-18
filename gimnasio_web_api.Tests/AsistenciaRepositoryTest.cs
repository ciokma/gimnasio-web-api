using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using gimnasio_web_api.Data;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;

namespace gimnasio_web_api.Tests
{
    public class AsistenciaRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public AsistenciaRepositoryTests()
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
        public async Task ObtenerUsuarioInfoAsync_DeberiaRetornarUsuarioSiExiste()
        {
            var db = CreateDbContext();
            var repository = new AsistenciaRepository(db);

            var usuario = new Usuarios
            {
                Codigo = 1,
                Nombres = "Juan",
                Apellidos = "Perez",
                Telefono = "123456789",
                Foto = "foto.jpg",
                FechaIngreso = DateTime.UtcNow,
                Activo = true
            };

            await db.Usuarios.AddAsync(usuario);
            await db.SaveChangesAsync();

            var resultado = await repository.ObtenerUsuarioInfoAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado?.Usuario?.Codigo);
            Assert.Equal("Juan", resultado?.Usuario?.Nombres);
        }

        [Fact]
        public async Task ObtenerUsuarioInfoAsync_DeberiaRetornarNullSiNoExiste()
        {
            var db = CreateDbContext();
            var repository = new AsistenciaRepository(db);

            var resultado = await repository.ObtenerUsuarioInfoAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObtenerUltimaInformacionPagoAsync_DeberiaRetornarUltimaFechaYUltimoPago()
        {
            var db = CreateDbContext();
            var repository = new AsistenciaRepository(db);

            var usuario = new Usuarios { Codigo = 1, Nombres = "Maria", Apellidos = "Lopez", Telefono = "987654321", Activo = true };
            await db.Usuarios.AddAsync(usuario);

            var fechaUsuario = new Fechas_Usuario
            {
                UsuarioId = 1,
                FechaPago = DateTime.UtcNow.AddDays(-10),
                FechaVencimiento = DateTime.UtcNow.AddDays(20)
            };
            await db.Fechas_Usuarios.AddAsync(fechaUsuario);

            var pago = new Pago
            {
                CodigoPago = 101,
                CodigoUsuario = 1,
                FechaPago = DateTime.UtcNow.AddDays(-5),
                MesesPagados = 1,
                Monto = 50.00M,
                DetallePago = "Pago mensual",
                IntervaloPago = true
            };
            await db.Pagos.AddAsync(pago);

            await db.SaveChangesAsync();

            var resultado = await repository.ObtenerUltimaInformacionPagoAsync(1);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.UltimaFechaUsuario);
            Assert.NotNull(resultado.UltimoPago);
            Assert.Equal(fechaUsuario.FechaPago, resultado.UltimaFechaUsuario.FechaPago);
            Assert.Equal(pago.Monto, resultado.UltimoPago.Monto);
        }

        [Fact]
        public async Task ObtenerUltimaInformacionPagoAsync_DeberiaRetornarObjetoVacioSiNoHayPagos()
        {
            var db = CreateDbContext();
            var repository = new AsistenciaRepository(db);

            var usuario = new Usuarios { Codigo = 2, Nombres = "Carlos", Apellidos = "Ramirez", Telefono = "111222333", Activo = true };
            await db.Usuarios.AddAsync(usuario);
            await db.SaveChangesAsync();

            var resultado = await repository.ObtenerUltimaInformacionPagoAsync(2);

            Assert.NotNull(resultado);
            Assert.Null(resultado.UltimaFechaUsuario);
            Assert.Null(resultado.UltimoPago);
        }
        [Fact]
        public async Task AddAsync_ShouldAddProduct()
        {
            var db = CreateDbContext();
            var repository = new AsistenciaRepository(db);
            var asistencia = new Asistencia
            {
                Codigo = 1,
                CodigoUsuario = 2,
                Fecha = DateTime.UtcNow,
                Hora = TimeSpan.FromHours(10)
            };
            await repository.AddAsync(asistencia);
            var result = db.Asistencias.SingleOrDefault(x => x.CodigoUsuario == 2);
            Assert.NotNull(result);
        }
        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct ()
        {
            var db = CreateDbContext();
            var repository = new AsistenciaRepository(db);
            var asistencia = new Asistencia
            {
                Codigo = 2,
                CodigoUsuario = 3,
                Fecha = DateTime.UtcNow,
                Hora = TimeSpan.FromHours(10)
            };
            await db.Asistencias.AddAsync(asistencia);
            await db.SaveChangesAsync();
            asistencia.CodigoUsuario = 4;
            await repository.UpdateAsync (asistencia);
            var result = db.Asistencias.Find(asistencia.Codigo);
            Assert.NotNull(result);
            Assert.Equal(4, result.CodigoUsuario);
        }
        [Fact]
        public async Task GetAsistenciasPorFechasAsync_ShouldReturnCorrectAsistencias()
        {
            var db = CreateDbContext();
            var repository = new AsistenciaRepository(db);

            var primerafecha = DateTime.UtcNow.Date;
            var segundafecha = primerafecha.AddDays(5);
            
            await db.Asistencias.AddRangeAsync(
                new Asistencia {Fecha = primerafecha, CodigoUsuario = 1, Hora = TimeSpan.FromHours(10) },
                new Asistencia {Fecha = segundafecha, CodigoUsuario = 3, Hora = TimeSpan.FromHours(15) }
            );
            await db.SaveChangesAsync();
            var asistencias = await repository.GetAsistenciaPorFechaAsync(primerafecha, segundafecha);
            Assert.Equal(2, asistencias.Count());
        }
        [Fact]
        public async Task GetAñosConAsistenciasAsync_DeberiaRetornarAñosConAsistencias()
        {
            var db = CreateDbContext();
            var repository = new AsistenciaRepository(db);

            await db.Asistencias.AddRangeAsync(
                new Asistencia { Fecha = new DateTime(2023, 5, 10), CodigoUsuario = 1 },
                new Asistencia { Fecha = new DateTime(2024, 3, 15), CodigoUsuario = 2 }
            );
            await db.SaveChangesAsync();

            var resultado = await repository.GetAñosConAsistenciasAsync();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.Contains(resultado, r => ((dynamic)r).Año == 2023);
            Assert.Contains(resultado, r => ((dynamic)r).Año == 2024);
        }

        [Fact]
        public async Task GetAñosConAsistenciasAsync_DeberiaRetornarAñosCorrectos()
        {
            var db = CreateDbContext();
            var repository = new AsistenciaRepository(db);

            await db.Asistencias.AddRangeAsync(
                new Asistencia { Fecha = new DateTime(2023, 5, 1), CodigoUsuario = 1 },
                new Asistencia { Fecha = new DateTime(2024, 6, 1), CodigoUsuario = 2 }
            );
            await db.SaveChangesAsync();

            var resultado = await repository.GetAñosConAsistenciasAsync();

            Assert.Contains(resultado, r => r.Año == 2023);
            Assert.Contains(resultado, r => r.Año == 2024);
        }

        [Fact]
        public async Task GetMesesConAsistenciasAsync_DeberiaRetornarMesesCorrectos()
        {
            var db = CreateDbContext();
            var repository = new AsistenciaRepository(db);

            await db.Asistencias.AddRangeAsync(
                new Asistencia { Fecha = new DateTime(2023, 1, 10), CodigoUsuario = 1 },
                new Asistencia { Fecha = new DateTime(2023, 2, 15), CodigoUsuario = 2 }
            );
            await db.SaveChangesAsync();

            var resultado = await repository.GetMesesConAsistenciasAsync(2023);

            Assert.Contains(resultado, r => r.Mes == 1);
            Assert.Contains(resultado, r => r.Mes == 2);
        }

        [Fact]
        public async Task GetDiasConAsistenciasAsync_DeberiaRetornarDiasCorrectos()
        {
            var db = CreateDbContext();
            var repository = new AsistenciaRepository(db);

            await db.Asistencias.AddRangeAsync(
                new Asistencia { Fecha = new DateTime(2023, 5, 10), CodigoUsuario = 1 },
                new Asistencia { Fecha = new DateTime(2023, 5, 15), CodigoUsuario = 2 }
            );
            await db.SaveChangesAsync();

            var resultado = await repository.GetDiasConAsistenciasAsync(2023, 5);

            Assert.Contains(resultado, r => r.Dia == 10);
            Assert.Contains(resultado, r => r.Dia == 15);
        }

        [Fact]
        public async Task GetDiasConAsistenciasAsync_DeberiaRetornarListaVaciaSiNoHayAsistenciasParaEseMesYAño()
        {
            var db = CreateDbContext();
            var repository = new AsistenciaRepository(db);
            int año = 2025, mes = 4;

            var resultado = await repository.GetDiasConAsistenciasAsync(año, mes);

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }
    }
}