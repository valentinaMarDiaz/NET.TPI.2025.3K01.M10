using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class VentaRepository
{
    private TPIContext Create() => new();

    public Venta ConfirmarCarrito(int idCliente)
    {
        var strategy = new TPIContext().Database.CreateExecutionStrategy();

        return strategy.Execute(() =>
        {
            using var ctx = Create();
            using var tx = ctx.Database.BeginTransaction();

            var carrito = ctx.Carritos
                .FirstOrDefault(x => x.IdCliente == idCliente && x.Estado == "Abierto")
                ?? throw new ArgumentException("No hay carrito abierto para el cliente.");

            var items = (from i in ctx.CarritoItems.Where(i => i.IdCarrito == carrito.IdCarrito)
                         join p in ctx.Productos on i.IdProducto equals p.IdProducto
                         select new { i, p }).ToList();

            if (items.Count == 0)
                throw new ArgumentException("El carrito está vacío.");

            // Validar stock
            foreach (var x in items)
                if (x.p.Stock < x.i.Cantidad)
                    throw new ArgumentException($"Stock insuficiente para '{x.p.Nombre}'.");

            var venta = new Venta(idCliente);
            ctx.Ventas.Add(venta);
            ctx.SaveChanges(); // obtener IdVenta

            var ahora = DateTime.UtcNow;

            foreach (var x in items)
            {
                // Precio y descuento vigente al momento de vender
                var precio = x.p.PrecioActual;

                int? idDesc = null;
                string? codigoDesc = null;
                decimal? porcentajeDesc = null;

                if (x.i.IdDescuento.HasValue)
                {
                    var desc = ctx.Descuentos.FirstOrDefault(d => d.IdDescuento == x.i.IdDescuento.Value);
                    if (desc != null && desc.EstaVigenteUtc(ahora))
                    {
                        idDesc = desc.IdDescuento;
                        codigoDesc = desc.Codigo;
                        porcentajeDesc = desc.Porcentaje;
                    }
                }

                var subtotal = precio * x.i.Cantidad;
                var subtotalConDesc = porcentajeDesc.HasValue
                    ? decimal.Round(subtotal * (100m - porcentajeDesc.Value) / 100m, 2, MidpointRounding.AwayFromZero)
                    : subtotal;

                // SNAPSHOT
                var detalle = new VentaDetalle(
                    idVenta: venta.IdVenta,
                    idProducto: x.p.IdProducto,
                    productoNombre: x.p.Nombre,
                    cantidad: x.i.Cantidad,
                    precioUnitario: precio,
                    idDescuento: idDesc,
                    codigoDescuento: codigoDesc,
                    porcentajeDescuento: porcentajeDesc,
                    subtotalConDescuento: subtotalConDesc
                );

                ctx.VentaDetalles.Add(detalle);

                // Restar stock
                x.p.SetStock(x.p.Stock - x.i.Cantidad);
            }

            carrito.MarcarConfirmado();

            ctx.SaveChanges();
            tx.Commit();
            return venta;
        });
    }

    public (Venta venta, List<(VentaDetalle d, Producto p)> detalles, Cliente? cliente) GetVenta(int idVenta)
    {
        using var ctx = Create();
        var v = ctx.Ventas.Find(idVenta) ?? throw new ArgumentException("Venta inexistente.");

        var dets = ctx.VentaDetalles
            .Where(d => d.IdVenta == idVenta)
            .Join(ctx.Productos, d => d.IdProducto, p => p.IdProducto, (d, p) => new { d, p })
            .AsEnumerable()
            .Select(x => (x.d, x.p))
            .ToList();

        var cliente = ctx.Set<Cliente>().FirstOrDefault(c => c.Id == v.IdCliente);
        return (v, dets, cliente);
    }

    public IEnumerable<(Venta v, Cliente? c, decimal total)> GetVentas(int? idCliente, DateTime? desdeUtc, DateTime? hastaUtc)
    {
        using var ctx = Create();

        var q = ctx.Ventas.AsQueryable();
        if (idCliente.HasValue) q = q.Where(v => v.IdCliente == idCliente.Value);
        if (desdeUtc.HasValue) q = q.Where(v => v.FechaHoraVentaUtc >= desdeUtc.Value);
        if (hastaUtc.HasValue) q = q.Where(v => v.FechaHoraVentaUtc <= hastaUtc.Value);

          var result = q
         .OrderByDescending(v => v.FechaHoraVentaUtc)
         .Select(v => new
         {
             venta = v,
             total = ctx.VentaDetalles.Where(d => d.IdVenta == v.IdVenta)
                                      .Select(d => (decimal?)d.SubtotalConDescuento)
                                      .Sum() ?? 0m
         })
         .ToList();


        var clientes = ctx.Set<Cliente>().ToDictionary(c => c.Id, c => c);
        return result.Select(x => (x.venta, clientes.GetValueOrDefault(x.venta.IdCliente), x.total));
    }


    public void DeleteVenta(int idVenta)
    {
        using var ctx = Create();
        using var tx = ctx.Database.BeginTransaction();

        var venta = ctx.Ventas.Find(idVenta) ?? throw new ArgumentException("Venta inexistente.");
        var dets = ctx.VentaDetalles.Where(d => d.IdVenta == idVenta).ToList();

        foreach (var d in dets)
        {
            var p = ctx.Productos.Find(d.IdProducto);
            if (p != null) p.SetStock(p.Stock + d.Cantidad);
        }

        ctx.VentaDetalles.RemoveRange(dets);
        ctx.Ventas.Remove(venta);
        ctx.SaveChanges();
        tx.Commit();
    }
}
