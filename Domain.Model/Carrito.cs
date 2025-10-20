namespace Domain.Model;

public class Carrito
{
    public int IdCarrito { get; private set; }
    public int IdCliente { get; private set; }     // FK a Usuario (Cliente)
    public DateTime FechaCreacionUtc { get; private set; }
    public string Estado { get; private set; } = "Abierto"; // Abierto | Confirmado | Cancelado
    public DateTime? FechaConfirmacionUtc { get; private set; }

    public Carrito() { }

    public Carrito(int idCliente)
    {
        SetIdCliente(idCliente);
        FechaCreacionUtc = DateTime.UtcNow;
        Estado = "Abierto";
    }

    public void SetIdCliente(int id) { if (id <= 0) throw new ArgumentException("Cliente inválido"); IdCliente = id; }
    public void MarcarConfirmado() { Estado = "Confirmado"; FechaConfirmacionUtc = DateTime.UtcNow; }
}
