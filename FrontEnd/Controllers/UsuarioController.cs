using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json;
// Eliminamos la línea de API.Models.Orden que causaba error

namespace FrontEnd.Controllers // Cambiado a FrontEnd para coincidir con tu proyecto
{
    public class UsuarioController : Controller
    {
        private readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"];
        private readonly HttpClient _httpClient = new HttpClient();

        // GET: Usuario/Index
        public ActionResult Index()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.UserName = Session["UsuarioNombre"];
            return View();
        }

        // GET: Usuario/ObtenerMisOrdenes
        [HttpGet]
        public async Task<JsonResult> ObtenerMisOrdenes()
        {
            try
            {
                if (Session["UsuarioID"] == null)
                    return Json(new { error = "Sesión expirada" }, JsonRequestBehavior.AllowGet);

                int usuarioId = Convert.ToInt32(Session["UsuarioID"]);

                // Llamada a la API
                var response = await _httpClient.GetAsync($"{API_BASE_URL}autenticacion/ordenes/{usuarioId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    // Usamos 'dynamic' para no necesitar un modelo Orden.cs en el FrontEnd
                    var ordenes = JsonConvert.DeserializeObject<List<dynamic>>(content);

                    // Corregido: Usamos JsonRequestBehavior para el error CS0103
                    return Json(ordenes, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }

            return Json(new { error = "No se pudieron cargar las órdenes" }, JsonRequestBehavior.AllowGet);
        }
    }
}