// Application.Services/CarritoService.cs
using Data;
using DTOs;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CarritoService
    {
        private readonly CarritoRepository _repo = new();

        public CarritoDTO Get(int idCliente)
        {
            var (carrito, pares) = _repo.GetAbiertoConItems(idCliente);

            using var ctx = new TPIContext();

            var items = pares.Select(t =>
            {
                var d = t.item.IdDescuento.HasValue
                    ? ctx.Descuentos.AsNoTracking().FirstOrDefault(x => x.IdDescuento == t.item.IdDescuento.Value)
                    : null;

                var dto = new CarritoItemDTO
                {
                    IdProducto = t.prod.IdProducto,
                    ProductoNombre = t.prod.Nombre,
                    Cantidad = t.item.Cantidad,
                    PrecioUnitario = t.prod.PrecioActual,
                    IdDescuento = t.item.IdDescuento,
                    CodigoDescuento = d?.Codigo,
                    Porcentaje = d?.Porcentaje
                };

                var sub = dto.Cantidad * dto.PrecioUnitario;
                if (dto.Porcentaje.HasValue)
                {
                    sub = Math.Round(sub * (100m - dto.Porcentaje.Value) / 100m,
                                     2, MidpointRounding.AwayFromZero);
                }
                dto.Subtotal = sub;

                return dto;
            }).ToList();

            return new CarritoDTO
            {
                IdCarrito = carrito.IdCarrito,
                IdCliente = carrito.IdCliente,
                Estado = carrito.Estado,
                Items = items,
                Total = items.Sum(x => x.Subtotal),
                CantidadTotal = items.Sum(x => x.Cantidad)
            };
        }

        public void Remove(int idCliente, int idProducto) => _repo.RemoveItem(idCliente, idProducto);
        public void AplicarCodigo(int idCliente, string codigo) => _repo.AplicarCodigo(idCliente, codigo);
        public void MarcarConfirmado(int idCarrito) => _repo.MarcarConfirmado(idCarrito);
        public void AddOrIncreaseItem(int idCliente, int idProducto, int cantidad)
    => _repo.AddOrIncreaseItem(idCliente, idProducto, cantidad);

    }
}
