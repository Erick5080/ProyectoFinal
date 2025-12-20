using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FrontEnd.Models;
using System.Configuration;

namespace FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private const string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"];
        private readonly HttpClient _httpClient = new HttpClient();

        //Página principal
        public async Task<ActionResult> Index()
        {
            // Carga los productos destacados antes de cargar la vista
            var productosDestacados = await GetProductosListFromApi("recomendaciones/masvendidos");
            ViewBag.Destacados = productosDestacados;

            // La carga del catálogo completo se realiza con JavaScript (Fetch) en la vista.
            return View();
        }

        // Vista de Detalle de Producto
        public async Task<ActionResult> Detalle(int id)
        {
            if (id <= 0)
            {
                // Redirigir si el ID no es válido
                return RedirectToAction("Index");
            }

            // Consumir el endpoint específico por ID: api/productos/obtenerporid/{id}
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{API_BASE_URL}productos/obtenerporid/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // Deserializar a un ÚNICO objeto Producto
                    var producto = JsonConvert.DeserializeObject<Producto>(responseBody);

                    if (producto != null)
                    {
                        return View(producto); // Pasa el objeto Producto a la vista Detalle
                    }
                }

                // Manejo de error si el producto no existe o la API falla
                ViewBag.Error = $"El producto con ID {id} no fue encontrado o la API falló. (Código HTTP: {response.StatusCode})";
                return View("ErrorNotFound");
            }
            catch (Exception ex)
            {
                // Manejo de error de conexión de red
                ViewBag.Error = $"Ocurrió un error al conectar con la API: {ex.Message}";
                return View("ErrorNotFound");
            }
        }

        // Función Auxiliar para Consumir la API y devolver una LISTA
        private async Task<List<Producto>> GetProductosListFromApi(string endpoint)
        {
            try
            {
                // Realizar la petición GET a la URL completa del API
                HttpResponseMessage response = await _httpClient.GetAsync($"{API_BASE_URL}{endpoint}");

                if (response.IsSuccessStatusCode)
                {
                    // Leer y deserializar la respuesta JSON a una lista
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Producto>>(responseBody);
                }
            }
            catch (Exception ex)
            {
                // Loguear error de conexión para depuración
                System.Diagnostics.Debug.WriteLine($"Error al consumir API en {endpoint}: {ex.Message}");
            }
            return new List<Producto>();
        }

        // Métodos base del controlador
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}