using Data;
using DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Application.Services
{
    public class DescuentoService
    {
        public IEnumerable<DescuentoDTO> GetAll()
        {
            using var ctx = new TPIContext();

            // No dependemos de navegación: hacemos join para traer el nombre del producto
            var q = from d in ctx.Descuentos
                    join p in ctx.Productos on d.IdProducto equals p.IdProducto
                    select new DescuentoDTO
                    {
                        IdDescuento = d.IdDescuento,
                        IdProducto = d.IdProducto,
                        ProductoNombre = p.Nombre,
                        Codigo = d.Codigo,
                        Descripcion = d.Descripcion,
                        Porcentaje = d.Porcentaje,
                        FechaInicioUtc = d.FechaInicioUtc,
                        FechaCaducidadUtc = d.FechaCaducidadUtc
                    };

            return q.ToList();
        }

        public DescuentoDTO Add(DescuentoDTO dto)
        {
            using var ctx = new TPIContext();

            // --- Validaciones
            if (dto.IdProducto <= 0) throw new ArgumentException("Debe seleccionar un producto.");
            if (!ctx.Productos.Any(p => p.IdProducto == dto.IdProducto))
                throw new ArgumentException("El producto indicado no existe.");

            if (string.IsNullOrWhiteSpace(dto.Codigo))
                throw new ArgumentException("El código es obligatorio.");

            var codigo = dto.Codigo.Trim().ToUpperInvariant();

            if (ctx.Descuentos.Any(d => d.Codigo == codigo))
                throw new ArgumentException("Ya existe un descuento con ese código.");

            if (dto.Porcentaje <= 0)
                throw new ArgumentException("El porcentaje debe ser mayor que 0.");

            if (dto.FechaInicioUtc == default || dto.FechaCaducidadUtc == default)
                throw new ArgumentException("Las fechas son obligatorias.");

            if (dto.FechaInicioUtc > dto.FechaCaducidadUtc)
                throw new ArgumentException("La fecha de inicio no puede ser posterior a la de caducidad.");

            // ⚠️ Orden REAL del constructor de tu entidad:
            // Descuento(int idDescuento, int idProducto, DateTime fechaInicioUtc, DateTime fechaCaducidadUtc,
            //           string descripcion, string codigo, decimal porcentaje)
            var entity = new Domain.Model.Descuento(
                0,
                dto.IdProducto,
                dto.FechaInicioUtc,
                dto.FechaCaducidadUtc,
                dto.Descripcion?.Trim() ?? string.Empty,
                codigo,
                dto.Porcentaje
            );

            ctx.Descuentos.Add(entity);
            ctx.SaveChanges();

            dto.IdDescuento = entity.IdDescuento;
            dto.Codigo = entity.Codigo;
            return dto;
        }

        public bool Update(DescuentoDTO dto)
        {
            using var ctx = new TPIContext();

            var d = ctx.Descuentos.FirstOrDefault(x => x.IdDescuento == dto.IdDescuento);
            if (d == null) return false;

            // --- Validaciones
            if (dto.IdProducto <= 0 || !ctx.Productos.Any(p => p.IdProducto == dto.IdProducto))
                throw new ArgumentException("El producto indicado no existe.");

            var codigo = (dto.Codigo ?? string.Empty).Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código es obligatorio.");

            if (ctx.Descuentos.Any(x => x.Codigo == codigo && x.IdDescuento != d.IdDescuento))
                throw new ArgumentException("Ya existe otro descuento con ese código.");

            if (dto.Porcentaje <= 0)
                throw new ArgumentException("El porcentaje debe ser mayor que 0.");

            if (dto.FechaInicioUtc == default || dto.FechaCaducidadUtc == default || dto.FechaInicioUtc > dto.FechaCaducidadUtc)
                throw new ArgumentException("Rango de fechas inválido.");

            // ⚠️ Tus propiedades tienen set privado → usamos EF para setear por Entry (evita CS0272)
            var entry = ctx.Entry(d);

            entry.Property("IdProducto").CurrentValue = dto.IdProducto;
            entry.Property("Codigo").CurrentValue = codigo;
            entry.Property("Descripcion").CurrentValue = dto.Descripcion?.Trim() ?? string.Empty;
            entry.Property("Porcentaje").CurrentValue = dto.Porcentaje;

            // Por si en tu entidad las fechas tienen otra capitalización (he visto 'FechainicioUtc'…)
            var props = entry.Properties.Select(p => p.Metadata.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            entry.Property(props.Contains("FechaInicioUtc") ? "FechaInicioUtc" : "FechainicioUtc")
                 .CurrentValue = dto.FechaInicioUtc;

            entry.Property(props.Contains("FechaCaducidadUtc") ? "FechaCaducidadUtc" : "FechacaducidadUtc")
                 .CurrentValue = dto.FechaCaducidadUtc;

            ctx.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            using var ctx = new TPIContext();
            var d = ctx.Descuentos.Find(id);
            if (d == null) return false;
            ctx.Descuentos.Remove(d);
            ctx.SaveChanges();
            return true;
        }
        public IEnumerable<DescuentoDTO> GetVigentes(string? texto, DateTime ahoraUtc)
        {
            using var ctx = new Data.TPIContext();

            var q = from d in ctx.Descuentos
                    join p in ctx.Productos on d.IdProducto equals p.IdProducto
                    where d.FechaInicioUtc <= ahoraUtc && ahoraUtc <= d.FechaCaducidadUtc
                    select new DescuentoDTO
                    {
                        IdDescuento = d.IdDescuento,
                        IdProducto = d.IdProducto,
                        ProductoNombre = p.Nombre,
                        Codigo = d.Codigo,
                        Descripcion = d.Descripcion,
                        Porcentaje = d.Porcentaje,
                        FechaInicioUtc = d.FechaInicioUtc,
                        FechaCaducidadUtc = d.FechaCaducidadUtc
                    };

            if (!string.IsNullOrWhiteSpace(texto))
            {
                var term = texto.Trim();
                q = q.Where(x => EF.Functions.Like(x.ProductoNombre, $"%{term}%")
                              || EF.Functions.Like(x.Codigo, $"%{term}%"));
            }

            return q.OrderBy(x => x.FechaCaducidadUtc).ToList();
        }

        public DescuentoDTO? GetByCodigoVigente(string codigo)
        {
            using var ctx = new Data.TPIContext();
            var now = DateTime.UtcNow;
            var code = (codigo ?? "").Trim().ToUpperInvariant();

            var q = from d in ctx.Descuentos
                    where d.Codigo == code
                       && d.FechaInicioUtc <= now
                       && now <= d.FechaCaducidadUtc
                    select new DescuentoDTO
                    {
                        IdDescuento = d.IdDescuento,
                        IdProducto = d.IdProducto,
                        Codigo = d.Codigo,
                        Descripcion = d.Descripcion,
                        Porcentaje = d.Porcentaje,
                        FechaInicioUtc = d.FechaInicioUtc,
                        FechaCaducidadUtc = d.FechaCaducidadUtc
                    };

            return q.FirstOrDefault();
        }

    }
}
