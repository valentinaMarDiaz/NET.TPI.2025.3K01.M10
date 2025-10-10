namespace Domain.Model;

public class Producto
{
    public int IdProducto { get; private set; }
    public int IdCatProducto { get; private set; }
    public string Nombre { get; private set; } = "";
    public string Descripcion { get; private set; } = "";
    public int Stock { get; private set; }
    public decimal PrecioActual { get; private set; }

    // navegación opcional
    public CategoriaProducto? Categoria { get; private set; }

    public Producto() { }

    public Producto(int id, int idCat, string nombre, string descripcion, int stock, decimal precio)
    {
        SetId(id);
        SetIdCatProducto(idCat);
        SetNombre(nombre);
        SetDescripcion(descripcion);
        SetStock(stock);
        SetPrecioActual(precio);
    }

    public void SetId(int id) { if (id < 0) throw new ArgumentException("Id inválido"); IdProducto = id; }
    public void SetIdCatProducto(int idCat) { if (idCat <= 0) throw new ArgumentException("Categoría requerida"); IdCatProducto = idCat; }
    public void SetNombre(string v) { if (string.IsNullOrWhiteSpace(v)) throw new ArgumentException("Nombre requerido"); Nombre = v.Trim(); }
    public void SetDescripcion(string v) { Descripcion = (v ?? "").Trim(); }
    public void SetStock(int s) { if (s < 0) throw new ArgumentException("Stock >= 0"); Stock = s; }
    public void SetPrecioActual(decimal p) { if (p < 0) throw new ArgumentException("Precio >= 0"); PrecioActual = decimal.Round(p, 2); }
}
