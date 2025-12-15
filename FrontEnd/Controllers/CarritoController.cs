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

        // Si tuvieras una página de "Confirmación de Pago" o similar, iría aquí.
        // public ActionResult Confirmacion() { ... }
    }
}