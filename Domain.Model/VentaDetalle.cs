using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{

    public class VentaDetalle
    {
        public int IdVentaDetalle { get; private set; }
        public int IdVenta { get; private set; }
        public int IdProducto { get; private set; }
        public int Cantidad { get; private set; }
        public decimal PrecioUnitario { get; private set; }

        public VentaDetalle() { }
        public VentaDetalle(int idVenta, int idProducto, int cantidad, decimal precioUnitario)
        {
            if (idVenta <= 0) throw new ArgumentException("Venta inválida");
            if (idProducto <= 0) throw new ArgumentException("Producto inválido");
            if (cantidad <= 0) throw new ArgumentException("Cantidad inválida");
            if (precioUnitario < 0) throw new ArgumentException("Precio inválido");

            IdVenta = idVenta;
            IdProducto = idProducto;
            Cantidad = cantidad;
            PrecioUnitario = decimal.Round(precioUnitario, 2);
        }
    }
}
