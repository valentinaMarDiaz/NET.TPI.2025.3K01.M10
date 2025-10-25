using System;

namespace Domain.Model
{
    public class VentaDetalle
    {
        public int IdVentaDetalle { get; private set; }
        public int IdVenta { get; private set; }

        // Producto
        public int IdProducto { get; private set; }
        public string ProductoNombre { get; private set; } = string.Empty;   // SNAPSHOT
        public int Cantidad { get; private set; }
        public decimal PrecioUnitario { get; private set; }                   // SNAPSHOT (precio al momento de vender)

        // Descuento aplicado (snapshot)
        public int? IdDescuento { get; private set; }
        public string? CodigoDescuento { get; private set; }
        public decimal? PorcentajeDescuento { get; private set; }

        // Totales línea
        public decimal SubtotalConDescuento { get; private set; }

        public VentaDetalle() { }

        public VentaDetalle(
            int idVenta,
            int idProducto,
            string productoNombre,
            int cantidad,
            decimal precioUnitario,
            int? idDescuento,
            string? codigoDescuento,
            decimal? porcentajeDescuento,
            decimal subtotalConDescuento)
        {
            if (idVenta <= 0) throw new ArgumentException("Venta inválida");
            if (idProducto <= 0) throw new ArgumentException("Producto inválido");
            if (string.IsNullOrWhiteSpace(productoNombre)) throw new ArgumentException("Nombre de producto requerido");
            if (cantidad <= 0) throw new ArgumentException("Cantidad inválida");
            if (precioUnitario < 0) throw new ArgumentException("Precio inválido");
            if (subtotalConDescuento < 0) throw new ArgumentException("Subtotal inválido");

            IdVenta = idVenta;
            IdProducto = idProducto;
            ProductoNombre = productoNombre.Trim();
            Cantidad = cantidad;
            PrecioUnitario = decimal.Round(precioUnitario, 2, MidpointRounding.AwayFromZero);

            IdDescuento = idDescuento;
            CodigoDescuento = codigoDescuento;
            PorcentajeDescuento = porcentajeDescuento;

            SubtotalConDescuento = decimal.Round(subtotalConDescuento, 2, MidpointRounding.AwayFromZero);
        }
    }
}
