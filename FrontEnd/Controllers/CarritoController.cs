using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FrontEnd.Controllers
{
    public class CarritoController : Controller
    {
        // Esta acción simplemente sirve la vista
        public ActionResult Index()
        {
            ViewBag.Title = "Carrito de Compras";
            return View();
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

            // Validación de los datos del formulario de Checkout
            if (!ModelState.IsValid)
            {
                // Si la validación falla, regresa a la vista con errores
                return View("Finalizar", model);
            }

            // 1. Preparar el objeto Orden para la API
            var orden = new
            {
                // Asumimos ClienteID = NULL si la lógica de autenticación no se implementa
                ClienteID = (int?)null,

                // Aquí usamos los datos capturados en el CheckoutViewModel
                DireccionEnvio = $"{model.Direccion}, {model.Ciudad}, {model.CodigoPostal}",
                Email = model.Email,
                NombreCliente = model.NombreCompleto,

                Total = carrito.Sum(i => i.Subtotal),
                Detalles = carrito.Select(i => new
                {
                    i.ProductoID,
                    i.Cantidad,
                    // Se utiliza el PrecioUnitario almacenado en la sesión del carrito
                    PrecioUnitario = i.PrecioUnitario
                })
            };

            // 2. Enviar a la API
            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(orden),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // POST al endpoint de registro de órdenes de la API
            var response = await _httpClient.PostAsync(API_BASE_URL + "ordenes/registrar", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // Limpiar carrito y redirigir
                Session.Remove(SESSION_KEY);
                return RedirectToAction("CompraExitosa", "Carrito");
            }
            else
            {
                // Opcional: Leer el mensaje de error de la API si lo envía
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.Error = $"Error ({response.StatusCode}): La API falló al procesar la orden. {errorContent}";
                return View("Finalizar", model);
            }
        }

        public ActionResult CompraExitosa()
        {
            return View();
        }
    }
}