using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEnd.Models
{
    public class OrdenDetallesViewModel
    {
        public Dictionary<string, object> Orden { get; set; }
        public List<DetalleOrden> Detalles { get; set; }
    }
}