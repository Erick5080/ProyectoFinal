using API.DAL;
using API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace API.Controllers
{
    public class CompraRequest
    {
        public int ProductoID { get; set; }
        public int CantidadComprada { get; set; }
    }

    [RoutePrefix("api/productos")]
    public class ProductosController : ApiController
    {
        private readonly DBHelper db = new DBHelper();

        // Función de Mapeo: Convierte las filas de la BD al modelo Producto
        private List<Producto> MapDataTableToProductos(DataTable dt)
        {
            List<Producto> productos = new List<Producto>();
            foreach (DataRow row in dt.Rows)
            {
                productos.Add(new Producto
                {
                    ProductoID = Convert.ToInt32(row["ProductoID"]),
                    Nombre = row["Nombre"].ToString(),
                    Descripcion = row["Descripcion"] != DBNull.Value ? row["Descripcion"].ToString() : "",
                    PrecioUnitario = Convert.ToDecimal(row["PrecioUnitario"]),
                    Stock = Convert.ToInt32(row["Stock"]),
                    FechaRegistro = dt.Columns.Contains("FechaRegistro") && row["FechaRegistro"] != DBNull.Value
                                    ? Convert.ToDateTime(row["FechaRegistro"]) : DateTime.Now,
                    ImagenURL = row["ImagenURL"] != DBNull.Value ? row["ImagenURL"].ToString() : "",
                    VentasAcumuladas = dt.Columns.Contains("VentasAcumuladas") ? Convert.ToInt32(row["VentasAcumuladas"]) : 0,
                    Activo = dt.Columns.Contains("Activo") ? Convert.ToBoolean(row["Activo"]) : true
                });
            }
            return productos;
        }

        // 1. OBTENER UN PRODUCTO POR ID (Utilizado por el Carrito)
        [HttpGet]
        [Route("obtener/{id:int}")]
        public IHttpActionResult ObtenerProductoPorId(int id)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ProductoID", id)
                };

                // Cambio Crítico: Usamos el procedimiento para buscar por ID específico
                // Asegúrate de tener el PA_ObtenerProductoPorID en tu BD
                DataTable dt = db.ExecuteDataTable("PA_ObtenerProductoPorID", parameters);
                List<Producto> productos = MapDataTableToProductos(dt);

                if (productos.Count == 0)
                {
                    return NotFound(); // Error 404
                }

                // Retornamos el objeto individual para que el FrontEnd lo procese correctamente
                return Ok(productos.FirstOrDefault());
            }
            catch (Exception ex)
            {
                // Retorna el error detallado para facilitar la depuración
                return InternalServerError(new Exception("Error al consultar la BD: " + ex.Message));
            }
        }

        // 2. OBTENER TODOS LOS PRODUCTOS EN STOCK
        [HttpGet]
        [Route("obtener")]
        public IHttpActionResult ObtenerTodosLosProductos()
        {
            try
            {
                DataTable dt = db.ExecuteDataTable("PA_ObtenerProductosEnStock");
                List<Producto> productos = MapDataTableToProductos(dt);

                if (productos.Count == 0) return NotFound();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // 3. REGISTRAR PRODUCTO (ADMIN)
        [HttpPost]
        [Route("registrar")]
        public IHttpActionResult RegistrarProducto([FromBody] Producto producto)
        {
            if (producto == null || !ModelState.IsValid) return BadRequest("Datos inválidos.");

            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Nombre", producto.Nombre),
                    new SqlParameter("@Descripcion", producto.Descripcion),
                    new SqlParameter("@PrecioUnitario", producto.PrecioUnitario),
                    new SqlParameter("@Stock", producto.Stock),
                    new SqlParameter("@ImagenURL", producto.ImagenURL)
                };

                DataTable result = db.ExecuteDataTable("PA_InsertarProducto", parameters);
                if (result.Rows.Count > 0)
                {
                    producto.ProductoID = Convert.ToInt32(result.Rows[0]["ProductoID"]);
                    return Content(HttpStatusCode.Created, producto);
                }
                return InternalServerError(new Exception("No se pudo obtener el nuevo ID."));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // 4. ACTUALIZAR PRODUCTO
        [HttpPut]
        [Route("actualizar/{id:int}")]
        public IHttpActionResult ActualizarProducto(int id, [FromBody] Producto producto)
        {
            if (id != producto.ProductoID || !ModelState.IsValid) return BadRequest("IDs no coinciden.");

            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ProductoID", producto.ProductoID),
                    new SqlParameter("@Nombre", producto.Nombre),
                    new SqlParameter("@Descripcion", producto.Descripcion),
                    new SqlParameter("@PrecioUnitario", producto.PrecioUnitario),
                    new SqlParameter("@Stock", producto.Stock),
                    new SqlParameter("@ImagenURL", producto.ImagenURL),
                    new SqlParameter("@Activo", producto.Activo)
                };

                DataTable result = db.ExecuteDataTable("PA_ActualizarProducto", parameters);
                return Ok(producto);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // 5. ELIMINAR PRODUCTO
        [HttpDelete]
        [Route("eliminar/{id:int}")]
        public IHttpActionResult EliminarProducto(int id)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ProductoID", id) };
                db.ExecuteDataTable("PA_DesactivarProducto", parameters);
                return Ok(new { Mensaje = "Producto desactivado." });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // 6. DISMINUIR STOCK
        [HttpPost]
        [Route("disminuirstock")]
        public IHttpActionResult DisminuirStock([FromBody] CompraRequest request)
        {
            if (request == null || request.CantidadComprada <= 0) return BadRequest("Cantidad inválida.");

            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ProductoID", request.ProductoID),
                    new SqlParameter("@CantidadComprada", request.CantidadComprada)
                };

                DataTable result = db.ExecuteDataTable("PA_DisminuirStock", parameters);
                if (Convert.ToInt32(result.Rows[0]["Resultado"]) == 1)
                    return Ok(new { Mensaje = "Stock actualizado." });

                return BadRequest("Stock insuficiente.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}