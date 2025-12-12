using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoFinal.Models
{
    public class DetalleOrden
    {
      public int ProductoID { get; set; }
      public int Cantidad { get; set; }
      public decimal PrecioUnitarioEnVenta { get; set; }
      public decimal SubTotal { get; set; }
    }

    public class OrdenRequest
    {
      public int? ClienteID { get; set; } 
      public List<DetalleOrden> Detalles { get; set; }
    }
}
