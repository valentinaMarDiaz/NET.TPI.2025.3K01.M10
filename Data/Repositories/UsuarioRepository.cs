using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class UsuarioRepository
{
    private TPIContext CreateContext() => new();

    public void Add(Usuario u)
    {
        using var ctx = CreateContext();
        ctx.Usuarios.Add(u);
        ctx.SaveChanges();
    }

    public bool Delete(int id)
    {
        using var ctx = CreateContext();
        var ent = ctx.Usuarios.Find(id);
        if (ent is null) return false;
        ctx.Usuarios.Remove(ent);
        ctx.SaveChanges();
        return true;
    }

    public Usuario? Get(int id)
    {
        using var ctx = CreateContext();
        return ctx.Usuarios.Find(id);
    }

    public IEnumerable<Usuario> GetAll()
    {
        using var ctx = CreateContext();
        return ctx.Usuarios.AsNoTracking()
            .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
            .ToList();
    }

    public bool Update(Usuario u)
    {
        using var ctx = CreateContext();
        var ent = ctx.Usuarios.Find(u.Id);
        if (ent is null) return false;

        ent.SetNombre(u.Nombre);
        ent.SetApellido(u.Apellido);
        ent.SetEmail(u.Email);

        if (ent is Cliente eCli && u is Cliente nCli)
        {
            eCli.SetTelefono(nCli.Telefono);
            eCli.SetDireccion(nCli.Direccion);
        }
        else if (ent is Vendedor eVen && u is Vendedor nVen)
        {
            eVen.SetCuil(nVen.Cuil);
            // Legajo no se toca acá
        }

        ctx.SaveChanges();
        return true;
    }


    public bool EmailExists(string email, int? excludeId = null)
    {
        using var ctx = CreateContext();
        var q = ctx.Usuarios.Where(x => x.Email.ToLower() == email.ToLower());
        if (excludeId.HasValue) q = q.Where(x => x.Id != excludeId.Value);
        return q.Any();
    }

    // Búsqueda por texto y tipo ("Cliente" | "Vendedor" | null)
    public IEnumerable<Usuario> GetByCriteria(string texto, string? tipo = null)
    {
        using var ctx = CreateContext();
        texto = (texto ?? "").Trim();
        IQueryable<Usuario> q = ctx.Usuarios.AsNoTracking();

        if (!string.IsNullOrEmpty(texto))
        {
            q = q.Where(u => u.Nombre.Contains(texto) ||
                             u.Apellido.Contains(texto) ||
                             u.Email.Contains(texto));
        }

        if (tipo == "Cliente") q = q.OfType<Cliente>();
        if (tipo == "Vendedor") q = q.OfType<Vendedor>();

        return q.OrderBy(u => u.Apellido).ThenBy(u => u.Nombre).ToList();
    }
}
