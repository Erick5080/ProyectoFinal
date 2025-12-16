using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class CarritoItem
    {
        // El ID del producto de la base de datos
        public int ProductoID { get; set; }

        // El nombre y precio son necesarios para mostrar en la vista
        [Display(Name = "Producto")]
        public string Nombre { get; set; }

        [Display(Name = "Precio Unitario")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal PrecioUnitario { get; set; }

        // Cantidad seleccionada por el usuario
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        // Propiedad de solo lectura para calcular el subtotal
        [Display(Name = "Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Subtotal
        {
            get { return PrecioUnitario * Cantidad; }
        }
    }
}