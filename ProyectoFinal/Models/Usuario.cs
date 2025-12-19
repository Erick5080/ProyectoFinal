using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string NombreCompleto { get; set; }
    }
}