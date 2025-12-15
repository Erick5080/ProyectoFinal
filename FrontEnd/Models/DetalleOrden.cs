using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class DetalleOrden
    {
        public int DetalleOrdenID { get; set; }
        public int OrdenID { get; set; }
        public int ProductoID { get; set; }

        [Display(Name = "Producto")]
        public string NombreProducto { get; set; }

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Precio Unitario")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal PrecioUnitario { get; set; }

        [Display(Name = "SubTotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal SubTotal { get; set; }
    }
}