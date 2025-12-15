using FrontEnd.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace FrontEnd.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProveedoresController : Controller
    {
        private readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"];
        private readonly HttpClient _httpClient = new HttpClient();

        // GET: Proveedores/Index (Listado)
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Gestión de Proveedores";
            try
            {
                var response = await _httpClient.GetAsync(API_BASE_URL + "proveedores/obtener");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var proveedores = JsonConvert.DeserializeObject<List<Proveedor>>(content);
                    return View(proveedores);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.InfoMessage = "No hay proveedores registrados.";
                    return View(new List<Proveedor>());
                }
                else
                {
                    ModelState.AddModelError("", $"Error al cargar proveedores de la API. Código: {response.StatusCode}");
                    return View(new List<Proveedor>());
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Error de conexión con la API: {ex.Message}");
                return View(new List<Proveedor>());
            }
        }

        // GET: Proveedores/Registrar
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.Title = "Registrar Nuevo Proveedor";
            return View(new Proveedor());
        }

        // POST: Proveedores/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Registrar(Proveedor proveedor)
        {
            if (!ModelState.IsValid)
            {
                return View(proveedor);
            }

            try
            {
                var jsonContent = JsonConvert.SerializeObject(proveedor);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(API_BASE_URL + "proveedores/registrar", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Proveedor '{proveedor.Nombre}' registrado exitosamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error al registrar. Código: {response.StatusCode}. Detalle: {errorContent}");
                    return View(proveedor);
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Error de conexión con la API: {ex.Message}");
                return View(proveedor);
            }
        }
        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            try
            {
                // 1. Llamar al endpoint GET de la API para obtener el proveedor
                var response = await _httpClient.GetAsync(API_BASE_URL + $"proveedores/obtener/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var proveedor = JsonConvert.DeserializeObject<Proveedor>(content);

                    // 2. Pasar el modelo a la vista para pre-llenar
                    return View(proveedor);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = $"Proveedor ID {id} no encontrado.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error al cargar proveedor: Código {response.StatusCode}.";
                    return RedirectToAction("Index");
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Error de conexión con la API: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Proveedores/Editar (Guardar Cambios)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(Proveedor proveedor)
        {
            if (!ModelState.IsValid)
            {
                return View(proveedor);
            }

            try
            {
                // 1. Serializar el objeto Proveedor a JSON
                var jsonContent = JsonConvert.SerializeObject(proveedor);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 2. Llamar al endpoint PUT de la API
                // Usamos PUT api/proveedores/actualizar/{id}
                var response = await _httpClient.PutAsync(API_BASE_URL + $"proveedores/actualizar/{proveedor.ProveedorID}", content);

                // 3. Procesar la respuesta
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Proveedor '{proveedor.Nombre}' actualizado exitosamente!";
                    return RedirectToAction("Index"); // Redirige al listado
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error al actualizar. Código: {response.StatusCode}. Detalle: {errorContent}");
                    return View(proveedor);
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Error de conexión con la API: {ex.Message}");
                return View(proveedor);
            }
        }
    }
}