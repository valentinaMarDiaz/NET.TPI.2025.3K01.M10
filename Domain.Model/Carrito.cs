namespace Domain.Model;

public class Carrito
{
    public int IdCarrito { get; private set; }
    public int IdCliente { get; private set; }
    public string Estado { get; private set; } = "Abierto";
    public DateTime FechaCreacionUtc { get; private set; } = DateTime.UtcNow;

  
    public List<CarritoItem> Items { get; private set; } = new();

    public Carrito(int idCliente)
    {
        IdCliente = idCliente;
        Estado = "Abierto";
        FechaCreacionUtc = DateTime.UtcNow;
    }

    public void MarcarConfirmado()
    {
        Estado = "Confirmado";
    }
}
