namespace Domain.Model;

public abstract class Usuario
{
    public int Id { get; protected set; }                   
    public string Nombre { get; protected set; } = "";
    public string Apellido { get; protected set; } = "";
    public string Email { get; protected set; } = "";
    public string PasswordHash { get; protected set; } = "";
    public DateTime FechaAlta { get; protected set; } = DateTime.UtcNow;

    protected Usuario() { }

    protected Usuario(string nombre, string apellido, string email, string passwordHash)
    {
        SetNombre(nombre);
        SetApellido(apellido);
        SetEmail(email);
        SetPasswordHash(passwordHash);
        FechaAlta = DateTime.UtcNow;
    }

    public void SetNombre(string v) { if (string.IsNullOrWhiteSpace(v)) throw new ArgumentException("Nombre requerido"); Nombre = v.Trim(); }
    public void SetApellido(string v) { if (string.IsNullOrWhiteSpace(v)) throw new ArgumentException("Apellido requerido"); Apellido = v.Trim(); }
    public void SetEmail(string v)
    {
        v = v?.Trim() ?? "";
        if (v.Length < 5 || !v.Contains('@')) throw new ArgumentException("Email inválido");
        Email = v;
    }
    public void SetPasswordHash(string v)
    {
        if (string.IsNullOrWhiteSpace(v)) throw new ArgumentException("Password hash requerido");
        PasswordHash = v;
    }
}
