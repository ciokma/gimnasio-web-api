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

            modelBuilder.Entity<Pago>()
            .HasIndex(p => p.CodigoUsuario);
        
            modelBuilder.Entity<Pago>()
                .HasIndex(p => p.FechaPago);
            
            modelBuilder.Entity<Usuarios>()
                .HasIndex(u => new { u.Nombres, u.Apellidos })
                .HasDatabaseName("idx_nombres_apellidos");
        }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Fechas_Usuario> Fechas_Usuarios { get; set; }
        public DbSet<Tipo_Pagos> Tipo_Pagos { get; set; }
        public DbSet<Tipo_Ejercicio> Tipo_Ejercicio { get; set; }
        public DbSet<Producto> Producto { get; set; } = default!;
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Mensaje> Mensaje { get; set; }
    }
}
