using Data;
using DTOs;

namespace Application.Services;

public class CarritoService
{
    private readonly CarritoRepository _repo = new();
    private readonly ProductoRepository _prodRepo = new();

    public CarritoDTO GetAbierto(int idCliente)
    {
        var (carrito, items) = _repo.GetAbiertoConItems(idCliente);
        var dto = new CarritoDTO
        {
            IdCarrito = carrito.IdCarrito,
            IdCliente = idCliente,
            Estado = carrito.Estado,
            FechaCreacionUtc = carrito.FechaCreacionUtc
        };
        foreach (var (i, p) in items)
        {
            dto.Items.Add(new CarritoItemDTO
            {
                IdCarritoItem = i.IdCarritoItem,
                IdProducto = i.IdProducto,
                ProductoNombre = p.Nombre,
                PrecioActual = p.PrecioActual,
                Cantidad = i.Cantidad
            });
        }
        return dto;
    }

    public CarritoDTO Add(AgregarCarritoDTO dto)
    {
        if (dto.Cantidad <= 0) throw new ArgumentException("Cantidad inválida.");
        _repo.AddOrIncreaseItem(dto.IdCliente, dto.IdProducto, dto.Cantidad);
        return GetAbierto(dto.IdCliente);
    }

    public CarritoDTO Remove(EliminarItemCarritoDTO dto)
    {
        _repo.RemoveItem(dto.IdCliente, dto.IdProducto);
        return GetAbierto(dto.IdCliente);
    }
}
