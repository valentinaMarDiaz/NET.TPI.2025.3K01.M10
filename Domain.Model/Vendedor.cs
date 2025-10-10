namespace Domain.Model;

public class Vendedor : Usuario
{
    public string Cuil { get; private set; } = "";
    public int Legajo { get; private set; }   // ahora se asigna con método

    public Vendedor() { }

    public Vendedor(string nombre, string apellido, string email, string passwordHash, string cuil)
        : base(nombre, apellido, email, passwordHash)
    {
        SetCuil(cuil);
    }

    public void SetCuil(string v)
    {
        v = (v ?? "").Trim();
        if (v.Length == 0 || !v.All(char.IsDigit)) throw new ArgumentException("CUIL inválido (solo dígitos).");
        Cuil = v;
    }

    public void AsignarLegajo(int legajo)
    {
        if (legajo <= 0) throw new ArgumentException("Legajo inválido.");
        Legajo = legajo;
    }
}
