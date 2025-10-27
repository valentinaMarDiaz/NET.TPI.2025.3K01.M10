
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(TPIContext))]
    [Migration("20251025004900_AddDescuentos")]
    partial class AddDescuentos
    {
        
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Model.Carrito", b =>
                {
                    b.Property<int>("IdCarrito")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCarrito"));

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime?>("FechaConfirmacionUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaCreacionUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdCliente")
                        .HasColumnType("int");

                    b.HasKey("IdCarrito");

                    b.HasIndex("IdCliente", "Estado")
                        .HasDatabaseName("IX_Carrito_Cliente_Estado");

                    b.ToTable("Carritos", (string)null);
                });

            modelBuilder.Entity("Domain.Model.CarritoItem", b =>
                {
                    b.Property<int>("IdCarritoItem")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCarritoItem"));

                    b.Property<int>("Cantidad")
                        .HasColumnType("int");

                    b.Property<int>("IdCarrito")
                        .HasColumnType("int");

                    b.Property<int?>("IdDescuento")
                        .HasColumnType("int");

                    b.Property<int>("IdProducto")
                        .HasColumnType("int");

                    b.HasKey("IdCarritoItem");

                    b.HasIndex("IdProducto");

                    b.HasIndex("IdCarrito", "IdProducto")
                        .IsUnique();

                    b.ToTable("CarritoItems", (string)null);
                });

            modelBuilder.Entity("Domain.Model.CategoriaProducto", b =>
                {
                    b.Property<int>("IdCatProducto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IdCatProducto");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCatProducto"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdCatProducto");

                    b.ToTable("Categorias", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Descuento", b =>
                {
                    b.Property<int>("IdDescuento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdDescuento"));

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("nvarchar(24)");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("FechaCaducidadUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaInicioUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdProducto")
                        .HasColumnType("int");

                    b.Property<decimal>("Porcentaje")
                        .HasColumnType("decimal(5,2)");

                    b.HasKey("IdDescuento");

                    b.HasIndex("Codigo")
                        .IsUnique();

                    b.HasIndex("IdProducto");

                    b.ToTable("Descuentos", (string)null);
                });

            modelBuilder.Entity("Domain.Model.PrecioProducto", b =>
                {
                    b.Property<int>("IdPrecioProducto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdPrecioProducto"));

                    b.Property<DateTime>("FechaModificacionUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdProducto")
                        .HasColumnType("int");

                    b.Property<decimal>("Valor")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("IdPrecioProducto");

                    b.HasIndex("IdProducto");

                    b.ToTable("PreciosProductos", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Producto", b =>
                {
                    b.Property<int>("IdProducto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdProducto"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int>("IdCatProducto")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<decimal>("PrecioActual")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.HasKey("IdProducto");

                    b.HasIndex("IdCatProducto");

                    b.HasIndex("Nombre")
                        .IsUnique();

                    b.ToTable("Productos", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("FechaAlta")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TipoUsuario")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Usuarios", null, t =>
                        {
                            t.HasCheckConstraint("CK_Usuarios_ClienteCampos", "(TipoUsuario <> 'Cliente') OR (Telefono IS NOT NULL AND Direccion IS NOT NULL AND Cuil IS NULL)");

                            t.HasCheckConstraint("CK_Usuarios_VendedorCampos", "(TipoUsuario <> 'Vendedor') OR (Cuil IS NOT NULL AND Legajo IS NOT NULL)");
                        });

                    b.HasDiscriminator<string>("TipoUsuario").HasValue("Usuario");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Domain.Model.Venta", b =>
                {
                    b.Property<int>("IdVenta")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdVenta"));

                    b.Property<DateTime>("FechaHoraVentaUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdCliente")
                        .HasColumnType("int");

                    b.HasKey("IdVenta");

                    b.ToTable("Ventas", (string)null);
                });

            modelBuilder.Entity("Domain.Model.VentaDetalle", b =>
                {
                    b.Property<int>("IdVentaDetalle")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdVentaDetalle"));

                    b.Property<int>("Cantidad")
                        .HasColumnType("int");

                    b.Property<int>("IdProducto")
                        .HasColumnType("int");

                    b.Property<int>("IdVenta")
                        .HasColumnType("int");

                    b.Property<decimal>("PrecioUnitario")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("IdVentaDetalle");

                    b.HasIndex("IdProducto");

                    b.HasIndex("IdVenta");

                    b.ToTable("VentaDetalles", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Cliente", b =>
                {
                    b.HasBaseType("Domain.Model.Usuario");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.ToTable(t =>
                        {
                            t.HasCheckConstraint("CK_Usuarios_ClienteCampos", "(TipoUsuario <> 'Cliente') OR (Telefono IS NOT NULL AND Direccion IS NOT NULL AND Cuil IS NULL)");

                            t.HasCheckConstraint("CK_Usuarios_VendedorCampos", "(TipoUsuario <> 'Vendedor') OR (Cuil IS NOT NULL AND Legajo IS NOT NULL)");
                        });

                    b.HasDiscriminator().HasValue("Cliente");
                });

            modelBuilder.Entity("Domain.Model.Vendedor", b =>
                {
                    b.HasBaseType("Domain.Model.Usuario");

                    b.Property<string>("Cuil")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Legajo")
                        .HasColumnType("int");

                    b.HasIndex("Cuil")
                        .IsUnique()
                        .HasFilter("[Cuil] IS NOT NULL");

                    b.HasIndex("Legajo")
                        .IsUnique()
                        .HasFilter("[Legajo] IS NOT NULL");

                    b.ToTable(t =>
                        {
                            t.HasCheckConstraint("CK_Usuarios_ClienteCampos", "(TipoUsuario <> 'Cliente') OR (Telefono IS NOT NULL AND Direccion IS NOT NULL AND Cuil IS NULL)");

                            t.HasCheckConstraint("CK_Usuarios_VendedorCampos", "(TipoUsuario <> 'Vendedor') OR (Cuil IS NOT NULL AND Legajo IS NOT NULL)");
                        });

                    b.HasDiscriminator().HasValue("Vendedor");
                });

            modelBuilder.Entity("Domain.Model.CarritoItem", b =>
                {
                    b.HasOne("Domain.Model.Carrito", null)
                        .WithMany()
                        .HasForeignKey("IdCarrito")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Model.Producto", null)
                        .WithMany()
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Model.Descuento", b =>
                {
                    b.HasOne("Domain.Model.Producto", null)
                        .WithMany()
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Model.PrecioProducto", b =>
                {
                    b.HasOne("Domain.Model.Producto", "Producto")
                        .WithMany()
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Producto");
                });

            modelBuilder.Entity("Domain.Model.Producto", b =>
                {
                    b.HasOne("Domain.Model.CategoriaProducto", "Categoria")
                        .WithMany()
                        .HasForeignKey("IdCatProducto")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Categoria");
                });

            modelBuilder.Entity("Domain.Model.VentaDetalle", b =>
                {
                    b.HasOne("Domain.Model.Producto", null)
                        .WithMany()
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Model.Venta", null)
                        .WithMany()
                        .HasForeignKey("IdVenta")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
