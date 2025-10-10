using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class CategoriaRepository
{
    private TPIContext Create() => new();

    public void Add(CategoriaProducto c)
    {
        using var ctx = Create();
        ctx.Categorias.Add(c);
        ctx.SaveChanges();
    }

    public bool Update(CategoriaProducto c)
    {
        using var ctx = Create();
        var db = ctx.Categorias.Find(c.IdCatProducto);
        if (db is null) return false;
        db.SetNombre(c.Nombre);
        db.SetDescripcion(c.Descripcion);
        ctx.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        using var ctx = Create();
        var db = ctx.Categorias.Find(id);
        if (db is null) return false;
        ctx.Categorias.Remove(db);
        ctx.SaveChanges();
        return true;
    }

    public CategoriaProducto? Get(int id)
    {
        using var ctx = Create();
        return ctx.Categorias.Find(id);
    }

    public IEnumerable<CategoriaProducto> GetAll()
    {
        using var ctx = Create();
        return ctx.Categorias.AsNoTracking()
            .OrderBy(x => x.Nombre).ToList();
    }

    public bool NameExists(string nombre, int? excludeId = null)
    {
        using var ctx = Create();
        var q = ctx.Categorias.AsNoTracking()
            .Where(x => x.Nombre.ToLower() == nombre.ToLower());
        if (excludeId.HasValue) q = q.Where(x => x.IdCatProducto != excludeId.Value);
        return q.Any();
    }
}
