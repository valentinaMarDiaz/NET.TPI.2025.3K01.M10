using System.ComponentModel.DataAnnotations; // <-- NECESARIO para los atributos de validación

namespace DTOs
{
    public class ProductoDTO
    {
        public int IdProducto { get; set; } // El ID no necesita validación aquí, lo maneja la BD/API

        [Required(ErrorMessage = "Debe seleccionar una categoría.")] // 1. Campo requerido
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")] // 2. El ID debe ser mayor a 0
        public int IdCatProducto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")] // 3. Campo requerido
        [StringLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")] // 4. Longitud máxima
        public string Nombre { get; set; } = "";

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres.")] // 5. Longitud máxima (opcional)
        public string Descripcion { get; set; } = "";

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")] // 6. Rango numérico (mínimo 0)
        public int Stock { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")] // 7. Rango numérico (mínimo > 0)
        public decimal PrecioActual { get; set; }

        // Esta propiedad es solo de lectura para mostrar en la grilla, no necesita validación de entrada
        public string? CategoriaNombre { get; set; }
    }
}