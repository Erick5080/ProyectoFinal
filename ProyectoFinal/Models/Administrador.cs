using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

public class Administrador
{
    [Key]
    public int AdminID { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    // Campo usado en AutenticacionController para decidir si puede ingresar
    public bool Activo { get; set; } = true;

    // Puede ser "Admin", "SuperAdmin", etc.
    public string Rol { get; set; }
}