using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class CarritoRepository
    {
        private TPIContext Create() => new();

        public Carrito GetOrCreateAbierto(int idCliente)
        {
            using var ctx = Create();
            var c = ctx.Carritos.FirstOrDefault(x => x.IdCliente == idCliente && x.Estado == "Abierto");
            if (c is null)
            {
                c = new Carrito(idCliente);
                ctx.Carritos.Add(c);
                ctx.SaveChanges();
            }
            return c;
        }

        public Carrito? GetAbierto(int idCliente)
        {
            using var ctx = Create();
            return ctx.Carritos.FirstOrDefault(x => x.IdCliente == idCliente && x.Estado == "Abierto");
        }

        
        public (Carrito carrito, List<(CarritoItem item, Producto prod)>) GetAbiertoConItems(int idCliente)
        {
            using var ctx = Create();
            var carrito = ctx.Carritos.FirstOrDefault(x => x.IdCliente == idCliente && x.Estado == "Abierto");
            if (carrito is null) return (new Carrito(idCliente), new());

            var items = ctx.CarritoItems
                .Where(i => i.IdCarrito == carrito.IdCarrito)
                .Join(ctx.Productos, i => i.IdProducto, p => p.IdProducto, (i, p) => new { i, p })
                .AsEnumerable()                  
                .Select(x => (x.i, x.p))
                .ToList();

            return (carrito, items);
        }

        
        public void AddOrIncreaseItem(int idCliente, int idProducto, int cantidad)
        {
            var strategy = new TPIContext().Database.CreateExecutionStrategy();

            strategy.Execute(() =>
            {
                using var ctx = Create();
                using var tx = ctx.Database.BeginTransaction();

                var carrito = ctx.Carritos
                    .FirstOrDefault(x => x.IdCliente == idCliente && x.Estado == "Abierto");

                if (carrito is null)
                {
                    carrito = new Carrito(idCliente);
                    
                    ctx.Carritos.Add(carrito);
                    ctx.SaveChanges();
                }

                var prod = ctx.Productos.FirstOrDefault(p => p.IdProducto == idProducto)
                           ?? throw new ArgumentException("Producto inexistente.");

                var existente = ctx.CarritoItems
                    .FirstOrDefault(ci => ci.IdCarrito == carrito.IdCarrito && ci.IdProducto == idProducto);

                var cantidadNueva = cantidad + (existente?.Cantidad ?? 0);

                if (prod.Stock < cantidadNueva)
                    throw new ArgumentException("No hay stock suficiente.");

                if (existente is null)
                    ctx.CarritoItems.Add(new CarritoItem(carrito.IdCarrito, idProducto, cantidad));
                else
                    existente.SetCantidad(cantidadNueva);

                ctx.SaveChanges();
                tx.Commit();
            });
        }

        public void RemoveItem(int idCliente, int idProducto)
        {
            using var ctx = Create();
            var carrito = ctx.Carritos.FirstOrDefault(x => x.IdCliente == idCliente && x.Estado == "Abierto");
            if (carrito is null) return;

            var item = ctx.CarritoItems.FirstOrDefault(ci => ci.IdCarrito == carrito.IdCarrito && ci.IdProducto == idProducto);
            if (item is null) return;

            ctx.CarritoItems.Remove(item);
            ctx.SaveChanges();
        }

        public void MarcarConfirmado(int idCarrito)
        {
            using var ctx = Create();
            var c = ctx.Carritos.Find(idCarrito) ?? throw new ArgumentException("Carrito inexistente.");
            c.MarcarConfirmado();
            ctx.SaveChanges();
        }

        
        public void AplicarCodigo(int idCliente, string codigo)
        {
            using var ctx = Create();
            codigo = (codigo ?? "").Trim().ToUpperInvariant();
            var now = DateTime.UtcNow;

            var carrito = ctx.Carritos
                .FirstOrDefault(c => c.IdCliente == idCliente && c.Estado == "Abierto")
                ?? throw new ArgumentException("No hay carrito abierto.");

            var d = ctx.Descuentos.FirstOrDefault(x =>
                x.Codigo == codigo &&
                x.FechaInicioUtc <= now && now <= x.FechaCaducidadUtc);

            if (d is null)
                throw new ArgumentException("Código inválido o vencido.");

            var item = ctx.CarritoItems.FirstOrDefault(i =>
                i.IdCarrito == carrito.IdCarrito && i.IdProducto == d.IdProducto);

            if (item is null)
                throw new ArgumentException("El código no corresponde a ningún producto del carrito.");

            
            item.SetIdDescuento(d.IdDescuento);

            ctx.SaveChanges();
        }


        
        public (int CantidadTotal, decimal Total) GetBadgeYTotal(int idCliente)
        {
            using var ctx = Create();
            var now = DateTime.UtcNow;

            var carrito = ctx.Carritos.AsNoTracking().FirstOrDefault(x => x.IdCliente == idCliente && x.Estado == "Abierto");
            if (carrito is null) return (0, 0m);

            var items =
                from i in ctx.CarritoItems.AsNoTracking().Where(i => i.IdCarrito == carrito.IdCarrito)
                join p in ctx.Productos.AsNoTracking() on i.IdProducto equals p.IdProducto
                select new { i, p };

            int cantidadTotal = 0;
            decimal total = 0m;

            foreach (var x in items)
            {
                var precioUnit = x.p.PrecioActual;
                var subtotal = precioUnit * x.i.Cantidad;
                cantidadTotal += x.i.Cantidad;

                decimal descuentoMonto = 0m;
                if (x.i.IdDescuento.HasValue)
                {
                    var desc = ctx.Descuentos.AsNoTracking()
                        .FirstOrDefault(d => d.IdDescuento == x.i.IdDescuento.Value);

                    if (desc != null && desc.FechaInicioUtc <= now && now <= desc.FechaCaducidadUtc)
                    {
                        descuentoMonto = decimal.Round(subtotal * (desc.Porcentaje / 100m), 2, MidpointRounding.AwayFromZero);
                    }
                }

                var totalLinea = subtotal - descuentoMonto;
                total += totalLinea;
            }

            return (cantidadTotal, total);
        }

       
        public (Carrito carrito, List<(CarritoItem item, Producto prod, Descuento? desc)>) GetAbiertoConItemsYDescuento(int idCliente)
        {
            using var ctx = Create();
            var now = DateTime.UtcNow;

            var carrito = ctx.Carritos.FirstOrDefault(x => x.IdCliente == idCliente && x.Estado == "Abierto");
            if (carrito is null) return (new Carrito(idCliente), new());

          
            var list =
                (from i in ctx.CarritoItems.Where(i => i.IdCarrito == carrito.IdCarrito)
                 join p in ctx.Productos on i.IdProducto equals p.IdProducto
                 select new { i, p, i.IdDescuento })
                .ToList();

            var result = new List<(CarritoItem, Producto, Descuento?)>();

            foreach (var row in list)
            {
                Descuento? desc = null;
                if (row.IdDescuento.HasValue)
                {
                    desc = ctx.Descuentos.AsNoTracking().FirstOrDefault(d => d.IdDescuento == row.IdDescuento.Value);
                    if (desc == null || !(desc.FechaInicioUtc <= now && now <= desc.FechaCaducidadUtc))
                    {
                        
                        var tracked = ctx.CarritoItems.Find(row.i.IdCarritoItem);

                        if (tracked != null)
                        {
                            tracked.SetIdDescuento(null);
                            ctx.SaveChanges();
                        }
                        desc = null;
                    }
                }
                result.Add((row.i, row.p, desc));
            }

            return (carrito, result);
        }
    }
}
