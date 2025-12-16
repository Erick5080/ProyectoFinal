using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using API.Models;
using API.DAL;
using API.Repositories;

namespace API.Controllers
{
    [RoutePrefix("api/autenticacion")]
    public class AutenticacionController : ApiController
    {
        // Usamos el Repositorio ADO.NET
        private readonly AdministradorRepository _adminRepo = new AdministradorRepository();

        [HttpPost]
        [Route("adminlogin")]
        public IHttpActionResult AdminLogin(LoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Faltan credenciales.");
            }

            var admin = _adminRepo.GetAdminByCredentials(model.Email, model.Password);

            if (admin == null || !admin.Activo)
            {
                return Unauthorized();
            }

            return Ok(new { success = true, userId = admin.AdminID, role = admin.Rol, nombre = admin.Nombre });
        }
    }
}