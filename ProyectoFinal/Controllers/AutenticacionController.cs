using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using API.Models;
using API.DAL;
using API.Repositories;
using System.Data;
using System.Data.SqlClient;

namespace API.Controllers
{
    [RoutePrefix("api/autenticacion")]
    public class AutenticacionController : ApiController
    {
        private readonly AdministradorRepository _adminRepo = new AdministradorRepository();
        private readonly DBHelper db = new DBHelper();

        // 1. LOGIN DE ADMINISTRADORES
        [HttpPost]
        [Route("adminlogin")]
        public IHttpActionResult AdminLogin(LoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email)) return BadRequest();

            // Busca en la tabla Administradores usando PasswordHash
            var admin = _adminRepo.GetAdminByCredentials(model.Email, model.Password);
            if (admin == null || !admin.Activo) return Unauthorized();

            return Ok(new { success = true, userId = admin.AdminID, role = admin.Rol, nombre = admin.Nombre });
        }

        // 2. REGISTRO DE USUARIOS (CORREGIDO)
        [HttpPost]
        [Route("registrar")]
        public IHttpActionResult Registrar([FromBody] Usuario usuario)
        {
            if (usuario == null) return BadRequest("Datos inválidos.");

            try
            {
                SqlParameter[] parameters = {
                    new SqlParameter("@Email", usuario.Email),
                    // Se usa .Password porque así se llama en tu clase Usuario
                    new SqlParameter("@PasswordHash", usuario.Password),
                    new SqlParameter("@NombreCompleto", usuario.NombreCompleto)
                };

                // Ejecuta el procedimiento almacenado para insertar en la tabla Usuarios
                db.ExecuteDataTable("PA_RegistrarUsuario", parameters);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Error SQL: " + ex.Message));
            }
        }

        // 3. LOGIN DE USUARIOS NORMALES
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(LoginModel model)
        {
            if (model == null) return BadRequest();

            try
            {
                SqlParameter[] parameters = {
                    new SqlParameter("@Email", model.Email),
                    new SqlParameter("@Password", model.Password)
                };

                // Valida contra la tabla dbo.Usuarios
                DataTable dt = db.ExecuteDataTable("PA_ValidarUsuario", parameters);

                if (dt.Rows.Count > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        usuarioId = Convert.ToInt32(dt.Rows[0]["UsuarioID"]),
                        nombre = dt.Rows[0]["NombreCompleto"].ToString()
                    });
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}