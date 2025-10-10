using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ProductoRepository
{
    private TPIContext Create() => new();

    public bool NombreExiste(string nombre, int? excludeId = null)
    {
        using var ctx = Create();
        var q = ctx.Productos.AsNoTracking().Where(p => p.Nombre.ToLower() == nombre.ToLower());
        if (excludeId.HasValue) q = q.Where(p => p.IdProducto != excludeId.Value);
        return q.Any();
    }

    public Producto Add(Producto p)
    {
        using var ctx = Create();
        ctx.Productos.Add(p);
        ctx.SaveChanges();
        return p;
    }

    public bool Update(Producto p)
    {
        using var ctx = Create();
        var db = ctx.Productos.Find(p.IdProducto);
        if (db is null) return false;
        db.SetNombre(p.Nombre);
        db.SetDescripcion(p.Descripcion);
        db.SetStock(p.Stock);
        db.SetPrecioActual(p.PrecioActual);
        db.SetIdCatProducto(p.IdCatProducto);
        ctx.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        using var ctx = Create();
        var db = ctx.Productos.Find(id);
        if (db is null) return false;
        ctx.Productos.Remove(db);
        ctx.SaveChanges();
        return true;
    }

    public Producto? Get(int id)
    {
        using var ctx = Create();
        return ctx.Productos.Include(p => p.Categoria).FirstOrDefault(p => p.IdProducto == id);
    }

    public IEnumerable<Producto> GetAll()
    {
        using var ctx = Create();
        return ctx.Productos.Include(p => p.Categoria)
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .ToList();
    }
}
