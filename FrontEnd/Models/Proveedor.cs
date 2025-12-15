using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class Proveedor
    {
        public int ProveedorID { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [Display(Name = "Nombre de Proveedor")]
        public string Nombre { get; set; }

        [Display(Name = "Nombre de Contacto")]
        public string ContactoNombre { get; set; }

        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "Formato de teléfono inválido.")]
        public string Telefono { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; }
    }
}