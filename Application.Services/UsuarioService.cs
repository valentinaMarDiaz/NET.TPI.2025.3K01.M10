using Data;
using Domain.Model;
using DTOs;

namespace Application.Services;

public class UsuarioService
{
    private readonly UsuarioRepository _repo = new();

    public IEnumerable<UsuarioDTO> GetAll()
        => _repo.GetAll().Select(Map);

    public UsuarioDTO? Get(int id)
        => _repo.Get(id) is Usuario u ? Map(u) : null;

    public bool Update(UsuarioDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains("@"))
            throw new ArgumentException("Email inválido.");

        Usuario u = dto.TipoUsuario == "Cliente"
            ? new Cliente(dto.Nombre, dto.Apellido, dto.Email, "DUMMY", dto.Telefono ?? "", dto.Direccion ?? "")
            : new Vendedor(dto.Nombre, dto.Apellido, dto.Email, "DUMMY", dto.Cuil ?? "");

        
        typeof(Usuario).GetProperty("Id")!.SetValue(u, dto.Id);

       
        u.SetNombre(dto.Nombre);
        u.SetApellido(dto.Apellido);
        u.SetEmail(dto.Email);

       
        if (u is Cliente c)
        {
            if (string.IsNullOrWhiteSpace(dto.Telefono) || !dto.Telefono.All(char.IsDigit))
                throw new ArgumentException("Teléfono inválido.");
            if (string.IsNullOrWhiteSpace(dto.Direccion))
                throw new ArgumentException("Dirección requerida.");
            c.SetTelefono(dto.Telefono);
            c.SetDireccion(dto.Direccion);
        }
        else if (u is Vendedor v)
        {
            if (string.IsNullOrWhiteSpace(dto.Cuil) || !dto.Cuil.All(char.IsDigit))
                throw new ArgumentException("CUIL inválido.");
            v.SetCuil(dto.Cuil);
        }

        return _repo.Update(u);
    }

    public bool Delete(int id) => _repo.Delete(id);

    private static UsuarioDTO Map(Usuario u) => u switch
    {
        Cliente c => new UsuarioDTO
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Apellido = c.Apellido,
            Email = c.Email,
            TipoUsuario = "Cliente",
            Telefono = c.Telefono,
            Direccion = c.Direccion
        },
        Vendedor v => new UsuarioDTO
        {
            Id = v.Id,
            Nombre = v.Nombre,
            Apellido = v.Apellido,
            Email = v.Email,
            TipoUsuario = "Vendedor",
            Cuil = v.Cuil,
            Legajo = v.Legajo
        },
        _ => new UsuarioDTO { Id = u.Id, Nombre = u.Nombre, Apellido = u.Apellido, Email = u.Email }
    };
}
