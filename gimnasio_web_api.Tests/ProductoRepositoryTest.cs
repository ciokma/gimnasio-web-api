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
    public class ProductoRepositoyTests{
        private readonly DbContextOptions<AppDbContext> _options;
        public ProductoRepositoyTests(){
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("ProductoDb")
                .Options;
        }
        private AppDbContext CreateDbContext()
        {
            return new AppDbContext(_options);
        }
        [Fact]
        public async AddAsync_ShouldAddProduct()
        {
            var db = CreateDbContext();
            var repository = new ProductoRepositoy(db);
            var producto = new Producto{
                Descripcion = "Agua Helada",
                Precio = 25.00,
                Existencias = 15.0
            }
            await repository.AddAsync(producto);
            var result = db.Producto.SingleOrDefault(x => x.Descripcion == "Agua Helada");
            Assert.NotNull(result);
        }
        [Facy]
        public async Task AddAsync_ShouldGetProduct()
        {
            var db = CreateDbContext();
            var repository = new ProductoRepositoy(db);
            var producto = new Producto{
                Descripcion = "Suero de Sabor",
                Precio = 15.00,
                Existencias = 42.0
            }
            await db.Producto.AddAsync(producto);
            await db.SaveChangesAsync();

            var result = await repository.GetByIdAsync(producto.CodigoProducto);
            Assert.NotNull(result);
            Assert.Equal("Suero de Sabor", result.Descripcion);
        }
        [Facy]
        public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
        {
            var db = CreateDbContext();
            var repository = new ProductoRepository(db);
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => repository.GetByIdAsync(999)
            );
        }
        [Fact]
        public async Task GetAllAsync_ShouldGetAllAsync()
        {
            var db = CreateDbContext();
            var repository = new ProductoRepository(db);
            var producto1 = new Producto{
                Descripcion = "Refresco Natural",
                Precio = 20.00,
                Existencias = 18.0
            }
            var producto2 = new Producto{
                Descripcion = "Tiras Elasticas",
                Precio = 24.00,
                Existencias = 38.0
            }
            await db.Producto.AddRangeAsync(producto1, producto2);
            await db.SaveChangesAsync();
            var result = await repository.GetAllAsync();
            Assert.NotNull(result);
            Assert.True(result.Count() >= 2);
        }
        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct(){
            var db = CreateDbContext();
            var repository = new ProductoRepository(db);
            var producto = new Producto{
                Descripcion = "Ensa",
                Precio = 15.00,
                Existencias = 16.0
            }
            await db.Producto.AddAsync(producto);
            await db.SaveChangesAsync();
            producto.Existencias = 14.0;
            await repository.UpdateAsync(producto);
            var result = db.Producto.Find(producto.CodigoProducto);
            Assert.NotNull(result);
            Assert.Equal(14.0, result.Existencias);
        }
        [Fact]
        public async Task DeleteAsync_ShouldDeleteProduct()
        {
            var db = CreateDbContext();
            var repository = new ProductoRepository(db);
            var producto = new Producto{
                Descripcion = "Colchoneta",
                Precio = 300.00,
                Existencias = 12.0
            }
            await db.Producto.AddAsync(producto);
            await db.SaveChangesAsync();
            await repository.DeleteAsync(producto.CodigoProducto);
            var result = db.Producto.Find(producto.CodigoProducto);
            Assert.Null(result);
        }
    }
}