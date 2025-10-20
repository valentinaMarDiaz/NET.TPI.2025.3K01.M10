namespace Domain.Model;

public class Venta
{
    public int IdVenta { get; private set; }
    public int IdCliente { get; private set; }
    public DateTime FechaHoraVentaUtc { get; private set; }

    public Venta() { }
    public Venta(int idCliente)
    {
        if (idCliente <= 0) throw new ArgumentException("Cliente inválido");
        IdCliente = idCliente;
        FechaHoraVentaUtc = DateTime.UtcNow;
    }
}
