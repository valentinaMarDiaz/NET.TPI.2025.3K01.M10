using Data;
using DTOs;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class VentaService
{
    private readonly VentaRepository _repo = new();

    public VentaDTO Confirmar(int idCliente)
    {
        var venta = _repo.ConfirmarCarrito(idCliente);
        return Get(venta.IdVenta)!;
    }

    public VentaDTO? Get(int idVenta)
    {
        var (v, dets, cliente) = _repo.GetVenta(idVenta);
        var dto = new VentaDTO
        {
            IdVenta = v.IdVenta,
            IdCliente = v.IdCliente,
            FechaHoraVentaUtc = v.FechaHoraVentaUtc,
            ClienteNombre = cliente is null ? "" : $"{cliente.Nombre} {cliente.Apellido}",
            ClienteTelefono = (cliente as Domain.Model.Cliente)?.Telefono,
            ClienteDireccion = (cliente as Domain.Model.Cliente)?.Direccion
        };

        foreach (var (d, p) in dets)
        {
            dto.Detalles.Add(new VentaDetalleDTO
            {
                IdProducto = p.IdProducto,
                ProductoNombre = d.ProductoNombre,        
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                IdDescuento = d.IdDescuento,
                CodigoDescuento = d.CodigoDescuento,
                PorcentajeDescuento = d.PorcentajeDescuento,
                SubtotalConDescuento = d.SubtotalConDescuento
            });
        }
        return dto;
    }

    public IEnumerable<VentaDTO> List(VentaFiltroDTO filtro)
    {
        var list = _repo.GetVentas(filtro.IdCliente, filtro.DesdeUtc, filtro.HastaUtc);
        return list.Select(x => new VentaDTO
        {
            IdVenta = x.v.IdVenta,
            IdCliente = x.v.IdCliente,
            FechaHoraVentaUtc = x.v.FechaHoraVentaUtc,
            ClienteNombre = x.c is null ? "" : $"{x.c.Nombre} {x.c.Apellido}",
            ClienteTelefono = (x.c as Domain.Model.Cliente)?.Telefono,
            ClienteDireccion = (x.c as Domain.Model.Cliente)?.Direccion,
            TotalListado = x.total, 
            Detalles = new List<VentaDetalleDTO>()
        }).ToList(); 
    }

    public IEnumerable<VentasMesDTO> TotalesPorMes(int anio)
    {
        using var ctx = new TPIContext();

        var desde = new DateTime(anio, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var hasta = desde.AddYears(1);

       

        var query =
            from v in ctx.Ventas.AsNoTracking()
            where v.FechaHoraVentaUtc >= desde && v.FechaHoraVentaUtc < hasta
            join d in ctx.VentaDetalles.AsNoTracking() on v.IdVenta equals d.IdVenta
            group d by v.FechaHoraVentaUtc.Month into g
            select new VentasMesDTO
            {
                Mes = g.Key,
                Total = g.Sum(x => x.SubtotalConDescuento)
            };

        var list = query.ToList();

        
        var dict = list.ToDictionary(x => x.Mes, x => x.Total);
        return Enumerable.Range(1, 12)
            .Select(m => new VentasMesDTO { Mes = m, Total = dict.TryGetValue(m, out var t) ? t : 0m })
            .ToList();
    }
    

    public void Delete(int idVenta) => _repo.DeleteVenta(idVenta);
}
