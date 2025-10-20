namespace DTOs;

public class CarritoDTO
{
    public int IdCarrito { get; set; }
    public int IdCliente { get; set; }
    public string Estado { get; set; } = "Abierto";
    public DateTime FechaCreacionUtc { get; set; }
    public List<CarritoItemDTO> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.Subtotal);
    public int CantidadTotal => Items.Sum(i => i.Cantidad);
}

public class CarritoItemDTO
{
    public int IdCarritoItem { get; set; }
    public int IdProducto { get; set; }
    public string ProductoNombre { get; set; } = "";
    public decimal PrecioActual { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal => decimal.Round(PrecioActual * Cantidad, 2);
}

public class AgregarCarritoDTO
{
    public int IdCliente { get; set; }
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
}

public class EliminarItemCarritoDTO
{
    public int IdCliente { get; set; }
    public int IdProducto { get; set; }
}

public class ConfirmarCarritoDTO
{
    public int IdCliente { get; set; }
}
