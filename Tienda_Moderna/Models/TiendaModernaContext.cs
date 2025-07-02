using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_Moderna.Models
{
    public class TiendaModernaContext : DbContext
    {
        public TiendaModernaContext(DbContextOptions<TiendaModernaContext> options)
            : base(options)
        {
        }

        public DbSet<Marca> Cat_Marcas { get; set; }
        public DbSet<Proveedor> Cat_Proveedores { get; set; }
        public DbSet<Producto> Cat_Productos { get; set; }
        public DbSet<Cliente> Cat_Clientes { get; set; }
        public DbSet<Inventario> ViewInventario { get; set; }
        public DbSet<NotificacionCaducidad> NotificacionCaducidad { get; set; }
        public DbSet<NotificacionStock> NotificacionStock { get; set; }
        public DbSet<Venta> Cat_Ventas { get; set; }
        public DbSet<Corte> Cat_Cortes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inventario>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("view_inventario");
            });
            modelBuilder.Entity<NotificacionCaducidad>(entity =>
            {
                entity.HasNoKey(); // Indica que no tiene clave primaria
            });
            modelBuilder.Entity<NotificacionStock>(entity =>
            {
                entity.HasNoKey(); // Indica que no tiene clave primaria
            });
        }
    }
}