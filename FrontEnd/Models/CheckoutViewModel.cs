using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    // Este modelo se usa para capturar los datos del cliente/envío en la vista Finalizar.cshtml
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La dirección de envío es obligatoria.")]
        [Display(Name = "Dirección de Envío")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        public string Ciudad { get; set; }

        [Required(ErrorMessage = "El código postal es obligatorio.")]
        [Display(Name = "Código Postal")]
        public string CodigoPostal { get; set; }

    }
}