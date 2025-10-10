using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AuthRepository
{
    private TPIContext Create() => new();

    public bool EmailExists(string email)
    {
        using var ctx = Create();
        return ctx.Usuarios.AsNoTracking().Any(u => u.Email.ToLower() == email.ToLower());
    }

    public bool CuilExists(string cuil)
    {
        using var ctx = Create();
        return ctx.Vendedores.AsNoTracking().Any(v => v.Cuil == cuil);
    }

    public int NextLegajo()
    {
        using var ctx = Create();
        var last = ctx.Vendedores.AsNoTracking()
                   .OrderByDescending(v => v.Legajo)
                   .Select(v => (int?)v.Legajo).FirstOrDefault();
        return (last ?? 1000) + 1;
    }

    public Cliente RegisterCliente(Cliente c)
    {
        using var ctx = Create();
        ctx.Clientes.Add(c);
        ctx.SaveChanges();
        return c;
    }

    public Vendedor RegisterVendedor(Vendedor v)
    {
        using var ctx = Create();
        if (v.Legajo <= 0) v.AsignarLegajo(NextLegajo());  // <- acá
        ctx.Vendedores.Add(v);
        ctx.SaveChanges();
        return v;
    }

    public Usuario? FindByEmail(string email)
    {
        using var ctx = Create();
        return ctx.Usuarios.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
    }
}
