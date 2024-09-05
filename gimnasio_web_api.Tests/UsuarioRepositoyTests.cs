using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gimnasio_web_api.Tests
{
    public class UsuarioRepositoyTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public UsuarioRepositoyTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("UsuarioDb")
                .Options;
        }

        //private AppDbContext CreateDbContext() => new AppDbContext(_options);
        private AppDbContext CreateDbContext()
        {
            return new AppDbContext(_options);
        }
        /// <summary>
        /// Prueba unitaria para validar que el usuario se pueda crear
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            //db context
            var db = CreateDbContext();
            //user repository
            var repository = new UsuarioRepository(db);

            //user
            var usuario = new Usuarios
            {
                Nombres = "Demo - Nombre 1",
                Apellidos = "Demo - Apellido",
                Telefono = "11112222",
                Activo = true,
                FechaIngreso = DateTime.Now,
                Observaciones = "Observaciones demo"
            };
            //execute
            await repository.AddAsync(usuario);

            //result
            var result = db.Usuarios.SingleOrDefault(x => x.Nombres == "Demo - Nombre 1");

            //assert
            Assert.NotNull(result);

        }

        [Fact]
        public async Task AddAsync_ShouldGetUser()
        {
            //db context
            var db = CreateDbContext();
            //user repository
            var repository = new UsuarioRepository(db);

            //user
            var usuario = new Usuarios
            {
                Nombres = "Demo - Nombre",
                Apellidos = "Demo - Apellido",
                Telefono = "11112222",
                Activo = true,
                FechaIngreso = DateTime.Now,
                Observaciones = "Observaciones demo"
            };
            //execute
            await db.Usuarios.AddAsync(usuario);
            await db.SaveChangesAsync();

            //result
            var result = await repository.GetByIdAsync(usuario.Codigo);

            //assert
            Assert.NotNull(result);
            Assert.Equal("Demo - Nombre", result.Nombres);
        }
        [Fact]
        public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
        {
            var db = CreateDbContext();
            var repository = new UsuarioRepository(db);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => repository.GetByIdAsync(999)
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldGetAllAsync()
        {
            //db context
            var db = CreateDbContext();
            //user repository
            var repository = new UsuarioRepository(db);

            //user
            var usuario1 = new Usuarios
            {
                Nombres = "Demo - Nombre 1 1",
                Apellidos = "Demo - Apellido 1 1",
                Telefono = "11112222",
                Activo = true,
                FechaIngreso = DateTime.Now,
                Observaciones = "Observaciones demo 1 1"
            };
            var usuario2 = new Usuarios
            {
                Nombres = "Demo - Nombre 2 2",
                Apellidos = "Demo - Apellido 2 2",
                Telefono = "11112222",
                Activo = true,
                FechaIngreso = DateTime.Now,
                Observaciones = "Observaciones demo 2 2"
            };
            //execute
            await db.Usuarios.AddRangeAsync(usuario1, usuario2);
            await db.SaveChangesAsync();

            //result
            var result = await repository.GetAllAsync();

            //assert
            Assert.NotNull(result);
            Assert.True(result.Count() >= 2);
        }
        [Fact]
        public async Task UpdateAsync_ShouldUpdateUsuario()
        {
            //db context
            var db = CreateDbContext();
            //user repository
            var repository = new UsuarioRepository(db);

            //user
            var usuario = new Usuarios
            {
                Nombres = "Demo - Nombre Updated",
                Apellidos = "Demo - Apellido Updated",
                Telefono = "11112222",
                Activo = true,
                FechaIngreso = DateTime.Now,
                Observaciones = "Observaciones demo Updated"
            };

            await db.Usuarios.AddAsync(usuario);
            await db.SaveChangesAsync();

            usuario.Observaciones = "Nuevas observaciones";

            await repository.UpdateAsync(usuario);

            var result = db.Usuarios.Find(usuario.Codigo);

            Assert.NotNull(result);
            Assert.Equal("Nuevas observaciones", result.Observaciones);
        }
        [Fact]
        public async Task DeleteAsync_ShouldDeleteUsuario()
        {
            //db context
            var db = CreateDbContext();
            //user repository
            var repository = new UsuarioRepository(db);

            //user
            var usuario = new Usuarios
            {
                Nombres = "Demo - Nombre Deleted",
                Apellidos = "Demo - Apellido Deleted",
                Telefono = "11112222",
                Activo = true,
                FechaIngreso = DateTime.Now,
                Observaciones = "Observaciones demo Deleted"
            };

            await db.Usuarios.AddAsync(usuario);
            await db.SaveChangesAsync();


            await repository.DeleteAsync(usuario.Codigo);

            var result = db.Usuarios.Find(usuario.Codigo);

            Assert.Null(result);

        }
    }
}
