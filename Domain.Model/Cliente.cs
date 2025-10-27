namespace Domain.Model;

public class Cliente : Usuario
{
    public string Telefono { get; private set; } = "";
    public string Direccion { get; private set; } = "";

    public Cliente() { } 
    public Cliente(string nombre, string apellido, string email, string passwordHash,
                   string telefono, string direccion)
        : base(nombre, apellido, email, passwordHash)
    {
        SetTelefono(telefono);
        SetDireccion(direccion);
    }

    public void SetTelefono(string v)
    {
        v = (v ?? "").Trim();
        if (v.Length == 0 || !v.All(char.IsDigit)) throw new ArgumentException("Teléfono inválido (solo dígitos).");
        Telefono = v;
    }

    public void SetDireccion(string v)
    {
        v = (v ?? "").Trim();
        if (v.Length == 0) throw new ArgumentException("Dirección requerida.");
        Direccion = v;
    }
}
