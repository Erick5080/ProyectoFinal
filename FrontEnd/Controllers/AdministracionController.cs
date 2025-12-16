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
using System.Net;

namespace FrontEnd.Controllers
{
    // Aseguramos que solo el administrador pueda acceder a estas rutas
    [Authorize(Roles = "Admin")]
    public class AdministracionController : Controller
    {
        private readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"];
        private readonly HttpClient _httpClient = new HttpClient();

        // GET: /Administracion/Dashboard
        [HttpGet]
        public ActionResult Dashboard()
        {
            ViewBag.Message = "Bienvenido al Panel de Administración.";
            ViewBag.UserName = User.Identity.Name;
            return View();
        }

        // GESTIÓN DE PROVEEDORES

        // GET: /Administracion/Proveedores (Listado)
        [HttpGet]
        public async Task<ActionResult> Proveedores()
        {
            ViewBag.Title = "Gestión de Proveedores";

            var response = await _httpClient.GetAsync(API_BASE_URL + "proveedores/obtener");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var proveedores = JsonConvert.DeserializeObject<List<ProveedorViewModel>>(content);
                return View(proveedores);
            }

            ViewBag.Error = "No se pudieron cargar los proveedores de la API.";
            return View(new List<ProveedorViewModel>());
        }

        // GET: /Administracion/CrearProveedor
        [HttpGet]
        public ActionResult CrearProveedor()
        {
            ViewBag.Title = "Crear Nuevo Proveedor";
            return View(new ProveedorViewModel { Activo = true });
        }

        // POST: /Administracion/CrearProveedor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearProveedor(ProveedorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(model),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(API_BASE_URL + "proveedores/registrar", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Proveedor creado con éxito.";
                    return RedirectToAction("Proveedores");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Error al guardar el proveedor: {errorContent}");
            }

            ViewBag.Title = "Crear Nuevo Proveedor";
            return View(model);
        }

        // GET: /Administracion/EditarProveedor/{id}
        [HttpGet]
        public async Task<ActionResult> EditarProveedor(int id)
        {
            ViewBag.Title = "Editar Proveedor";
            var response = await _httpClient.GetAsync(API_BASE_URL + $"proveedores/obtener/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var proveedor = JsonConvert.DeserializeObject<ProveedorViewModel>(content);
                return View(proveedor);
            }

            TempData["ErrorMessage"] = "Proveedor no encontrado o error de API.";
            return RedirectToAction("Proveedores");
        }

        // POST: /Administracion/EditarProveedor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarProveedor(ProveedorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(model),
                    Encoding.UTF8,
                    "application/json"
                );

                // Usamos PUT para actualizar
                var response = await _httpClient.PutAsync(API_BASE_URL + $"proveedores/actualizar/{model.ProveedorID}", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Proveedor actualizado con éxito.";
                    return RedirectToAction("Proveedores");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Error al actualizar el proveedor: {errorContent}");
            }
            ViewBag.Title = "Editar Proveedor";
            return View(model);
        }

        // GET: /Administracion/EliminarProveedor/{id}
        // Usamos GET para la acción simple de eliminar
        [HttpGet]
        public async Task<ActionResult> EliminarProveedor(int id)
        {
            var response = await _httpClient.DeleteAsync(API_BASE_URL + $"proveedores/eliminar/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Proveedor eliminado con éxito.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Error al eliminar el proveedor: {errorContent}";
            }

            return RedirectToAction("Proveedores");
        }

        // GESTIÓN DE PRODUCTOS

        [HttpGet]
        public async Task<ActionResult> Productos()
        {
            ViewBag.Message = "Panel de Administración de Productos";

            try
            {
                var response = await _httpClient.GetAsync(API_BASE_URL + "productos/obtener");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var productos = JsonConvert.DeserializeObject<List<Producto>>(content);
                    return View(productos);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return View(new List<Producto>());
                }
                else
                {
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
        public async Task<ActionResult> RegistrarProducto() // Ahora debe ser async para cargar proveedores
        {
            ViewBag.Proveedores = await ObtenerListaProveedores(); // Cargar la lista para el dropdown
            return View(new Producto { Activo = true, PrecioUnitario = 0.00M, Stock = 0 });
        }

        // Envía el nuevo producto a la API para ser guardado.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrarProducto(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Proveedores = await ObtenerListaProveedores(); // Recargar si falla la validación
                return View(producto);
            }

            try
            {
                var jsonContent = JsonConvert.SerializeObject(producto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(API_BASE_URL + "productos/registrar", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Producto registrado exitosamente!";
                    return RedirectToAction("Productos"); // Corregido de Index a Productos
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error al registrar el producto. Código: {response.StatusCode}. Detalle: {errorContent}");
                    ViewBag.Proveedores = await ObtenerListaProveedores();
                    return View(producto);
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Error de conexión con la API: {ex.Message}");
                ViewBag.Proveedores = await ObtenerListaProveedores();
                return View(producto);
            }
        }

        // EditarProducto (GET)
        [HttpGet]
        public async Task<ActionResult> EditarProducto(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync(API_BASE_URL + $"productos/obtener/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var producto = JsonConvert.DeserializeObject<Producto>(content);

                    ViewBag.Proveedores = await ObtenerListaProveedores(); // Cargar lista de proveedores
                    return View(producto);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = $"Producto ID {id} no encontrado.";
                    return RedirectToAction("Productos");
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error al cargar producto: Código {response.StatusCode}.";
                    return RedirectToAction("Productos");
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Error de conexión con la API: {ex.Message}";
                return RedirectToAction("Productos");
            }
        }

        // EditarProducto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarProducto(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Proveedores = await ObtenerListaProveedores(); // Recargar si falla la validación
                return View(producto);
            }

            try
            {
                var jsonContent = JsonConvert.SerializeObject(producto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(API_BASE_URL + $"productos/actualizar/{producto.ProductoID}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Producto '{producto.Nombre}' actualizado exitosamente!";
                    return RedirectToAction("Productos"); // Corregido de Index a Productos
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error al actualizar. Código: {response.StatusCode}. Detalle: {errorContent}");
                    ViewBag.Proveedores = await ObtenerListaProveedores();
                    return View(producto);
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Error de conexión con la API: {ex.Message}");
                ViewBag.Proveedores = await ObtenerListaProveedores();
                return View(producto);
            }
        }

        // FUNCIÓN AUXILIAR
        // Función auxiliar para obtener proveedores de la API y prepararlos para el Dropdown
        private async Task<SelectList> ObtenerListaProveedores()
        {
            try
            {
                var response = await _httpClient.GetAsync(API_BASE_URL + "proveedores/obtener");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // NOTA: Asegúrate de que este modelo Proveedor exista para la función auxiliar
                    var proveedores = JsonConvert.DeserializeObject<List<Proveedor>>(content);

                    // Crear SelectList para el Dropdown
                    return new SelectList(proveedores, "ProveedorID", "Nombre");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener proveedores: {ex.Message}");
            }
            // Retorna lista vacía en caso de error
            return new SelectList(new List<Proveedor>(), "ProveedorID", "Nombre");
        }

        // GESTIÓN DE ÓRDENES

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
                    // Usamos Dictionary<string, object> o un modelo 'Orden' específico
                    var ordenes = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content);

                    return View(ordenes);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
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

        [HttpGet]
        public async Task<ActionResult> VerDetalles(int id)
        {
            try
            {
                // 1. Obtener Encabezado de la Orden
                var headerResponse = await _httpClient.GetAsync(API_BASE_URL + $"ordenes/obtener/{id}");
                if (!headerResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"Orden ID {id} no encontrada o error al cargar encabezado.";
                    return RedirectToAction("VerOrdenes");
                }
                var headerContent = await headerResponse.Content.ReadAsStringAsync();
                var ordenHeader = JsonConvert.DeserializeObject<Dictionary<string, object>>(headerContent);

                // 2. Obtener Detalles de la Orden
                var detailsResponse = await _httpClient.GetAsync(API_BASE_URL + $"ordenes/detalles/{id}");

                List<DetalleOrden> detalles = new List<DetalleOrden>();
                if (detailsResponse.IsSuccessStatusCode)
                {
                    var detailsContent = await detailsResponse.Content.ReadAsStringAsync();
                    detalles = JsonConvert.DeserializeObject<List<DetalleOrden>>(detailsContent);
                }
                else
                {
                    ModelState.AddModelError("", "Advertencia: No se pudieron cargar los productos (detalles) de la orden.");
                }

                var viewModel = new OrdenDetallesViewModel
                {
                    Orden = ordenHeader,
                    Detalles = detalles
                };

                ViewBag.Title = $"Detalles de Orden #{id}";
                return View(viewModel);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Error de conexión con la API: {ex.Message}";
                return RedirectToAction("VerOrdenes");
            }
        }
    }
}