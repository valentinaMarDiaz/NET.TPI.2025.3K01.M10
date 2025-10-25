using System.Text.RegularExpressions;

namespace Domain.Model
{
    public class Descuento
    {
        public int IdDescuento { get; private set; }
        public int IdProducto { get; private set; }

        public DateTime FechaInicioUtc { get; private set; }
        public DateTime FechaCaducidadUtc { get; private set; }

        public string Descripcion { get; private set; } = string.Empty;
        public string Codigo { get; private set; } = string.Empty; // Único, mayúsculas
        public decimal Porcentaje { get; private set; } // 0 < % ≤ 100

        public Descuento() { }

        public Descuento(int idDescuento, int idProducto, DateTime inicioUtc, DateTime caducaUtc,
                         string descripcion, string codigo, decimal porcentaje)
        {
            SetId(idDescuento);
            SetIdProducto(idProducto);
            SetFechas(inicioUtc, caducaUtc);
            SetDescripcion(descripcion);
            SetCodigo(codigo);
            SetPorcentaje(porcentaje);
        }

        public void SetId(int id) => IdDescuento = id;

        public void SetIdProducto(int idProducto)
        {
            if (idProducto <= 0) throw new ArgumentException("IdProducto inválido.");
            IdProducto = idProducto;
        }

        public void SetFechas(DateTime inicioUtc, DateTime caducaUtc)
        {
            if (inicioUtc.Kind != DateTimeKind.Utc || caducaUtc.Kind != DateTimeKind.Utc)
                throw new ArgumentException("Las fechas deben estar en UTC.");
            if (caducaUtc < inicioUtc) throw new ArgumentException("Fecha de caducidad no puede ser menor al inicio.");
            FechaInicioUtc = inicioUtc;
            FechaCaducidadUtc = caducaUtc;
        }

        public void SetDescripcion(string descripcion)
        {
            descripcion = (descripcion ?? "").Trim();
            if (descripcion.Length is < 1 or > 200)
                throw new ArgumentException("La descripción debe tener entre 1 y 200 caracteres.");
            Descripcion = descripcion;
        }

        public void SetCodigo(string codigo)
        {
            codigo = (codigo ?? "").Trim().ToUpperInvariant();
            if (codigo.Length is < 3 or > 24)
                throw new ArgumentException("El código debe tener entre 3 y 24 caracteres.");
            if (!Regex.IsMatch(codigo, @"^[A-Z0-9-]+$"))
                throw new ArgumentException("El código solo admite letras mayúsculas, números y guiones.");
            Codigo = codigo;
        }

        public void SetPorcentaje(decimal porcentaje)
        {
            if (porcentaje <= 0 || porcentaje > 100)
                throw new ArgumentException("El porcentaje debe ser > 0 y ≤ 100.");
            Porcentaje = decimal.Round(porcentaje, 2, MidpointRounding.AwayFromZero);
        }

        public bool EstaVigenteUtc(DateTime ahoraUtc)
            => ahoraUtc >= FechaInicioUtc && ahoraUtc <= FechaCaducidadUtc;
    }
}
