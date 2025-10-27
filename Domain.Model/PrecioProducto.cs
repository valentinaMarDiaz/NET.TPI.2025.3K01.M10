namespace Domain.Model;

public class PrecioProducto
{
    public int IdPrecioProducto { get; private set; }
    public int IdProducto { get; private set; }
    public decimal Valor { get; private set; }
    public DateTime FechaModificacionUtc { get; private set; }

    
    public Producto? Producto { get; private set; }

    public PrecioProducto() { }

    public PrecioProducto(int id, int idProducto, decimal valor, DateTime fechaUtc)
    {
        SetId(id);
        SetIdProducto(idProducto);
        SetValor(valor);
        SetFecha(fechaUtc);
    }

    public void SetId(int id) { if (id < 0) throw new ArgumentException("Id inválido"); IdPrecioProducto = id; }
    public void SetIdProducto(int id) { if (id <= 0) throw new ArgumentException("Producto requerido"); IdProducto = id; }
    public void SetValor(decimal v) { if (v < 0) throw new ArgumentException("Valor >= 0"); Valor = decimal.Round(v, 2); }
    public void SetFecha(DateTime fUtc) { if (fUtc == default) throw new ArgumentException("Fecha requerida"); FechaModificacionUtc = DateTime.SpecifyKind(fUtc, DateTimeKind.Utc); }
}
