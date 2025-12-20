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
        // 1. DASHBOARD
        // ==========================================
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");
            ViewBag.UserName = Session["AdminNombre"];
            return View("Dashboard");
        }

        // ==========================================
        // 2. GESTIÓN DE PROVEEDORES
        // ==========================================
        [HttpGet]
        public async Task<ActionResult> Proveedores()
        {
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");
            var proveedores = new List<ProveedorViewModel>();
            try
            {
                var response = await _httpClient.GetAsync(API_BASE_URL + "proveedores/obtener");
                if (response.IsSuccessStatusCode)
                {
                    proveedores = JsonConvert.DeserializeObject<List<ProveedorViewModel>>(await response.Content.ReadAsStringAsync());
                }
            }
            catch { }
            return View(proveedores);
        }

        [HttpGet]
        public ActionResult CrearProveedor()
        {
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");
            return View(new ProveedorViewModel { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearProveedor(ProveedorViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync(API_BASE_URL + "proveedores/registrar", content);
            return RedirectToAction("Proveedores");
        }

        [HttpGet]
        public async Task<ActionResult> EditarProveedor(int id)
        {
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");
            var response = await _httpClient.GetAsync(API_BASE_URL + $"proveedores/obtener/{id}");
            var prov = JsonConvert.DeserializeObject<ProveedorViewModel>(await response.Content.ReadAsStringAsync());
            return View(prov);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarProveedor(ProveedorViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PutAsync(API_BASE_URL + $"proveedores/actualizar/{model.ProveedorID}", content);
            return RedirectToAction("Proveedores");
        }

        // ==========================================
        // 3. GESTIÓN DE PRODUCTOS
        // ==========================================
        [HttpGet]
        public async Task<ActionResult> Productos()
        {
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");
            var productos = new List<Producto>();
            var response = await _httpClient.GetAsync(API_BASE_URL + "productos/obtener");
            if (response.IsSuccessStatusCode)
            {
                productos = JsonConvert.DeserializeObject<List<Producto>>(await response.Content.ReadAsStringAsync());
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
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync(API_BASE_URL + "productos/registrar", content);
                return RedirectToAction("Productos");
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
            var product = JsonConvert.DeserializeObject<Producto>(await response.Content.ReadAsStringAsync());
            return View(product);
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

        [HttpPost]
        public async Task<ActionResult> DesactivarProducto(int id)
        {
            var response = await _httpClient.DeleteAsync(API_BASE_URL + $"productos/eliminar/{id}");
            return Json(new { success = response.IsSuccessStatusCode });
        }

        // ==========================================
        // 4. GESTIÓN DE ÓRDENES
        // ==========================================
        [HttpGet]
        public async Task<ActionResult> VerOrdenes()
        {
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");
            var response = await _httpClient.GetAsync(API_BASE_URL + "ordenes/obtener");
            var ordenes = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(await response.Content.ReadAsStringAsync());
            return View(ordenes);
        }

        [HttpGet]
        public async Task<ActionResult> VerDetalles(int id)
        {
            if (Session["AdminID"] == null) return RedirectToAction("Index", "Login");

            var viewModel = new OrdenDetallesViewModel
            {
                Orden = new Dictionary<string, object>(),
                Detalles = new List<DetalleOrden>()
            };

            var respOrden = await _httpClient.GetAsync(API_BASE_URL + $"ordenes/obtener/{id}");
            if (respOrden.IsSuccessStatusCode)
                viewModel.Orden = JsonConvert.DeserializeObject<Dictionary<string, object>>(await respOrden.Content.ReadAsStringAsync());

            var respDetalles = await _httpClient.GetAsync(API_BASE_URL + $"ordenes/detalles/{id}");
            if (respDetalles.IsSuccessStatusCode)
                viewModel.Detalles = JsonConvert.DeserializeObject<List<DetalleOrden>>(await respDetalles.Content.ReadAsStringAsync());

            return View(viewModel);
        }

        // ==========================================
        // 5. SESIÓN Y AUXILIARES
        // ==========================================
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        private async Task CargarProveedoresEnViewBag()
        {
            var resp = await _httpClient.GetAsync(API_BASE_URL + "proveedores/obtener");
            if (resp.IsSuccessStatusCode)
            {
                var lista = JsonConvert.DeserializeObject<List<ProveedorViewModel>>(await resp.Content.ReadAsStringAsync());
                ViewBag.Proveedores = new SelectList(lista, "ProveedorID", "Nombre");
            }
        }
    }
}