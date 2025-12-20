using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using API.DAL;
using API.Models;
using System.Data;
using System.Data.SqlClient;

namespace API.Repositories
{
    public class AdministradorRepository
    {
        private readonly DBHelper _dbHelper = new DBHelper();

        public Administrador GetAdminByCredentials(string email, string password)
        {
            string storedProcedure = "PA_ValidarAdministrador";

            // CORRECCIÓN: Los nombres de los parámetros deben coincidir EXACTAMENTE con el SQL
            // Tu procedimiento espera @Email y @Password
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Email", (object)email ?? DBNull.Value),
                new SqlParameter("@Password", (object)password ?? DBNull.Value)
            };

            try
            {
                DataTable dt = _dbHelper.ExecuteDataTable(storedProcedure, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Administrador
                    {
                        // Se usan los alias definidos en tu procedimiento (userId, nombre, role)
                        AdminID = Convert.ToInt32(row["userId"]),
                        Nombre = row["nombre"].ToString(),
                        Rol = row["role"].ToString(),
                        // Asignamos el email usado para el login
                        Email = email,
                        Activo = true
                    };
                }
            }
            catch (Exception ex)
            {
                // Esto ayuda a capturar errores de conexión o de SQL durante la depuración
                throw new Exception("Error en AdministradorRepository: " + ex.Message);
            }

            return null;
        }
    }
}