// DTOs/CarritoDTO.cs
namespace DTOs
{
    public class CarritoDTO
    {
        public int IdCarrito { get; set; }
        public int IdCliente { get; set; }
        public string Estado { get; set; } = "Abierto";

        // Para el badge del carrito
        public int CantidadTotal { get; set; }

        public decimal Total { get; set; }
        public List<CarritoItemDTO> Items { get; set; } = new();
    }

    public class CarritoItemDTO
    {
        public int IdProducto { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public int? IdDescuento { get; set; }
        public string? CodigoDescuento { get; set; }
        public decimal? Porcentaje { get; set; }

        // set con setter para que el servicio lo asigne sin errores
        public decimal Subtotal { get; set; }
    }
}
