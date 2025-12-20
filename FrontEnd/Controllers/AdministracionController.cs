using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using FrontEnd.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace FrontEnd.Controllers
{
    public class AdministracionController : Controller
    {
        private readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"];
        private readonly HttpClient _httpClient = new HttpClient();

        // ==========================================
        // 1. DASHBOARD & INDEX
        // ==========================================
        [HttpGet]
        public ActionResult Index()
        {
            // Seguridad: Si no hay admin logueado, al Login
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");

            ViewBag.UserName = Session["AdminNombre"];

            // Forzamos la carga de "Dashboard.cshtml"
            return View("Dashboard");
        }

        // ==========================================
        // 2. GESTIÓN DE PRODUCTOS
        // ==========================================
        [HttpGet]
        public async Task<ActionResult> Productos()
        {
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");

            var productos = new List<Producto>();
            try
            {
                var response = await _httpClient.GetAsync(API_BASE_URL + "productos/obtener");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    productos = JsonConvert.DeserializeObject<List<Producto>>(content);
                }
            }
            catch
            {
                ViewBag.Error = "No se pudieron conectar los productos con la API.";
            }
            return View(productos);
        }

        [HttpGet]
        public async Task<ActionResult> RegistrarProducto()
        {
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");
            await CargarProveedoresEnViewBag();
            return View(new Producto { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrarProducto(Producto model)
        {
            // Eliminamos validaciones de campos que el sistema genera solo
            ModelState.Remove("FechaRegistro");
            ModelState.Remove("VentasAcumuladas");
            ModelState.Remove("ProductoID");

            if (ModelState.IsValid)
            {
                // ASIGNACIÓN AUTOMÁTICA DE DATOS
                model.FechaRegistro = DateTime.Now;
                model.VentasAcumuladas = 0;

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(API_BASE_URL + "productos/registrar", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Productos");
                }

                var errorApi = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "La API rechazó el registro: " + errorApi);
            }

            await CargarProveedoresEnViewBag();
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> EditarProducto(int id)
        {
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");
            await CargarProveedoresEnViewBag();

            var response = await _httpClient.GetAsync(API_BASE_URL + $"productos/obtener/{id}");
            if (response.IsSuccessStatusCode)
            {
                var product = JsonConvert.DeserializeObject<Producto>(await response.Content.ReadAsStringAsync());
                return View(product);
            }
            return RedirectToAction("Productos");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarProducto(Producto model)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await _httpClient.PutAsync(API_BASE_URL + $"productos/actualizar/{model.ProductoID}", content);
                return RedirectToAction("Productos");
            }
            await CargarProveedoresEnViewBag();
            return View(model);
        }

        // ==========================================
        // 3. GESTIÓN DE PROVEEDORES
        // ==========================================
        [HttpGet]
        public async Task<ActionResult> Proveedores()
        {
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");

            var lista = new List<ProveedorViewModel>();
            var response = await _httpClient.GetAsync(API_BASE_URL + "proveedores/obtener");
            if (response.IsSuccessStatusCode)
            {
                lista = JsonConvert.DeserializeObject<List<ProveedorViewModel>>(await response.Content.ReadAsStringAsync());
            }
            return View(lista);
        }

        // ==========================================
        // 4. VER ÓRDENES (ADMIN)
        // ==========================================
        [HttpGet]
        public async Task<ActionResult> VerOrdenes()
        {
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");

            var ordenes = new List<Dictionary<string, object>>();
            try
            {
                var response = await _httpClient.GetAsync(API_BASE_URL + "ordenes/obtener");
                if (response.IsSuccessStatusCode)
                {
                    ordenes = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(await response.Content.ReadAsStringAsync());
                }
            }
            catch { }

            return View(ordenes);
        }

        // ==========================================
        // 5. LOGOUT UNIVERSAL
        // ==========================================
        public ActionResult Logout()
        {
            // Limpia TODA la sesión (Admin y Usuario)
            Session.Clear();
            Session.Abandon();

            // Limpiar cookies de autenticación si existen
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            }

            return RedirectToAction("Index", "Home");
        }

        // ==========================================
        // MÉTODOS AUXILIARES
        // ==========================================
        private async Task CargarProveedoresEnViewBag()
        {
            try
            {
                var resp = await _httpClient.GetAsync(API_BASE_URL + "proveedores/obtener");
                if (resp.IsSuccessStatusCode)
                {
                    var lista = JsonConvert.DeserializeObject<List<ProveedorViewModel>>(await resp.Content.ReadAsStringAsync());
                    ViewBag.Proveedores = new SelectList(lista, "ProveedorID", "Nombre");
                }
            }
            catch
            {
                ViewBag.Proveedores = new SelectList(new List<ProveedorViewModel>(), "ProveedorID", "Nombre");
            }
        }
    }
}