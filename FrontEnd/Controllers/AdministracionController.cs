using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Configuration;
using FrontEnd.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace FrontEnd.Controllers
{
    public class AdministracionController : Controller
    {
        private readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"];
        private readonly HttpClient _httpClient = new HttpClient();

        // (Panel de Listado de Productos)
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = "Panel de Administración de Productos";

            try
            {
                // Llamar al endpoint que obtiene todos los productos
                var response = await _httpClient.GetAsync(API_BASE_URL + "productos/obtener");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // Deserializar la lista de productos
                    var productos = JsonConvert.DeserializeObject<List<Producto>>(content);

                    // Pasar la lista a la vista
                    return View(productos);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // No hay productos, devolver una lista vacía
                    return View(new List<Producto>());
                }
                else
                {
                    // Manejo de otros errores HTTP
                    ModelState.AddModelError("", $"Error al cargar productos de la API. Código: {response.StatusCode}");
                    return View(new List<Producto>());
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Error de conexión con la API: {ex.Message}");
                return View(new List<Producto>());
            }
        }


        // Muestra el formulario vacío para registrar un nuevo producto.
        [HttpGet]
        public ActionResult RegistrarProducto()
        {
            // Creamos un modelo vacío para que la vista lo use
            return View(new Producto { Activo = true, PrecioUnitario = 0.00M, Stock = 0 });
        }

        // Envía el nuevo producto a la API para ser guardado.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrarProducto(Producto producto)
        {
            // 1. Validar el modelo
            if (!ModelState.IsValid)
            {
                return View(producto); // Vuelve a mostrar el formulario con errores de validación
            }

            try
            {
                // 2. Serializar el objeto Producto a JSON
                var jsonContent = JsonConvert.SerializeObject(producto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 3. Llamar al endpoint POST de la API
                var response = await _httpClient.PostAsync(API_BASE_URL + "productos/registrar", content);

                // 4. Procesar la respuesta
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Producto registrado exitosamente!";
                    return RedirectToAction("Index"); // Redirige al panel principal
                }
                else
                {
                    // Manejar errores de la API
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error al registrar el producto. Código: {response.StatusCode}. Detalle: {errorContent}");
                    return View(producto);
                }
            }
            catch (HttpRequestException ex)
            {
                // Manejar errores de conexión (API caída)
                ModelState.AddModelError("", $"Error de conexión con la API: {ex.Message}");
                return View(producto);
            }
        }
        [HttpGet]
        public async Task<ActionResult> EditarProducto(int id)
        {
            try
            {
                // 1. Llamar al endpoint GET de la API para obtener el producto
                var response = await _httpClient.GetAsync(API_BASE_URL + $"productos/obtener/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // 2. Deserializar el objeto Producto
                    var producto = JsonConvert.DeserializeObject<Producto>(content);

                    // 3. Pasar el modelo a la vista para pre-llenar el formulario
                    return View(producto);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = $"Producto ID {id} no encontrado.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error al cargar producto: Código {response.StatusCode}.";
                    return RedirectToAction("Index");
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Error de conexión con la API: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Administracion/EditarProducto (Guardar Cambios)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarProducto(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                return View(producto); // Volver a mostrar el formulario con errores de validación
            }

            try
            {
                // 1. Serializar el objeto Producto a JSON
                var jsonContent = JsonConvert.SerializeObject(producto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 2. Llamar al endpoint PUT de la API
                var response = await _httpClient.PutAsync(API_BASE_URL + $"productos/actualizar/{producto.ProductoID}", content);

                // 3. Procesar la respuesta
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Producto '{producto.Nombre}' actualizado exitosamente!";
                    return RedirectToAction("Index"); // Redirige al panel principal
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error al actualizar. Código: {response.StatusCode}. Detalle: {errorContent}");
                    return View(producto);
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Error de conexión con la API: {ex.Message}");
                return View(producto);
            }
        }
        [HttpGet]
        public async Task<ActionResult> VerOrdenes()
        {
            ViewBag.Message = "Listado de Órdenes de Compra";

            try
            {
                // 1. Llamar al endpoint que obtiene todas las órdenes
                var response = await _httpClient.GetAsync(API_BASE_URL + "ordenes/obtener");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var ordenes = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content);

                    // 2. Pasar la lista a la vista
                    return View(ordenes);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.InfoMessage = "Aún no hay órdenes registradas.";
                    return View(new List<Dictionary<string, object>>());
                }
                else
                {
                    ModelState.AddModelError("", $"Error al cargar órdenes de la API. Código: {response.StatusCode}");
                    return View(new List<Dictionary<string, object>>());
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Error de conexión con la API: {ex.Message}");
                return View(new List<Dictionary<string, object>>());
            }
        }
    }
}