using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Data;

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
            .AsEnumerable()                  // materializa para salir del árbol de expresión
            .Select(x => (x.i, x.p))         // ahora sí, tupla
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
}
