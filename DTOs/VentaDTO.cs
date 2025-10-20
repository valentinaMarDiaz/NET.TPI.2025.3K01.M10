namespace DTOs;

public class VentaDTO
{
    public int IdVenta { get; set; }
    public int IdCliente { get; set; }
    public string ClienteNombre { get; set; } = "";
    public string? ClienteTelefono { get; set; }
    public string? ClienteDireccion { get; set; }
    public DateTime FechaHoraVentaUtc { get; set; }
    public List<VentaDetalleDTO> Detalles { get; set; } = new();
    public decimal Total => Detalles.Sum(d => d.Subtotal);
}

public class VentaDetalleDTO
{
    public int IdProducto { get; set; }
    public string ProductoNombre { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => decimal.Round(PrecioUnitario * Cantidad, 2);
}

public class VentaFiltroDTO
{
    public int? IdCliente { get; set; }
    public DateTime? DesdeUtc { get; set; }
    public DateTime? HastaUtc { get; set; }
}
