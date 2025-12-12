using ProyectoFinal.DAL;
using ProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace ProyectoFinal.Controllers
{
    // Modelo simple para la solicitud de compra (Cliente)
    public class CompraRequest
    {
        public int ProductoID { get; set; }
        public int CantidadComprada { get; set; }
    }

    // RUTA BASE: /api/productos
    [RoutePrefix("api/productos")]
    public class ProductosController : ApiController
    {
        private readonly DBHelper db = new DBHelper();

        // =========================================================================
        // Mapeo (Helper Function)
        // =========================================================================

        private List<Producto> MapDataTableToProductos(DataTable dt)
        {
            List<Producto> productos = new List<Producto>();
            foreach (DataRow row in dt.Rows)
            {
                productos.Add(new Producto
                {
                    ProductoID = Convert.ToInt32(row["ProductoID"]),
                    Nombre = row["Nombre"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),
                    PrecioUnitario = Convert.ToDecimal(row["PrecioUnitario"]),
                    Stock = Convert.ToInt32(row["Stock"]),
                    FechaRegistro = Convert.ToDateTime(row["FechaRegistro"]),
                    ImagenURL = row["ImagenURL"].ToString(),
                    VentasAcumuladas = Convert.ToInt32(row["VentasAcumuladas"]),
                    Activo = Convert.ToBoolean(row["Activo"])
                });
            }
            return productos;
        }

        // =========================================================================
        // 1. CREAR PRODUCTO (ADMIN)
        // Endpoint: POST api/productos/registrar
        // =========================================================================

        [HttpPost]
        [Route("registrar")]
        public IHttpActionResult RegistrarProducto([FromBody] Producto producto)
        {
            if (producto == null || !ModelState.IsValid)
            {
                return BadRequest("Datos de producto inválidos.");
            }

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
                    int newId = Convert.ToInt32(result.Rows[0]["ProductoID"]);

                    if (newId > 0)
                    {
                        producto.ProductoID = newId;
                        return Content(HttpStatusCode.Created, producto); // 201 Created
                    }
                    else if (newId == -1)
                    {
                        return Conflict(); // 409 Conflict (Duplicado)
                    }
                }

                return InternalServerError(new Exception("Fallo al obtener el ID del producto."));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // =========================================================================
        // 2. LEER PRODUCTOS (LECTURA)
        // Endpoint: GET api/productos/obtener y GET api/productos/obtener/5
        // =========================================================================

        [HttpGet]
        [Route("obtener")]
        public IHttpActionResult ObtenerTodosLosProductos()
        {
            try
            {
                DataTable dt = db.ExecuteDataTable("PA_ObtenerProductosEnStock");
                List<Producto> productos = MapDataTableToProductos(dt);

                if (productos.Count == 0)
                {
                    return NotFound();
                }

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

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

                DataTable dt = db.ExecuteDataTable("PA_ObtenerProductosEnStock", parameters);
                List<Producto> productos = MapDataTableToProductos(dt);

                if (productos.Count == 0)
                {
                    return NotFound();
                }

                return Ok(productos.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // =========================================================================
        // 3. ACTUALIZAR PRODUCTO (ADMIN)
        // Endpoint: PUT api/productos/actualizar/5
        // =========================================================================

        [HttpPut]
        [Route("actualizar/{id:int}")]
        public IHttpActionResult ActualizarProducto(int id, [FromBody] Producto producto)
        {
            if (id != producto.ProductoID || !ModelState.IsValid)
            {
                return BadRequest("Datos de producto o ID de ruta inválidos.");
            }

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

                if (Convert.ToInt32(result.Rows[0]["RowsAffected"]) > 0)
                {
                    return Ok(producto); // 200 OK
                }

                return NotFound(); // 404 si el ID no existe
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // =========================================================================
        // 4. ELIMINAR/DESACTIVAR PRODUCTO (ADMIN)
        // Endpoint: DELETE api/productos/eliminar/5
        // =========================================================================

        [HttpDelete]
        [Route("eliminar/{id:int}")]
        public IHttpActionResult EliminarProducto(int id)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ProductoID", id)
                };

                DataTable result = db.ExecuteDataTable("PA_DesactivarProducto", parameters);

                if (Convert.ToInt32(result.Rows[0]["RowsAffected"]) > 0)
                {
                    return Ok(new { Mensaje = $"Producto con ID {id} desactivado correctamente." }); // 200 OK
                }

                return NotFound(); // 404 si el ID no existe
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // =========================================================================
        // 5. DISMINUIR STOCK (CLIENTE/VENTA)
        // Endpoint: POST api/productos/disminuirstock
        // =========================================================================

        [HttpPost]
        [Route("disminuirstock")]
        public IHttpActionResult DisminuirStock([FromBody] CompraRequest request)
        {
            if (request == null || request.CantidadComprada <= 0)
            {
                return BadRequest("Cantidad de compra inválida.");
            }

            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ProductoID", request.ProductoID),
                    new SqlParameter("@CantidadComprada", request.CantidadComprada)
                };

                DataTable result = db.ExecuteDataTable("PA_DisminuirStock", parameters);

                if (Convert.ToInt32(result.Rows[0]["Resultado"]) == 1)
                {
                    // 200 OK (El stock se procesó)
                    return Ok(new { Mensaje = "Stock actualizado correctamente tras la compra." });
                }
                else
                {
                    // 400 Bad Request (Stock insuficiente)
                    return BadRequest("Error: Stock insuficiente para procesar la compra.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}