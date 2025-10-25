using Data;
using DTOs;

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
                ProductoNombre = d.ProductoNombre,        // SNAPSHOT
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
            Detalles = new List<VentaDetalleDTO>() // detalles se cargan con Get(id)
        })
        // Truco: no cargamos detalles, pero queremos el total. Lo devolvemos “calculado” por repo:
        .Select(v =>
        {
            // inyectamos el total sumando SubtotalConDescuento vía repo result (ya viene)
            // Como VentaDTO.Total depende de Detalles, si querés mostrar total en el listado
            // podés agregar una propiedad TotalListado en el DTO o recuperar cada venta con Get(id).
            return v;
        })
        .ToList();
    }

    public void Delete(int idVenta) => _repo.DeleteVenta(idVenta);
}
