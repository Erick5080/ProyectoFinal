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
        // Asegúrate que tu AppSettings["ApiBaseUrl"] termina en /
        private readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"];
        private readonly HttpClient _httpClient;

        public CarritoController()
        {
            _httpClient = new HttpClient();
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
                // Manejo de error si la sesión está corrupta
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

        // GET: /Carrito/Index (Muestra el carrito de compras)
        public ActionResult Index()
        {
            var carrito = ObtenerCarrito();
            // Esto es crucial para que el _Layout pueda mostrar el número de ítems
            ViewBag.CarritoCount = carrito.Sum(i => i.Cantidad);
            return View(carrito);
        }

        // POST: /Carrito/Agregar (Añade un producto al carrito)
        [HttpPost]
        public async Task<JsonResult> Agregar(int productoId, int cantidad)
        {
            var carrito = ObtenerCarrito();

            // 1. Obtener detalles del producto (Necesitamos el PrecioUnitario)
            var response = await _httpClient.GetAsync($"productos/obtenerporid/{productoId}");
            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Producto no encontrado en la API." });
            }

            var content = await response.Content.ReadAsStringAsync();
            var productoDetalle = JsonConvert.DeserializeObject<dynamic>(content); // Usamos dynamic para no depender de un modelo Producto específico.

            // 2. Actualizar o Añadir item al carrito
            var itemExistente = carrito.FirstOrDefault(i => i.ProductoID == productoId);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                // Asegúrate que el tipo de dato coincida con la BD/API (decimal)
                decimal precio = (decimal)productoDetalle.PrecioUnitario;

                carrito.Add(new CarritoItem
                {
                    ProductoID = productoId,
                    Nombre = (string)productoDetalle.Nombre,
                    PrecioUnitario = precio,
                    Cantidad = cantidad
                });
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
                var newItemSubtotal = item != null ? item.Subtotal : 0;

                return Json(new
                {
                    success = true,
                    newCount = carrito.Sum(i => i.Cantidad),
                    newTotalText = newTotal.ToString("C"),
                    newItemSubtotalText = newItemSubtotal.ToString("C")
                });
            }

            return Json(new { success = false, message = "Producto no encontrado en el carrito." });
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

                var newTotal = carrito.Sum(i => i.Subtotal);

                return Json(new
                {
                    success = true,
                    newCount = carrito.Sum(i => i.Cantidad),
                    newTotalText = newTotal.ToString("C")
                });
            }

            return Json(new { success = false, message = "Producto no encontrado en el carrito." });
        }

        // GET: /Carrito/Finalizar (Vista del formulario de Checkout)
        public ActionResult Finalizar()
        {
            var carrito = ObtenerCarrito();
            if (!carrito.Any())
            {
                TempData["Message"] = "Su carrito está vacío y no puede finalizar la compra.";
                return RedirectToAction("Index");
            }

            var checkoutModel = new CheckoutViewModel();

            ViewBag.CarritoItems = carrito;
            ViewBag.Total = carrito.Sum(i => i.Subtotal);

            return View(checkoutModel);
        }

        // POST: /Carrito/ConfirmarCompra (Llama a la API para registrar la Orden)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmarCompra(CheckoutViewModel model)
        {
            var carrito = ObtenerCarrito();
            ViewBag.CarritoItems = carrito;
            ViewBag.Total = carrito.Sum(i => i.Subtotal);

            if (!carrito.Any())
            {
                TempData["Message"] = "Su carrito está vacío. Por favor, añada productos.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                return View("Finalizar", model);
            }

            // 1. Preparar el objeto Orden para la API
            var orden = new
            {
                ClienteID = (int?)null,
                DireccionEnvio = $"{model.Direccion}, {model.Ciudad}, {model.CodigoPostal}",
                Email = model.Email,
                NombreCliente = model.NombreCompleto,
                Total = carrito.Sum(i => i.Subtotal),
                Detalles = carrito.Select(i => new
                {
                    i.ProductoID,
                    i.Cantidad,
                    PrecioUnitario = i.PrecioUnitario
                })
            };

            // 2. Enviar a la API
            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(orden),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("ordenes/registrar", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                Session.Remove(SESSION_KEY);
                return RedirectToAction("CompraExitosa", "Carrito");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.Error = $"Error ({response.StatusCode}): La API falló al procesar la orden. Detalles: {errorContent}";
                return View("Finalizar", model);
            }
        }

        public ActionResult CompraExitosa()
        {
            return View();
        }
    }
}