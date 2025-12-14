using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Producto
    {
        public int ProductoID { get; set; }

        // Mapeo de campos existentes en el FrontEnd:
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Stock { get; set; }
        public int ProveedorID { get; set; }
        public string ImagenURL { get; set; }
        public bool Activo { get; set; }

        public DateTime FechaRegistro { get; set; }

        public int VentasAcumuladas { get; set; }
    }
}