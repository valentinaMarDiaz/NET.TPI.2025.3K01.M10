namespace DTOs;

public class PrecioProductoDTO
{
    public int IdPrecioProducto { get; set; }
    public int IdProducto { get; set; }
    public string ProductoNombre { get; set; } = "";
    public decimal Valor { get; set; }
    public DateTime FechaModificacionUtc { get; set; }

    
    public string Edad => GetEdad();

    private string GetEdad()
    {
        var ts = DateTime.UtcNow - FechaModificacionUtc;
        if (ts.TotalDays >= 1) return $"{(int)ts.TotalDays} d";
        if (ts.TotalHours >= 1) return $"{(int)ts.TotalHours} h";
        return $"{(int)ts.TotalMinutes} min";
    }
}
