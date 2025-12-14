using API.DAL;
using API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Transactions;

namespace API.Controllers
{
    [RoutePrefix("api/ordenes")]
    public class OrdenesController : ApiController
    {
        private readonly DBHelper db = new DBHelper();

        // 1. REGISTRAR ORDEN (VENTA)
        
        [HttpPost]
        [Route("registrar")]
        public IHttpActionResult RegistrarOrden([FromBody] OrdenRequest orden)
        {
            if (orden == null || orden.Detalles == null || !orden.Detalles.Any())
            {
                return BadRequest("La orden no puede estar vacía.");
            }

            // Usar TransactionScope asegura que todas las llamadas a la BD se hagan como una sola unidad atómica.
            using (var scope = new TransactionScope())
            {
                try
                {
                    // Calcular el total de la orden
                    decimal totalOrden = orden.Detalles.Sum(d => d.Cantidad * d.PrecioUnitarioEnVenta);

                    // 1. Insertar Encabezado de la Orden
                    SqlParameter[] headerParams = new SqlParameter[]
                    {
                        new SqlParameter("@ClienteID", orden.ClienteID ?? (object)DBNull.Value),
                        new SqlParameter("@Total", totalOrden)
                    };
                    
                    DataTable dtHeader = db.ExecuteDataTable("PA_InsertarOrden", headerParams);
                    if (dtHeader.Rows.Count == 0 || dtHeader.Rows[0]["OrdenID"] == DBNull.Value)
                    {
                        throw new Exception("No se pudo obtener el ID de la orden.");
                    }
                    int ordenID = Convert.ToInt32(dtHeader.Rows[0]["OrdenID"]);

                    // 2. Procesar cada Detalle de la Orden
                    foreach (var detalle in orden.Detalles)
                    {
                        // A) Disminuir Stock
                        SqlParameter[] stockParams = new SqlParameter[]
                        {
                            new SqlParameter("@ProductoID", detalle.ProductoID),
                            new SqlParameter("@CantidadComprada", detalle.Cantidad)
                        };
                        
                        DataTable resultStock = db.ExecuteDataTable("PA_DisminuirStock", stockParams);

                        if (Convert.ToInt32(resultStock.Rows[0]["Resultado"]) == 0)
                        {
                            // Si falla la disminución de stock, la transacción se revertirá
                            return BadRequest($"Error: Stock insuficiente para Producto ID: {detalle.ProductoID}");
                        }
                        
                        // B) Insertar Detalle de la Orden
                        SqlParameter[] detailParams = new SqlParameter[]
                        {
                            new SqlParameter("@OrdenID", ordenID),
                            new SqlParameter("@ProductoID", detalle.ProductoID),
                            new SqlParameter("@Cantidad", detalle.Cantidad),
                            new SqlParameter("@PrecioUnitarioEnVenta", detalle.PrecioUnitarioEnVenta),
                            new SqlParameter("@SubTotal", detalle.Cantidad * detalle.PrecioUnitarioEnVenta)
                        };
                        
                        db.ExecuteNonQuery("PA_InsertarDetalleOrden", detailParams);
                    }
                    
                    // 3. Si todo fue exitoso, confirmar la transacción (Commit)
                    scope.Complete(); 
                    
                    return Content(HttpStatusCode.Created, new { OrdenID = ordenID, Mensaje = "Orden registrada y stock actualizado correctamente." });
                }
                catch (Exception ex)
                {
                    // Si hay una excepción, la transacción se revertirá (Rollback) automáticamente
                    return InternalServerError(ex); 
                }
            }
        }
        
        // 2. CONSULTAR ÓRDENES (ADMIN)

        [HttpGet]
        [Route("obtener")]
        public IHttpActionResult ObtenerTodasLasOrdenes()
        {
            // Nota: Se necesitaría un modelo de mapeo para esta tabla si se quiere devolver más detalle
            try
            {
                DataTable dt = db.ExecuteDataTable("PA_ObtenerOrdenes");

                if (dt.Rows.Count == 0)
                {
                    return NotFound();
                }

                // Devolver el DataTable directamente o mapear a un modelo de Ordenes
                return Ok(dt); 
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}