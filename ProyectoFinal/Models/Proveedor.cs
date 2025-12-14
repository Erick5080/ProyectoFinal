using System;

namespace API.Models
{
    // Clase que representa la entidad Proveedor en la base de datos
    public class Proveedor
    {
        public int ProveedorID { get; set; }
        public string Nombre { get; set; }
        public string ContactoNombre { get; set; } // Nombre del contacto dentro de la empresa
        public string Telefono { get; set; }
        public string Email { get; set; }

    }
}