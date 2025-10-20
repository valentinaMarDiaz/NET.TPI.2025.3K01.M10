namespace Domain.Model;

public class CarritoItem
{
    public int IdCarritoItem { get; private set; }
    public int IdCarrito { get; private set; }
    public int IdProducto { get; private set; }
    public int Cantidad { get; private set; }

    public CarritoItem() { }
    public CarritoItem(int idCarrito, int idProducto, int cantidad)
    {
        if (idCarrito <= 0) throw new ArgumentException("Carrito inválido");
        if (idProducto <= 0) throw new ArgumentException("Producto inválido");
        if (cantidad <= 0) throw new ArgumentException("Cantidad inválida");

        IdCarrito = idCarrito;
        IdProducto = idProducto;
        Cantidad = cantidad;
    }

    public void SetCantidad(int cantidad) { if (cantidad <= 0) throw new ArgumentException("Cantidad inválida"); Cantidad = cantidad; }
}
