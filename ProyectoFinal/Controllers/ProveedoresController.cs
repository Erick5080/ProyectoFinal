using ProyectoFinal.DAL;
using ProyectoFinal.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Web.Http;

namespace ProyectoFinal.Controllers
{
    // Ruta de acceso: /api/proveedores
    [RoutePrefix("api/proveedores")]
    public class ProveedoresController : ApiController
    {
        private readonly DBHelper db = new DBHelper();

        // POST api/proveedores/registrar
        [HttpPost]
        [Route("registrar")]
        public IHttpActionResult RegistrarProveedor([FromBody] Proveedor proveedor)
        {
            if (proveedor == null || string.IsNullOrEmpty(proveedor.Nombre))
            {
                return BadRequest("El nombre del proveedor es obligatorio.");
            }

            try
            {
                // 1. Crear los parámetros
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Nombre", proveedor.Nombre),
                    new SqlParameter("@ContactoNombre", proveedor.ContactoNombre ?? (object)DBNull.Value), // Manejo de nulos
                    new SqlParameter("@Telefono", proveedor.Telefono ?? (object)DBNull.Value),
                    new SqlParameter("@Email", proveedor.Email ?? (object)DBNull.Value)
                };

                // 2. Ejecutar el stored procedure
                DataTable result = db.ExecuteDataTable("PA_InsertarProveedor", parameters);

                if (result.Rows.Count > 0)
                {
                    int newId = Convert.ToInt32(result.Rows[0]["ProveedorID"]);

                    if (newId > 0)
                    {
                        // Éxito
                        proveedor.ProveedorID = newId;
                        return Content(HttpStatusCode.Created, proveedor); // 201 Created
                    }
                    else if (newId == -1)
                    {
                        // Proveedor duplicado
                        return Conflict(); // 409 Conflict
                    }
                }

                return InternalServerError(new Exception("Fallo al obtener el ID del proveedor."));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); // 500 Internal Server Error
            }
        }

        // Función auxiliar para mapear un DataTable a una lista de Proveedores
        private List<Proveedor> MapDataTableToProveedores(DataTable dt)
        {
            List<Proveedor> proveedores = new List<Proveedor>();
            foreach (DataRow row in dt.Rows)
            {
                proveedores.Add(new Proveedor
                {
                    ProveedorID = Convert.ToInt32(row["ProveedorID"]),
                    Nombre = row["Nombre"].ToString(),
                    ContactoNombre = row["ContactoNombre"].ToString(),
                    Telefono = row["Telefono"].ToString(),
                    Email = row["Email"].ToString()
                });
            }
            return proveedores;
        }

        // GET api/proveedores/obtener
        [HttpGet]
        [Route("obtener")]
        public IHttpActionResult ObtenerTodosLosProveedores()
        {
            try
            {
                DataTable dt = db.ExecuteDataTable("PA_ObtenerProveedores");
                List<Proveedor> proveedores = MapDataTableToProveedores(dt);

                if (proveedores.Count == 0)
                {
                    return NotFound();
                }

                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/proveedores/obtener/{id}
        [HttpGet]
        [Route("obtener/{id:int}")]
        public IHttpActionResult ObtenerProveedorPorId(int id)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@ProveedorID", id)
                };

                DataTable dt = db.ExecuteDataTable("PA_ObtenerProveedores", parameters);
                List<Proveedor> proveedores = MapDataTableToProveedores(dt);

                if (proveedores.Count == 0)
                {
                    return NotFound();
                }

                return Ok(proveedores.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        // ... dentro de la clase ProveedoresController ...

        // PUT api/proveedores/actualizar/{id}
        [HttpPut]
        [Route("actualizar/{id:int}")]
        public IHttpActionResult ActualizarProveedor(int id, [FromBody] Proveedor proveedor)
        {
            // Validar que el ID de la URL y el cuerpo coincidan
            if (id != proveedor.ProveedorID || !ModelState.IsValid)
            {
                return BadRequest("Datos de proveedor o ID de ruta inválidos.");
            }

            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@ProveedorID", proveedor.ProveedorID),
            new SqlParameter("@Nombre", proveedor.Nombre),
            new SqlParameter("@ContactoNombre", proveedor.ContactoNombre ?? (object)DBNull.Value),
            new SqlParameter("@Telefono", proveedor.Telefono ?? (object)DBNull.Value),
            new SqlParameter("@Email", proveedor.Email ?? (object)DBNull.Value)
                };

                DataTable result = db.ExecuteDataTable("PA_ActualizarProveedor", parameters);

                // Verifica si se afectó al menos una fila
                if (Convert.ToInt32(result.Rows[0]["RowsAffected"]) > 0)
                {
                    return Ok(proveedor); // 200 OK con el objeto actualizado
                }

                return NotFound(); // 404 si el ID no existe
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}