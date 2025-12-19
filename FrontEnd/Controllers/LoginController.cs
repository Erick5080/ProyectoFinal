using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FrontEnd.Models;
using System.Net.Http;
using System.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Web.Security;

namespace FrontEnd.Controllers
{
    public class LoginController : Controller
    {
        private readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"];
        private readonly HttpClient _httpClient = new HttpClient();

        // GET: /Login/Index
        public ActionResult Index()
        {
            if (Session["AdminID"] != null) return RedirectToAction("Index", "Administracion");
            if (Session["UsuarioID"] != null) return RedirectToAction("Index", "Usuario");

            return View(new LoginModel());
        }

        // GET: /Login/Registro
        public ActionResult Registro()
        {
            // Retorna la vista vinculada a la clase Usuario (Email, Password, NombreCompleto)
            return View(new Usuario());
        }

        // POST: /Login/ProcesarLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProcesarLogin(LoginModel model, string UserType)
        {
            if (!ModelState.IsValid) return View("Index", model);

            // Determina endpoint: admin usa tabla con PasswordHash, cliente usa tabla Usuarios
            string endpoint = (UserType == "admin") ? "autenticacion/adminlogin" : "autenticacion/login";

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(model),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PostAsync(API_BASE_URL + endpoint, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(responseBody);

                    FormsAuthentication.SetAuthCookie(model.Email, false);

                    if (UserType == "admin")
                    {
                        Session["AdminID"] = (int)data.userId;
                        Session["AdminNombre"] = (string)data.nombre;
                        Session["AdminRol"] = (string)data.role;
                        return RedirectToAction("Index", "Administracion");
                    }
                    else
                    {
                        Session["UsuarioID"] = (int)data.usuarioId;
                        Session["UsuarioNombre"] = (string)data.nombre;
                        return RedirectToAction("Index", "Usuario");
                    }
                }

                ModelState.AddModelError("", "Credenciales inválidas o perfil incorrecto.");
                return View("Index", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error de conexión: " + ex.Message);
                return View("Index", model);
            }
        }

        // POST: /Login/ProcesarRegistro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProcesarRegistro(Usuario model)
        {
            if (!ModelState.IsValid) return View("Registro", model);

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(model),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                // Envía el objeto Usuario (NombreCompleto, Email, Password) a la API
                var response = await _httpClient.PostAsync(API_BASE_URL + "autenticacion/registrar", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "¡Registro exitoso! Ya puedes iniciar sesión.";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "No se pudo registrar. El correo podría ya estar en uso.");
                return View("Registro", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al conectar con la API: " + ex.Message);
                return View("Registro", model);
            }
        }

        // GET: /Login/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var myCookie = new HttpCookie(FormsAuthentication.FormsCookieName)
                {
                    Expires = DateTime.Now.AddDays(-1d)
                };
                Response.Cookies.Add(myCookie);
            }

            return RedirectToAction("Index", "Login");
        }
    }
}