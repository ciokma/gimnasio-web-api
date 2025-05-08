using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Models;
namespace gimnasio_web_api.Data 
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Usuarios>()
                .HasMany(u => u.FechasUsuario)
                .WithOne(f => f.Usuario)
                .HasForeignKey(f => f.UsuarioId);

            modelBuilder.Entity<Administrador>()
            .HasIndex(p => p.FechaIngreso);

            modelBuilder.Entity<Pago>()
            .HasIndex(p => p.CodigoUsuario);
        
            modelBuilder.Entity<Pago>()
                .HasIndex(p => p.FechaPago);
            
            modelBuilder.Entity<Usuarios>()
                .HasIndex(u => new { u.Nombres, u.Apellidos })
                .HasDatabaseName("idx_nombres_apellidos");

            modelBuilder.Entity<Usuarios>()
                .HasIndex(u => new { u.Nombres, u.Apellidos, u.Foto })
                .HasDatabaseName("idx_usuarios_para_asistencia");

            modelBuilder.Entity<Fechas_Usuario>()
                .HasIndex(f => new {f.UsuarioId, f.FechaPago, f.FechaVencimiento})
                .HasDatabaseName("idx_fechas_usuario_para_asistencia");

            modelBuilder.Entity<Pago>()
                .HasIndex(p => new {p.CodigoUsuario, p.FechaPago, p.MesesPagados, p.IntervaloPago})
                .HasDatabaseName("idx_pagos_para_asistencia");
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Producto)
                .WithMany(p => p.Venta)
                .HasForeignKey(v => v.CodigoProducto)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Asistencia>()
                .HasIndex(a => a.Fecha)
                .HasDatabaseName("idx_asistencia_fecha");
        }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Fechas_Usuario> Fechas_Usuarios { get; set; }
        public DbSet<Tipo_Pagos> Tipo_Pagos { get; set; }
        public DbSet<Tipo_Ejercicio> Tipo_Ejercicio { get; set; }
        public DbSet<Producto> Producto { get; set; } = default!;
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Mensaje> Mensaje { get; set; }
        public DbSet<Venta> Venta { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<Backup> Backup { get; set; }
    }
}
