using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Orden
    {
        public int OrdenID { get; set; }
        public string NombreCliente { get; set; }
        public string Email { get; set; }
        public string DireccionEnvio { get; set; }
        public decimal Total { get; set; }
        public ICollection<DetalleOrden> Detalles { get; set; }
    }
}