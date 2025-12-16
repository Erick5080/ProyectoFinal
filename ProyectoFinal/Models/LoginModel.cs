using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

public class LoginModel
{
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Password { get; set; }
}