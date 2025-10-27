using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection.Emit;

namespace Data;

public class TPIContext : DbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Vendedor> Vendedores => Set<Vendedor>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<PrecioProducto> PreciosProductos => Set<PrecioProducto>();

    public DbSet<CategoriaProducto> Categorias => Set<CategoriaProducto>();
    public DbSet<Carrito> Carritos => Set<Carrito>();
    public DbSet<CarritoItem> CarritoItems => Set<CarritoItem>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<VentaDetalle> VentaDetalles => Set<VentaDetalle>();
    public DbSet<Domain.Model.Descuento> Descuentos { get; set; } = null!;


    public TPIContext() { }
    public TPIContext(DbContextOptions<TPIContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder b)
    {
        if (!b.IsConfigured)
        {
            var cfg = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
            var cs = cfg.GetConnectionString("DefaultConnection")
                     ?? "Server=(localdb)\\MSSQLLocalDB;Database=SmartTienda;Trusted_Connection=true";
            b.UseSqlServer(cs, o => o.EnableRetryOnFailure());
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<CategoriaProducto>(e =>
        {
            e.ToTable("Categorias");
            e.HasKey(x => x.IdCatProducto);
            e.Property(x => x.IdCatProducto).HasColumnName("IdCatProducto");
            e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
            e.Property(x => x.Descripcion).HasMaxLength(500);
        });

        

        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.HasDiscriminator<string>("TipoUsuario")
                .HasValue<Cliente>("Cliente")
                .HasValue<Vendedor>("Vendedor");

            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();

            e.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Usuarios_ClienteCampos",
                  "(TipoUsuario <> 'Cliente') OR (Telefono IS NOT NULL AND Direccion IS NOT NULL AND Cuil IS NULL)");
                tb.HasCheckConstraint("CK_Usuarios_VendedorCampos",
                  "(TipoUsuario <> 'Vendedor') OR (Cuil IS NOT NULL AND Legajo IS NOT NULL)");
            });
        });

        modelBuilder.Entity<Cliente>(e =>
        {
            e.Property(x => x.Telefono).HasMaxLength(30);
            e.Property(x => x.Direccion).HasMaxLength(200);
        });
        modelBuilder.Entity<Vendedor>(e =>
        {
            e.Property(x => x.Cuil).HasMaxLength(20);
            e.Property(x => x.Legajo).IsRequired();
            e.HasIndex(x => x.Cuil).IsUnique();
            e.HasIndex(x => x.Legajo).IsUnique();
        });
       
        modelBuilder.Entity<Producto>(e =>
        {
            e.ToTable("Productos");
            e.HasKey(x => x.IdProducto);
            e.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
            e.HasIndex(x => x.Nombre).IsUnique(); 
            e.Property(x => x.Descripcion).HasMaxLength(1000);
            e.Property(x => x.Stock).IsRequired();
            e.Property(x => x.PrecioActual).HasColumnType("decimal(18,2)").IsRequired();

            e.HasOne(x => x.Categoria)
             .WithMany()
             .HasForeignKey(x => x.IdCatProducto)
             .OnDelete(DeleteBehavior.Cascade); 
        });

       
        modelBuilder.Entity<PrecioProducto>(e =>
        {
            e.ToTable("PreciosProductos");
            e.HasKey(x => x.IdPrecioProducto);
            e.Property(x => x.Valor).HasColumnType("decimal(18,2)").IsRequired();
            e.Property(x => x.FechaModificacionUtc).IsRequired();

            e.HasOne(x => x.Producto)
             .WithMany()
             .HasForeignKey(x => x.IdProducto)
             .OnDelete(DeleteBehavior.Cascade); 
        });
      
        modelBuilder.Entity<Carrito>(e =>
        {
            e.ToTable("Carritos");
            e.HasKey(c => c.IdCarrito);
            e.Property(c => c.Estado).HasMaxLength(20).IsRequired();
            e.Property(c => c.FechaCreacionUtc)
             .HasColumnType("datetime2")
             .HasDefaultValueSql("GETUTCDATE()")
             .IsRequired();
        });

        
        modelBuilder.Entity<CarritoItem>(e =>
        {
            e.ToTable("CarritoItems");
            e.HasKey(i => i.IdCarritoItem);

            
            e.Property(i => i.IdCarrito).HasColumnName("IdCarrito");
            e.Property(i => i.IdProducto).HasColumnName("IdProducto");
            e.Property(i => i.Cantidad).IsRequired();

            
            e.HasOne(i => i.Carrito)
             .WithMany(c => c.Items)
             .HasForeignKey(i => i.IdCarrito)
             .HasPrincipalKey(c => c.IdCarrito)
             .OnDelete(DeleteBehavior.Cascade);

            
            e.HasOne<Producto>()
             .WithMany()
             .HasForeignKey(i => i.IdProducto)
             .OnDelete(DeleteBehavior.Restrict);

            
            e.HasIndex(i => new { i.IdCarrito, i.IdProducto }).IsUnique();
        });


       
        modelBuilder.Entity<Venta>(e =>
        {
            e.ToTable("Ventas");
            e.HasKey(x => x.IdVenta);
            e.Property(x => x.FechaHoraVentaUtc).IsRequired();
        });

        modelBuilder.Entity<VentaDetalle>(e =>
        {
            e.ToTable("VentaDetalles");
            e.HasKey(x => x.IdVentaDetalle);

            e.Property(x => x.ProductoNombre)
             .HasMaxLength(200)
             .IsRequired();

            e.Property(x => x.PrecioUnitario)
             .HasColumnType("decimal(18,2)")
             .IsRequired();

            e.Property(x => x.SubtotalConDescuento)
             .HasColumnType("decimal(18,2)")
             .IsRequired();

            e.Property(x => x.PorcentajeDescuento)
             .HasColumnType("decimal(18,2)");

            e.Property(x => x.CodigoDescuento)
             .HasMaxLength(64);

            e.HasOne<Venta>()
             .WithMany()
             .HasForeignKey(x => x.IdVenta)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne<Producto>()
             .WithMany()
             .HasForeignKey(x => x.IdProducto)
             .OnDelete(DeleteBehavior.Restrict);
        });



        modelBuilder.Entity<Descuento>(b =>
        {
            b.HasKey(x => x.IdDescuento);

            b.Property(x => x.Codigo).IsRequired().HasMaxLength(64);
            b.HasIndex(x => x.Codigo).IsUnique();

            
            b.Property("Porcentaje").HasColumnType("decimal(18,2)");

            
            b.HasOne<Producto>()
             .WithMany()
             .HasForeignKey("IdProducto")
             .OnDelete(DeleteBehavior.Cascade);

            
        });
    }
}





