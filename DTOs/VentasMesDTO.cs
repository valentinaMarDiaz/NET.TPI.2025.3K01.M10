namespace DTOs
{
    public sealed class VentasMesDTO
    {
        public int Mes { get; set; }        // 1..12
        public decimal Total { get; set; }  // suma del mes
    }
}
