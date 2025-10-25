// Domain.Model/CarritoItem.cs
namespace Domain.Model
{
    public class CarritoItem
    {
        public int IdCarritoItem { get; private set; }
        public int IdCarrito { get; private set; }
        public int IdProducto { get; private set; }
        public int Cantidad { get; private set; }
        public int? IdDescuento { get; private set; }

        public Carrito? Carrito { get; private set; }

        public CarritoItem(int idCarrito, int idProducto, int cantidad)
        {
            IdCarrito = idCarrito;
            IdProducto = idProducto;
            SetCantidad(cantidad);
        }

        public void SetCantidad(int cantidad)
        {
            if (cantidad <= 0) throw new ArgumentException("Cantidad inválida.");
            Cantidad = cantidad;
        }

        public void SetIdDescuento(int? idDesc) => IdDescuento = idDesc;
    }
}
