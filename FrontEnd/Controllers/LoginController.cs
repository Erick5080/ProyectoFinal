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

        // GET: /Login/Index (Muestra el formulario de login)
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Si ya está autenticado, redirigir al panel
                return RedirectToAction("Dashboard", "Admin");
            }
            return View(new LoginModel());
        }

        // POST: /Login/Index (Procesa el formulario)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. Enviar credenciales a la API para validación
            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(model),
                Encoding.UTF8,
                "application/json"
            );

            // Llama al endpoint de la API que creamos
            var response = await _httpClient.PostAsync(API_BASE_URL + "autenticacion/adminlogin", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // 2. Autenticación exitosa: Establecer la cookie de autenticación
                FormsAuthentication.SetAuthCookie(model.Email, false);

                // 3. Redirigir al panel o a la URL solicitada
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Dashboard", "Admin"); // Redirigir al futuro panel de administración
                }
            }
            else
            {
                // 4. Autenticación fallida
                ModelState.AddModelError("", "Credenciales de administrador inválidas o API inaccesible.");
                return View(model);
            }
        }

        // POST: /Login/Logout
        [Authorize] // Solo se puede ejecutar si el usuario está logueado
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}