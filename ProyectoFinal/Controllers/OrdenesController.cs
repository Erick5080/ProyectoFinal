using ProyectoFinal.DAL;
using ProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Transactions; // Necesitas agregar esta referencia: System.Transactions

namespace ProyectoFinal.Controllers
{
    [RoutePrefix("api/ordenes")]
    public class OrdenesController : ApiController
    {
        private readonly DBHelper db = new DBHelper();

        // POST api/ordenes/registrar
        [HttpPost]
        [Route("registrar")]
        public IHttpActionResult RegistrarOrden([FromBody] OrdenRequest orden)
        {
            if (orden == null || !orden.Detalles.Any())
            {
                return BadRequest("La orden no puede estar vacía.");
            }

            // Usamos System.Transactions para asegurar que todas las operaciones de BD se completen o fallen juntas
            using (var scope = new TransactionScope())
            {
                try
                {
                    decimal totalOrden = orden.Detalles.Sum(d => d.Cantidad * d.PrecioUnitarioEnVenta);

                    // 1. Insertar el Encabezado de la Orden
                    SqlParameter[] headerParams = new SqlParameter[]
                    {
                        new SqlParameter("@ClienteID", orden.ClienteID ?? (object)DBNull.Value),
                        new SqlParameter("@Total", totalOrden),
                        // El PA debe devolver el nuevo OrdenID
                    };

                    // Supongamos que tienes un PA que inserta la orden y devuelve el ID
                    // PA_InsertarOrden debería ejecutarse aquí
                    // Por simplicidad, simularemos la inserción del encabezado con un PA ficticio o una función de DB que devuelva el ID.

                    // **NOTA: Necesitas crear el PA_InsertarOrden en SQL**
                    // CREATE PROCEDURE PA_InsertarOrden ... SELECT SCOPE_IDENTITY() AS OrdenID;

                    DataTable dtHeader = db.ExecuteDataTable("PA_InsertarOrden", headerParams);
                    int ordenID = Convert.ToInt32(dtHeader.Rows[0]["OrdenID"]);

                    // 2. Procesar cada Detalle de la Orden
                    foreach (var detalle in orden.Detalles)
                    {
                        // a) Disminuir Stock (utilizando el PA que ya creaste)
                        SqlParameter[] stockParams = new SqlParameter[]
                        {
                            new SqlParameter("@ProductoID", detalle.ProductoID),
                            new SqlParameter("@CantidadComprada", detalle.Cantidad)
                        };

                        DataTable resultStock = db.ExecuteDataTable("PA_DisminuirStock", stockParams);

                        if (Convert.ToInt32(resultStock.Rows[0]["Resultado"]) == 0)
                        {
                            // Si falla la disminución de stock, aborta la transacción
                            return BadRequest($"Stock insuficiente para Producto ID: {detalle.ProductoID}");
                        }

                        // b) Insertar Detalle
                        SqlParameter[] detailParams = new SqlParameter[]
                        {
                            new SqlParameter("@OrdenID", ordenID),
                            new SqlParameter("@ProductoID", detalle.ProductoID),
                            new SqlParameter("@Cantidad", detalle.Cantidad),
                            new SqlParameter("@PrecioUnitarioEnVenta", detalle.PrecioUnitarioEnVenta),
                            new SqlParameter("@SubTotal", detalle.Cantidad * detalle.PrecioUnitarioEnVenta)
                        };

                        // **NOTA: Necesitas crear el PA_InsertarDetalleOrden en SQL**
                        // CREATE PROCEDURE PA_InsertarDetalleOrden ...
                        db.ExecuteNonQuery("PA_InsertarDetalleOrden", detailParams);
                    }

                    // 3. Confirmar la Transacción
                    scope.Complete();

                    return Ok(new { OrdenID = ordenID, Mensaje = "Orden registrada y stock actualizado correctamente." });
                }
                catch (Exception ex)
                {
                    // Si algo falla, el scope.Complete() no se llama y la transacción se revierte
                    return InternalServerError(ex);
                }
            }
        }
    }
}