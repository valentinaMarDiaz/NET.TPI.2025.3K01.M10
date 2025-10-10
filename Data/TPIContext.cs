using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data;

public class TPIContext : DbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Vendedor> Vendedores => Set<Vendedor>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<PrecioProducto> PreciosProductos => Set<PrecioProducto>();

    public DbSet<CategoriaProducto> Categorias => Set<CategoriaProducto>();

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

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // ----- CategoriaProducto -----
        mb.Entity<CategoriaProducto>(e =>
        {
            e.ToTable("Categorias");
            e.HasKey(x => x.IdCatProducto);
            e.Property(x => x.IdCatProducto).HasColumnName("IdCatProducto");
            e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
            e.Property(x => x.Descripcion).HasMaxLength(500);
        });

        // ----- Usuarios TPH -----
        
        mb.Entity<Usuario>(e =>
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

        mb.Entity<Cliente>(e =>
        {
            e.Property(x => x.Telefono).HasMaxLength(30);
            e.Property(x => x.Direccion).HasMaxLength(200);
        });
        mb.Entity<Vendedor>(e =>
        {
            e.Property(x => x.Cuil).HasMaxLength(20);
            e.Property(x => x.Legajo).IsRequired();
            e.HasIndex(x => x.Cuil).IsUnique();
            e.HasIndex(x => x.Legajo).IsUnique();
        });
        // -------- Productos --------
        mb.Entity<Producto>(e =>
        {
            e.ToTable("Productos");
            e.HasKey(x => x.IdProducto);
            e.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
            e.HasIndex(x => x.Nombre).IsUnique(); // nombre único en toda la tabla
            e.Property(x => x.Descripcion).HasMaxLength(1000);
            e.Property(x => x.Stock).IsRequired();
            e.Property(x => x.PrecioActual).HasColumnType("decimal(18,2)").IsRequired();

            e.HasOne(x => x.Categoria)
             .WithMany()
             .HasForeignKey(x => x.IdCatProducto)
             .OnDelete(DeleteBehavior.Cascade); // al borrar categoría, se borran productos
        });

        // -------- Historial de precios --------
        mb.Entity<PrecioProducto>(e =>
        {
            e.ToTable("PreciosProductos");
            e.HasKey(x => x.IdPrecioProducto);
            e.Property(x => x.Valor).HasColumnType("decimal(18,2)").IsRequired();
            e.Property(x => x.FechaModificacionUtc).IsRequired();

            e.HasOne(x => x.Producto)
             .WithMany()
             .HasForeignKey(x => x.IdProducto)
             .OnDelete(DeleteBehavior.Cascade); // al borrar producto, se borra su historial
        });
    }

}

