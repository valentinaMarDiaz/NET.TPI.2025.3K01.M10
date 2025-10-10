namespace DTOs;

public class ProductoDTO
{
    public int IdProducto { get; set; }
    public int IdCatProducto { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int Stock { get; set; }
    public decimal PrecioActual { get; set; }
    public string? CategoriaNombre { get; set; }  // solo lectura para grilla
}
