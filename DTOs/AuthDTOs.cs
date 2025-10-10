namespace DTOs;

public class RegisterClienteDTO
{
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Direccion { get; set; } = "";
}

public class RegisterVendedorDTO
{
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Cuil { get; set; } = "";
}

public class LoginRequestDTO
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResponseDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string TipoUsuario { get; set; } = ""; // "Cliente" o "Vendedor"
    public int? Legajo { get; set; }
}
