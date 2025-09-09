using Microsoft.EntityFrameworkCore;
using SimpleMarketplace.Api.Entities;

namespace SimpleMarketplace.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Comentario> Comentarios { get; set; }
    public DbSet<Administrador> Administradores { get; set; }
    public DbSet<Configuracion> Configuraciones { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }
        public DbSet<CarritoItem> Carrito { get; set; }
    public DbSet<LogAdministrativo> LogsAdministrativos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }
        public DbSet<Factura> Facturas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<CarritoItem>().HasIndex(c => new { c.UsuarioId, c.ProductoId }).IsUnique();
            modelBuilder.Entity<Producto>().HasIndex(p => p.Categoria).HasDatabaseName("idx_productos_categoria");
            modelBuilder.Entity<Pedido>().HasIndex(p => p.UsuarioId).HasDatabaseName("idx_pedidos_usuario");
            modelBuilder.Entity<Pedido>().HasIndex(p => p.Estado).HasDatabaseName("idx_pedidos_estado");
            modelBuilder.Entity<CarritoItem>().HasIndex(c => c.UsuarioId).HasDatabaseName("idx_carrito_usuario");

            // Column defaults and constraints to match SQL DDL
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(u => u.ContrasenaHash).HasColumnName("contraseñaHash");
                entity.Property(u => u.Estado).HasColumnName("estado").HasMaxLength(20).HasDefaultValue("activo");
                entity.Property(u => u.FechaCreacion).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(u => u.FechaActualizacion).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.Property(p => p.Estado).HasColumnName("estado").HasMaxLength(20).HasDefaultValue("disponible");
                entity.Property(p => p.FechaCreacion).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(p => p.FechaActualizacion).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Administrador>(entity =>
            {
                entity.HasIndex(a => a.Email).IsUnique();
                entity.Property(a => a.ContrasenaHash).HasColumnName("contraseñaHash");
                entity.Property(a => a.FechaCreacion).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(a => a.NivelAcceso).HasMaxLength(20).HasDefaultValue("basico");
                entity.Property(a => a.Estado).HasColumnName("estado").HasMaxLength(20).HasDefaultValue("activo");
            });

            modelBuilder.Entity<Configuracion>(entity =>
            {
                entity.HasIndex(c => c.Clave).IsUnique();
                entity.Property(c => c.FechaActualizacion).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Direccion>(entity =>
            {
                entity.Property(d => d.Estado).HasColumnName("estado").HasMaxLength(100).HasDefaultValue("");
            });

            modelBuilder.Entity<Factura>(entity =>
            {
                entity.HasIndex(f => f.NumeroFactura).IsUnique();
            });

            // Configure cascade delete where appropriate
            modelBuilder.Entity<Direccion>()
                .HasOne(d => d.Usuario)
                .WithMany(u => u.Direcciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MetodoPago>()
                .HasOne(m => m.Usuario)
                .WithMany(u => u.MetodosPago)
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarritoItem>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Carrito)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarritoItem>()
                .HasOne(c => c.Producto)
                .WithMany(p => p.Carritos)
                .HasForeignKey(c => c.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetallePedido>()
                .HasOne(d => d.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetallePedido>()
                .HasOne(d => d.Producto)
                .WithMany(p => p.DetallesPedido)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Direccion)
                .WithMany()
                .HasForeignKey(p => p.DireccionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.MetodoPago)
                .WithMany()
                .HasForeignKey(p => p.MetodoPagoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Pedido)
                .WithOne(p => p.Factura)
                .HasForeignKey<Factura>(f => f.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LogAdministrativo>()
                .HasOne(l => l.Administrador)
                .WithMany()
                .HasForeignKey(l => l.AdminId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configurar relaciones de Comentario
            modelBuilder.Entity<Comentario>()
                .HasOne(c => c.Producto)
                .WithMany(p => p.Comentarios)
                .HasForeignKey(c => c.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comentario>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
