using System.Security.Cryptography;
using System.Text;
using Data;
using Domain.Model;
using DTOs;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;


namespace Application.Services;

public class AuthService
{
    private readonly AuthRepository _repo = new();

    static string Sha256(string text)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text ?? ""));
        return Convert.ToHexString(bytes);
    }

    static void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new ArgumentException("Email inválido.");
    }
    static void ValidarSoloDigitos(string v, string campo)
    {
        if (string.IsNullOrWhiteSpace(v) || !v.All(char.IsDigit))
            throw new ArgumentException($"{campo} inválido (solo dígitos).");
    }

    public LoginResponseDTO RegisterCliente(RegisterClienteDTO dto)
    {
        ValidarEmail(dto.Email);
        if (_repo.EmailExists(dto.Email)) throw new ArgumentException("Email ya registrado.");
        ValidarSoloDigitos(dto.Telefono, "Teléfono");
        if (string.IsNullOrWhiteSpace(dto.Direccion)) throw new ArgumentException("Dirección requerida.");
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 4) throw new ArgumentException("Password muy corta.");

        var cli = new Cliente(dto.Nombre, dto.Apellido, dto.Email, Sha256(dto.Password), dto.Telefono, dto.Direccion);
        cli = _repo.RegisterCliente(cli);


        return new LoginResponseDTO { Id = cli.Id, Nombre = cli.Nombre, Apellido = cli.Apellido, TipoUsuario = "Cliente" };
    }

    public LoginResponseDTO RegisterVendedor(RegisterVendedorDTO dto)
    {
        ValidarEmail(dto.Email);
        if (_repo.EmailExists(dto.Email)) throw new ArgumentException("Email ya registrado.");
        ValidarSoloDigitos(dto.Cuil, "CUIL");
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 4) throw new ArgumentException("Password muy corta.");

        var ven = new Vendedor(dto.Nombre, dto.Apellido, dto.Email, Sha256(dto.Password), dto.Cuil);
        ven.AsignarLegajo(_repo.NextLegajo());
        ven = _repo.RegisterVendedor(ven);


        return new LoginResponseDTO { Id = ven.Id, Nombre = ven.Nombre, Apellido = ven.Apellido, TipoUsuario = "Vendedor", Legajo = ven.Legajo };
    }


    public LoginResponseDTO? Login(LoginRequestDTO dto, IConfiguration config)
    {
        ValidarEmail(dto.Email);
        var user = _repo.FindByEmail(dto.Email);
        if (user is null) return null;
        if (!string.Equals(user.PasswordHash, Sha256(dto.Password), StringComparison.OrdinalIgnoreCase))
            return null;

        string tipoUsuario = user is Cliente ? "Cliente" : "Vendedor";
        string token = GenerateJwtToken(user, tipoUsuario, config);

        return user switch
        {
            Cliente c => new LoginResponseDTO { Id = c.Id, Nombre = c.Nombre, Apellido = c.Apellido, TipoUsuario = "Cliente", Token = token }, // <-- Agrega Token
            Vendedor v => new LoginResponseDTO { Id = v.Id, Nombre = v.Nombre, Apellido = v.Apellido, TipoUsuario = "Vendedor", Legajo = v.Legajo, Token = token }, // <-- Agrega Token
            _ => null
        };
    }


    private string GenerateJwtToken(Usuario user, string tipoUsuario, IConfiguration config)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key no configurada"));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.Nombre} {user.Apellido}"),
            new Claim(ClaimTypes.Role, tipoUsuario) 
        };

        if (user is Vendedor v)
        {
            claims.Add(new Claim("legajo", v.Legajo.ToString()));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7), 
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}