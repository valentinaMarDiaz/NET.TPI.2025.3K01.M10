using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Domain.Model;
using DTOs;

namespace Application.Services
{
    public class CategoriaService
    {
        private readonly CategoriaRepository _repo = new();

        public CategoriaDTO Add(CategoriaDTO dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre es requerido.", nameof(dto.Nombre));

            dto.Descripcion ??= string.Empty;

            if (_repo.NameExists(dto.Nombre))
                throw new ArgumentException($"Ya existe una categoría con el nombre '{dto.Nombre}'.");

            var entity = new CategoriaProducto(0, dto.Nombre.Trim(), dto.Descripcion.Trim());
            _repo.Add(entity);

            return new CategoriaDTO
            {
                IdCatProducto = entity.IdCatProducto,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion
            };
        }

        public bool Update(CategoriaDTO dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            if (dto.IdCatProducto <= 0) throw new ArgumentException("Id inválido.", nameof(dto.IdCatProducto));
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre es requerido.", nameof(dto.Nombre));

            dto.Descripcion ??= string.Empty;

            if (_repo.NameExists(dto.Nombre, dto.IdCatProducto))
                throw new ArgumentException($"Ya existe una categoría con el nombre '{dto.Nombre}'.");

            var entity = new CategoriaProducto(dto.IdCatProducto, dto.Nombre.Trim(), dto.Descripcion.Trim());
            return _repo.Update(entity);
        }

        public CategoriaDTO? Get(int id)
        {
            var c = _repo.Get(id);
            return c == null ? null : new CategoriaDTO
            {
                IdCatProducto = c.IdCatProducto,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion
            };
        }

        public IEnumerable<CategoriaDTO> GetAll()
        {
            return _repo.GetAll().Select(c => new CategoriaDTO
            {
                IdCatProducto = c.IdCatProducto,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion
            }).ToList();
        }

        public bool Delete(int id) => _repo.Delete(id);
    }
}
