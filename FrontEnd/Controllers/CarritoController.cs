using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FrontEnd.Models;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace FrontEnd.Controllers
{
    public class CarritoController : Controller
    {
        private const string SESSION_KEY = "CarritoSesion";
        private readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"];
        private readonly HttpClient _httpClient;

        public CarritoController()
        {
            _httpClient = new HttpClient();
            // Asegúrate que en Web.config la URL termine en / (ej: https://localhost:44389/api/)
            _httpClient.BaseAddress = new Uri(API_BASE_URL);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // FUNCIÓN AUXILIAR: Obtener Carrito de la Sesión
        private List<CarritoItem> ObtenerCarrito()
        {
            if (Session[SESSION_KEY] == null)
            {
                return new List<CarritoItem>();
            }
            var serializer = new JavaScriptSerializer();
            try
            {
                return serializer.Deserialize<List<CarritoItem>>(Session[SESSION_KEY].ToString());
            }
            catch
            {
                Session.Remove(SESSION_KEY);
                return new List<CarritoItem>();
            }
        }

        // FUNCIÓN AUXILIAR: Guardar Carrito en la Sesión
        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            var serializer = new JavaScriptSerializer();
            Session[SESSION_KEY] = serializer.Serialize(carrito);
        }

        // GET: /Carrito/Index
        public ActionResult Index()
        {
            var carrito = ObtenerCarrito();
            ViewBag.CarritoCount = carrito.Sum(i => i.Cantidad);
            return View(carrito);
        }

        // POST: /Carrito/Agregar
        [HttpPost]
        public async Task<JsonResult> Agregar(int productoId, int cantidad)
        {
            var carrito = ObtenerCarrito();
            var itemExistente = carrito.FirstOrDefault(i => i.ProductoID == productoId);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                try
                {
                    // CAMBIO CLAVE: Ruta corregida a "obtener/{id}" para coincidir con tu API
                    var response = await _httpClient.GetAsync($"productos/obtener/{productoId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        // Deserializamos dinámicamente para mayor flexibilidad
                        var productoDetalle = JsonConvert.DeserializeObject<dynamic>(content);

                        // Mapeo defensivo: verifica minúsculas y mayúsculas según lo que devuelva tu API
                        string nombre = productoDetalle.Nombre ?? productoDetalle.nombre ?? $"Producto {productoId}";
                        decimal precio = productoDetalle.PrecioUnitario ?? productoDetalle.precioUnitario ?? 0m;

                        carrito.Add(new CarritoItem
                        {
                            ProductoID = productoId,
                            Nombre = nombre,
                            PrecioUnitario = precio,
                            Cantidad = cantidad
                        });
                    }
                    else
                    {
                        // Si la API falla, notificamos al usuario en lugar de usar datos de prueba
                        return Json(new { success = false, message = $"No se encontró el producto en la base de datos (Error API: {response.StatusCode})" });
                    }
                }
                catch (Exception ex)
                {
                    // Error de conexión física con el puerto 44389
                    return Json(new { success = false, message = "Error de conexión con el servidor: " + ex.Message });
                }
            }

            GuardarCarrito(carrito);

            return Json(new
            {
                success = true,
                count = carrito.Sum(i => i.Cantidad)
            });
        }

        // POST: /Carrito/ActualizarCantidad
        [HttpPost]
        public JsonResult ActualizarCantidad(int productoId, int cantidad)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(i => i.ProductoID == productoId);

            if (item != null)
            {
                if (cantidad > 0)
                {
                    item.Cantidad = cantidad;
                }
                else
                {
                    carrito.Remove(item);
                }

                GuardarCarrito(carrito);

                var newTotal = carrito.Sum(i => i.Subtotal);
                return Json(new
                {
                    success = true,
                    newCount = carrito.Sum(i => i.Cantidad),
                    newTotalText = newTotal.ToString("C"),
                    newItemSubtotalText = item.Subtotal.ToString("C")
                });
            }
            return Json(new { success = false, message = "Producto no encontrado." });
        }

        // POST: /Carrito/Eliminar
        [HttpPost]
        public JsonResult Eliminar(int productoId)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(i => i.ProductoID == productoId);

            if (item != null)
            {
                carrito.Remove(item);
                GuardarCarrito(carrito);

                return Json(new
                {
                    success = true,
                    newCount = carrito.Sum(i => i.Cantidad),
                    newTotalText = carrito.Sum(i => i.Subtotal).ToString("C")
                });
            }
            return Json(new { success = false });
        }

        // GET: /Carrito/Finalizar
        public ActionResult Finalizar()
        {
            var carrito = ObtenerCarrito();
            if (!carrito.Any()) return RedirectToAction("Index");

            ViewBag.CarritoItems = carrito;
            ViewBag.Total = carrito.Sum(i => i.Subtotal);
            return View(new CheckoutViewModel());
        }

        // POST: /Carrito/ConfirmarCompra
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmarCompra(CheckoutViewModel model)
        {
            var carrito = ObtenerCarrito();
            if (!ModelState.IsValid)
            {
                ViewBag.CarritoItems = carrito;
                ViewBag.Total = carrito.Sum(i => i.Subtotal);
                return View("Finalizar", model);
            }

            var orden = new
            {
                DireccionEnvio = $"{model.Direccion}, {model.Ciudad}",
                Email = model.Email,
                NombreCliente = model.NombreCompleto,
                Total = carrito.Sum(i => i.Subtotal),
                Detalles = carrito.Select(i => new { i.ProductoID, i.Cantidad, i.PrecioUnitario })
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(orden), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("ordenes/registrar", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                Session.Remove(SESSION_KEY);
                return RedirectToAction("CompraExitosa");
            }

            ViewBag.Error = "No se pudo procesar la orden en la API.";
            return View("Finalizar", model);
        }

        public ActionResult CompraExitosa() => View();
    }
}