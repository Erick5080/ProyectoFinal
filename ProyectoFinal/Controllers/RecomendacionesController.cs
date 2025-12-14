using API.DAL;
using API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/recomendaciones")]
    public class RecomendacionesController : ApiController
    {
        private readonly DBHelper db = new DBHelper();

        // Mapea solo las propiedades necesarias para la vista rápida (slider)
        private List<Producto> MapDataTableToProductoSlider(DataTable dt)
        {
            List<Producto> productos = new List<Producto>();
            foreach (DataRow row in dt.Rows)
            {
                // Mapeamos solo lo esencial para el slider
                productos.Add(new Producto
                {
                    ProductoID = Convert.ToInt32(row["ProductoID"]),
                    Nombre = row["Nombre"].ToString(),
                    PrecioUnitario = Convert.ToDecimal(row["PrecioUnitario"]),
                    ImagenURL = row["ImagenURL"].ToString()
                    // No necesitamos Stock, Descripcion, etc., para el slider
                });
            }
            return productos;
        }

        // GET api/recomendaciones/masbaratos
        [HttpGet]
        [Route("masbaratos")]
        public IHttpActionResult ObtenerMasBaratos()
        {
            try
            {
                DataTable dt = db.ExecuteDataTable("PA_ObtenerProductosMasBaratos");
                List<Producto> productos = MapDataTableToProductoSlider(dt);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/recomendaciones/masrecientes
        [HttpGet]
        [Route("masrecientes")]
        public IHttpActionResult ObtenerMasRecientes()
        {
            try
            {
                DataTable dt = db.ExecuteDataTable("PA_ObtenerProductosMasRecientes");
                List<Producto> productos = MapDataTableToProductoSlider(dt);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/recomendaciones/masvendidos
        [HttpGet]
        [Route("masvendidos")]
        public IHttpActionResult ObtenerMasVendidos()
        {
            try
            {
                DataTable dt = db.ExecuteDataTable("PA_ObtenerProductosMasVendidos");
                List<Producto> productos = MapDataTableToProductoSlider(dt);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}