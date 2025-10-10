using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class PrecioProductoRepository
{
    private TPIContext Create() => new();

    public PrecioProducto Add(PrecioProducto h)
    {
        using var ctx = Create();
        ctx.PreciosProductos.Add(h);
        ctx.SaveChanges();
        return h;
    }

    public IEnumerable<PrecioProducto> GetByProducto(int idProducto)
    {
        using var ctx = Create();
        return ctx.PreciosProductos.AsNoTracking()
            .Where(x => x.IdProducto == idProducto)
            .OrderByDescending(x => x.FechaModificacionUtc)
            .ToList();
    }
}
