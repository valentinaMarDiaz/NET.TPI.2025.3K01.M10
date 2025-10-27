using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{

    public partial class VentaDetalleSnapshot : Migration
    {
    
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carrito_Cliente_Estado",
                table: "Carritos");

            migrationBuilder.DropColumn(
                name: "FechaConfirmacionUtc",
                table: "Carritos");

            migrationBuilder.AddColumn<string>(
                name: "CodigoDescuento",
                table: "VentaDetalles",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdDescuento",
                table: "VentaDetalles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PorcentajeDescuento",
                table: "VentaDetalles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductoNombre",
                table: "VentaDetalles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SubtotalConDescuento",
                table: "VentaDetalles",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Porcentaje",
                table: "Descuentos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Descuentos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Descuentos",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(24)",
                oldMaxLength: 24);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacionUtc",
                table: "Carritos",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoDescuento",
                table: "VentaDetalles");

            migrationBuilder.DropColumn(
                name: "IdDescuento",
                table: "VentaDetalles");

            migrationBuilder.DropColumn(
                name: "PorcentajeDescuento",
                table: "VentaDetalles");

            migrationBuilder.DropColumn(
                name: "ProductoNombre",
                table: "VentaDetalles");

            migrationBuilder.DropColumn(
                name: "SubtotalConDescuento",
                table: "VentaDetalles");

            migrationBuilder.AlterColumn<decimal>(
                name: "Porcentaje",
                table: "Descuentos",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Descuentos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Descuentos",
                type: "nvarchar(24)",
                maxLength: 24,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacionUtc",
                table: "Carritos",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaConfirmacionUtc",
                table: "Carritos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carrito_Cliente_Estado",
                table: "Carritos",
                columns: new[] { "IdCliente", "Estado" });
        }
    }
}
