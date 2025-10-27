using System.ComponentModel.DataAnnotations; 

namespace DTOs
{
    public class ProductoDTO
    {
        public int IdProducto { get; set; } 

        [Required(ErrorMessage = "Debe seleccionar una categoría.")] 
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")] 
        public int IdCatProducto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")] 
        [StringLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")] 
        public string Nombre { get; set; } = "";

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres.")] 
        public string Descripcion { get; set; } = "";

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")] 
        public int Stock { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")] 
        public decimal PrecioActual { get; set; }

        
        public string? CategoriaNombre { get; set; }
    }
}