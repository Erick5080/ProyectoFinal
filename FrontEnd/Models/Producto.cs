using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FrontEnd.Models
{
    // Clase para la obtencion de datos de productos del API
    public class Producto
    {

        [Required]
        [Display(Name = "ID de Producto")]
        public int ProductoID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre del Producto")]
        public string Nombre { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio Unitario")]
        // Usamos decimal para precisión en el dinero.
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, 99999, ErrorMessage = "El stock debe ser un valor positivo.")]
        public int Stock { get; set; }


        // Manejo de Relación

        [Required(ErrorMessage = "El ID del proveedor es obligatorio.")]
        [Display(Name = "ID Proveedor")]
        public int ProveedorID { get; set; }

        [Display(Name = "URL de Imagen")]
        [StringLength(500)]
        [DataType(DataType.ImageUrl)]
        public string ImagenURL { get; set; }

        // Propiedad para la eliminación lógica
        public bool Activo { get; set; }
    }
}