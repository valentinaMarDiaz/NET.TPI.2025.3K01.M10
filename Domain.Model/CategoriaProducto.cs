namespace Domain.Model;

public class CategoriaProducto
{
    public int IdCatProducto { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;

    public CategoriaProducto() { }

    public CategoriaProducto(int id, string nombre, string descripcion)
    {
        SetId(id); SetNombre(nombre); SetDescripcion(descripcion);
    }

    public void SetId(int id) { if (id < 0) throw new ArgumentException("Id inválido"); IdCatProducto = id; }
    public void SetNombre(string n) { if (string.IsNullOrWhiteSpace(n)) throw new ArgumentException("Nombre requerido"); Nombre = n.Trim(); }
    public void SetDescripcion(string d) { Descripcion = (d ?? "").Trim(); }
}
