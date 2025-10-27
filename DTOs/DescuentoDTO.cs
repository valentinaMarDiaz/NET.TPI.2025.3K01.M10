namespace DTOs
{
    public class DescuentoDTO
    {
        public int IdDescuento { get; set; }
        public int IdProducto { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;

        public DateTime FechaInicioUtc { get; set; }
        public DateTime FechaCaducidadUtc { get; set; }

        public string Descripcion { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public decimal Porcentaje { get; set; }
    }

    public class DescuentoCUDTO 
    {
        public int IdDescuento { get; set; } 
        public int IdProducto { get; set; }
        public DateTime FechaInicioUtc { get; set; } = DateTime.UtcNow;
        public DateTime FechaCaducidadUtc { get; set; } = DateTime.UtcNow.AddDays(7);
        public string Descripcion { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public decimal Porcentaje { get; set; }
    }
}
