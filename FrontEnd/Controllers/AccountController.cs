using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FrontEnd.Models;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Claims;
using System.Threading.Tasks;
namespace FrontEnd.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login (Muestra el formulario)
        [HttpGet]
        [AllowAnonymous] // Permitir acceso sin estar logeado
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login (Procesa el formulario)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // SIMULACIÓN DE AUTENTICACIÓN:
            // 1. Verificar credenciales. En una app real, llamarías a la API aquí.
            bool isAuthenticated = (model.Email == "admin@store.com" && model.Password == "Password123");

            if (isAuthenticated)
            {
                string userRole = "Admin"; // Asignamos el rol fijo si la simulación es exitosa

                // 2. Establecer la cookie de autenticación (la sesión del usuario)
                // Esto crea la cookie que el sistema de Autorización usará.
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    model.Email,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(30),
                    false, // No persistente
                    userRole // Guardamos el rol aquí
                );

                string encTicket = FormsAuthentication.Encrypt(ticket);
                System.Web.HttpCookie faCookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Cookies.Add(faCookie);

                // 3. Redirigir al usuario al área protegida o a la URL que intentaba acceder.
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    // Redirige al inicio del panel de Administración
                    return RedirectToAction("VerOrdenes", "Administracion");
                }
            }
            else
            {
                // Error de credenciales
                ModelState.AddModelError("", "Intento de inicio de sesión no válido. Verifique sus credenciales.");
                return View(model);
            }
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}