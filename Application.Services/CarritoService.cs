using Data;
using DTOs;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CarritoService
    {
        // ===================== Helpers =====================

        private static Domain.Model.Carrito GetOrCreateCart(TPIContext ctx, int idCliente)
        {
            var carrito = ctx.Carritos
                .Include(c => c.Items)
                .FirstOrDefault(c => c.IdCliente == idCliente && c.Estado == "Abierto");

            if (carrito == null)
            {
                // Tu entidad exige ctor con idCliente
                carrito = new Domain.Model.Carrito(idCliente);
                ctx.Carritos.Add(carrito);
                ctx.SaveChanges();

                // Aseguramos Items cargados
                ctx.Entry(carrito).Collection("Items").Load();
            }

            return carrito;
        }

        // Obtiene un PRECIO decimal de la entidad Producto, aceptando varios posibles nombres
        private static decimal GetPrecioFromProduct(object producto)
        {
            var t = producto.GetType();
            foreach (var name in new[] { "Precio", "PrecioActual", "Valor", "PrecioUnitario" })
            {
                var pi = t.GetProperty(name);
                if (pi != null && pi.PropertyType == typeof(decimal))
                    return (decimal)(pi.GetValue(producto) ?? 0m);
            }
            return 0m;
        }

        private static string GetNombreFromProduct(object producto)
        {
            var pi = producto.GetType().GetProperty("Nombre");
            var val = pi?.GetValue(producto) as string;
            return val ?? "";
        }

        private static bool DescuentoVigente(Domain.Model.Descuento d, DateTime ahoraUtc)
            => d.FechaInicioUtc <= ahoraUtc && ahoraUtc <= d.FechaCaducidadUtc;

        private static CarritoDTO MapToDto(TPIContext ctx, Domain.Model.Carrito c)
        {
            // Traemos items + producto + (descuento) y calculamos precio/porcentaje por JOIN,
            // sin depender de propiedades inexistentes en CarritoItem.
            var rows =
                (from i in ctx.CarritoItems.AsNoTracking().Where(x => x.IdCarrito == c.IdCarrito)
                 join p in ctx.Productos.AsNoTracking() on i.IdProducto equals p.IdProducto
                 join d in ctx.Descuentos.AsNoTracking() on EF.Property<int?>(i, "IdDescuento") equals d.IdDescuento into dj
                 from d in dj.DefaultIfEmpty()
                 select new { i, p, d })
                .ToList();

            var items = new List<CarritoItemDTO>(rows.Count);

            foreach (var r in rows)
            {
                var precioUnit = GetPrecioFromProduct(r.p);
                decimal? porcentaje = r.d?.Porcentaje;
                var cant = r.i.Cantidad;  // por si Cantidad tiene set privado
                var nombre = GetNombreFromProduct(r.p);

                decimal subtotal = porcentaje.HasValue && porcentaje.Value > 0
                    ? decimal.Round(cant * precioUnit * (100 - porcentaje.Value) / 100m, 2, MidpointRounding.AwayFromZero)
                    : cant * precioUnit;

                items.Add(new CarritoItemDTO
                {
                    IdProducto = r.i.IdProducto,
                    ProductoNombre = nombre,
                    Cantidad = cant,
                    PrecioUnitario = precioUnit,
                    IdDescuento = r.d?.IdDescuento,
                    CodigoDescuento = r.d?.Codigo,
                    Porcentaje = porcentaje,
                    Subtotal = subtotal
                });
            }

            return new CarritoDTO
            {
                IdCarrito = c.IdCarrito,
                IdCliente = c.IdCliente,
                Estado = c.Estado,
                CantidadTotal = items.Sum(x => x.Cantidad),
                Total = items.Sum(x => x.Subtotal),
                Items = items
            };
        }

        // ===================== API usado por Program.cs =====================

        public CarritoDTO Get(int idCliente)
        {
            using var ctx = new TPIContext();
            var carrito = GetOrCreateCart(ctx, idCliente);
            return MapToDto(ctx, carrito);
        }

        public void AddOrIncreaseItem(int idCliente, int idProducto, int cantidad)
        {
            if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor a 0.");

            using var ctx = new TPIContext();

            if (!ctx.Productos.Any(p => p.IdProducto == idProducto))
                throw new ArgumentException("Producto inexistente.");

            var carrito = GetOrCreateCart(ctx, idCliente);

            var item = ctx.CarritoItems.FirstOrDefault(i => i.IdCarrito == carrito.IdCarrito && i.IdProducto == idProducto);
            if (item == null)
            {
                // Tu entidad exige ctor (idCarrito,idProducto,cantidad)
                item = new Domain.Model.CarritoItem(carrito.IdCarrito, idProducto, cantidad);
                ctx.CarritoItems.Add(item);
            }
            else
            {
                // Incrementar cantidad usando EF (por si set es privado)
                var e = ctx.Entry(item);
                var actual = e.Property("Cantidad").CurrentValue as int? ?? 0;
                e.Property("Cantidad").CurrentValue = actual + cantidad;
            }

            ctx.SaveChanges();
        }

        public void AplicarCodigo(int idCliente, string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Código vacío.");

            var code = codigo.Trim().ToUpperInvariant();

            using var ctx = new TPIContext();

            var carrito = GetOrCreateCart(ctx, idCliente);

            // Buscar descuento vigente
            var now = DateTime.UtcNow;
            var desc = ctx.Descuentos.AsNoTracking()
                        .FirstOrDefault(d => d.Codigo == code && DescuentoVigente(d, now));

            if (desc == null)
                throw new ArgumentException("El código es inválido o está vencido.");

            // Aplicarlo a items del producto correspondiente
            var items = ctx.CarritoItems
                           .Where(i => i.IdCarrito == carrito.IdCarrito && i.IdProducto == desc.IdProducto)
                           .ToList();

            if (items.Count == 0)
                throw new ArgumentException("El código no aplica a productos de tu carrito.");

            foreach (var it in items)
            {
                // Solo seteamos IdDescuento (lo demás lo resolvemos por JOIN al mapear)
                ctx.Entry(it).Property("IdDescuento").CurrentValue = desc.IdDescuento;
            }

            ctx.SaveChanges();
        }

        public void Remove(int idCliente, int idProducto)
        {
            using var ctx = new TPIContext();

            var carrito = ctx.Carritos
                .Include(c => c.Items)
                .FirstOrDefault(c => c.IdCliente == idCliente && c.Estado == "Abierto");

            if (carrito is null) return;

            var item = carrito.Items.FirstOrDefault(i => i.IdProducto == idProducto);
            if (item is null) return;

            ctx.CarritoItems.Remove(item);
            ctx.SaveChanges();
        }

        public void RemoveMany(int idCliente, IEnumerable<int> idsProducto)
        {
            var ids = idsProducto?.Distinct().ToArray() ?? Array.Empty<int>();
            if (ids.Length == 0) return;

            using var ctx = new TPIContext();

            var carrito = ctx.Carritos
                .Include(c => c.Items)
                .FirstOrDefault(c => c.IdCliente == idCliente && c.Estado == "Abierto");

            if (carrito is null) return;

            var aBorrar = carrito.Items.Where(i => ids.Contains(i.IdProducto)).ToList();
            if (aBorrar.Count == 0) return;

            ctx.CarritoItems.RemoveRange(aBorrar);
            ctx.SaveChanges();
        }
    }
}
