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
       
    public decimal TotalListado { get; set; }

    public decimal Total => Detalles.Sum(d => d.SubtotalConDescuento);
}


public class VentaDetalleDTO
{
    public int IdProducto { get; set; }
    public string ProductoNombre { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    public int? IdDescuento { get; set; }
    public string? CodigoDescuento { get; set; }
    public decimal? PorcentajeDescuento { get; set; }

    public decimal SubtotalConDescuento { get; set; }

     public decimal Subtotal => SubtotalConDescuento;
}


public class VentaFiltroDTO
{
    public int? IdCliente { get; set; }
    public DateTime? DesdeUtc { get; set; }
    public DateTime? HastaUtc { get; set; }
}
