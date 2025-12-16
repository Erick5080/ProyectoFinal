using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using BackEnd.Models; 
using System.Linq;
public class AutenticacionController : ApiController
{
    private readonly TuDbContext _contexto = new TuDbContext(); // Reemplaza por tu contexto de BD

    [HttpPost]
    [Route("api/autenticacion/adminlogin")]
    public IHttpActionResult AdminLogin(LoginModel model) // Necesitas un modelo LoginModel en tu API
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
        {
            return BadRequest("Faltan credenciales.");
        }

        // Por simplicidad, buscamos texto plano para el ejemplo de prueba:
        var admin = _contexto.Administradores
                             .FirstOrDefault(a => a.Email == model.Email &&
                                                  a.PasswordHash == model.Password && // Usamos PasswordHash (que es 'contraseña' para el test)
                                                  a.Activo == true);

        if (admin == null)
        {
            return Unauthorized(); // 401: Credenciales inválidas
        }

        // En producción, aquí se generaría un Token JWT.
        return Ok(new { success = true, userId = admin.AdminID, role = admin.Rol });
    }
}