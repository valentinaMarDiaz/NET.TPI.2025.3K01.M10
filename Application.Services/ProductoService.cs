using Data;
using Domain.Model;
using DTOs;

namespace Application.Services;

public class ProductoService
{
    private readonly ProductoRepository _repo = new();
    private readonly PrecioProductoRepository _histRepo = new();

    public ProductoDTO Add(ProductoDTO dto)
    {
        if (dto.IdCatProducto <= 0) throw new ArgumentException("Categoría requerida.");
        if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("Nombre requerido.");
        if (_repo.NombreExiste(dto.Nombre)) throw new ArgumentException("Ya existe un producto con ese nombre.");
        if (dto.Stock < 0) throw new ArgumentException("Stock >= 0.");
        if (dto.PrecioActual < 0) throw new ArgumentException("Precio >= 0.");

        var p = new Producto(0, dto.IdCatProducto, dto.Nombre, dto.Descripcion ?? "", dto.Stock, dto.PrecioActual);
        p = _repo.Add(p);

        // precio inicial al historial
        _histRepo.Add(new PrecioProducto(0, p.IdProducto, p.PrecioActual, DateTime.UtcNow));

        var saved = _repo.Get(p.IdProducto)!;
        return Map(saved);
    }

    public bool Update(ProductoDTO dto)
    {
        if (dto.IdProducto <= 0) throw new ArgumentException("Id inválido.");
        if (dto.IdCatProducto <= 0) throw new ArgumentException("Categoría requerida.");
        if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("Nombre requerido.");
        if (_repo.NombreExiste(dto.Nombre, dto.IdProducto)) throw new ArgumentException("Ya existe un producto con ese nombre.");
        if (dto.Stock < 0) throw new ArgumentException("Stock >= 0.");
        if (dto.PrecioActual < 0) throw new ArgumentException("Precio >= 0.");

        var actual = _repo.Get(dto.IdProducto);
        if (actual is null) return false;

        bool precioCambio = actual.PrecioActual != dto.PrecioActual;

        var p = new Producto(dto.IdProducto, dto.IdCatProducto, dto.Nombre, dto.Descripcion ?? "", dto.Stock, dto.PrecioActual);
        var ok = _repo.Update(p);

        if (ok && precioCambio)
            _histRepo.Add(new PrecioProducto(0, p.IdProducto, p.PrecioActual, DateTime.UtcNow));

        return ok;
    }

    public bool Delete(int id) => _repo.Delete(id);

    public ProductoDTO? Get(int id) => _repo.Get(id) is Producto p ? Map(p) : null;

    public IEnumerable<ProductoDTO> GetAll() => _repo.GetAll().Select(Map);

    public IEnumerable<PrecioProductoDTO> GetHistorial(int idProducto)
        => _histRepo.GetByProducto(idProducto).Select(h => new PrecioProductoDTO
        {
            IdPrecioProducto = h.IdPrecioProducto,
            IdProducto = h.IdProducto,
            ProductoNombre = _repo.Get(h.IdProducto)?.Nombre ?? "",
            Valor = h.Valor,
            FechaModificacionUtc = h.FechaModificacionUtc
        });

    private static ProductoDTO Map(Producto p) => new()
    {
        IdProducto = p.IdProducto,
        IdCatProducto = p.IdCatProducto,
        Nombre = p.Nombre,
        Descripcion = p.Descripcion,
        Stock = p.Stock,
        PrecioActual = p.PrecioActual,
        CategoriaNombre = p.Categoria?.Nombre
    };
}
