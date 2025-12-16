using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class ProveedorViewModel
    {
        public int ProveedorID { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre de la Empresa")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El nombre de contacto es obligatorio.")]
        [Display(Name = "Persona de Contacto")]
        public string ContactoNombre { get; set; }

        [Phone(ErrorMessage = "Formato de teléfono inválido.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }
    }
}