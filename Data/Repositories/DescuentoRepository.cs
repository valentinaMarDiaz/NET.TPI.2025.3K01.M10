using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class DescuentoRepository
    {
        private TPIContext Create() => new TPIContext();

        public Descuento Add(Descuento d)
        {
            using var ctx = Create();
            if (ctx.Descuentos.Any(x => x.Codigo == d.Codigo))
                throw new ArgumentException($"Ya existe un descuento con código '{d.Codigo}'.");

            ctx.Descuentos.Add(d);
            ctx.SaveChanges();
            return d;
        }

        public bool Update(Descuento d)
        {
            using var ctx = Create();

            if (ctx.Descuentos.Any(x => x.Codigo == d.Codigo && x.IdDescuento != d.IdDescuento))
                throw new ArgumentException($"Ya existe otro descuento con código '{d.Codigo}'.");

            var current = ctx.Descuentos.Find(d.IdDescuento);
            if (current == null) return false;

            current.SetIdProducto(d.IdProducto);
            current.SetFechas(d.FechaInicioUtc, d.FechaCaducidadUtc);
            current.SetDescripcion(d.Descripcion);
            current.SetCodigo(d.Codigo);
            current.SetPorcentaje(d.Porcentaje);

            ctx.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            using var ctx = Create();
            var d = ctx.Descuentos.Find(id);
            if (d == null) return false;
            ctx.Descuentos.Remove(d);
            ctx.SaveChanges();
            return true;
        }

        public Descuento? Get(int id)
        {
            using var ctx = Create();
            return ctx.Descuentos.AsNoTracking().FirstOrDefault(x => x.IdDescuento == id);
        }

        public IEnumerable<Descuento> GetAll(string? productoNombre = null)
        {
            using var ctx = Create();

            var q =
                from d in ctx.Descuentos.AsNoTracking()
                join p in ctx.Productos.AsNoTracking() on d.IdProducto equals p.IdProducto
                select new { d, p };

            if (!string.IsNullOrWhiteSpace(productoNombre))
            {
                var pat = productoNombre.Trim().ToLower();
                q = q.Where(x => x.p.Nombre.ToLower().Contains(pat));
            }

            return q
                .OrderBy(x => x.d.FechaCaducidadUtc)
                .Select(x => new Descuento(
                    x.d.IdDescuento, x.d.IdProducto, x.d.FechaInicioUtc, x.d.FechaCaducidadUtc,
                    x.d.Descripcion, x.d.Codigo, x.d.Porcentaje))
                .ToList();
        }

        public IEnumerable<(Descuento d, string ProductoNombre)> GetVigentes(string? productoOrCodigo = null)
        {
            using var ctx = Create();
            var now = DateTime.UtcNow;

            var q =
                from d in ctx.Descuentos.AsNoTracking()
                join p in ctx.Productos.AsNoTracking() on d.IdProducto equals p.IdProducto
                where d.FechaInicioUtc <= now && now <= d.FechaCaducidadUtc
                select new { d, p };

            if (!string.IsNullOrEmpty(productoOrCodigo))
            {
                var pat = productoOrCodigo.Trim().ToUpperInvariant();
                q = q.Where(x => x.p.Nombre.ToUpper().Contains(pat) || x.d.Codigo.ToUpper() == pat);
            }

            return q
                .OrderBy(x => x.d.FechaCaducidadUtc)
                .AsEnumerable()
                .Select(x => (x.d, x.p.Nombre))
                .ToList();
        }

        public Descuento? GetVigentePorCodigo(string codigo)
        {
            using var ctx = Create();
            var now = DateTime.UtcNow;
            var code = (codigo ?? "").Trim().ToUpperInvariant();

            return ctx.Descuentos.AsNoTracking()
                .FirstOrDefault(x => x.Codigo == code &&
                                     x.FechaInicioUtc <= now && now <= x.FechaCaducidadUtc);
        }
    }
}
