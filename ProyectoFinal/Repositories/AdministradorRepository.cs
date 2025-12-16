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
        // Instancia de tu clase de acceso a datos
        private readonly DBHelper _dbHelper = new DBHelper();

        // Obtiene un administrador por email y contraseña (asumiendo SP)
        public Administrador GetAdminByCredentials(string email, string password)
        {
            // NOTA: Debes crear este procedimiento almacenado en tu base de datos SQL Server.
            string storedProcedure = "SP_ValidarAdministrador";

            // Parámetros para el SP
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Email", email),
                new SqlParameter("@Password", password) 
                // Asegúrate que tu SP maneje correctamente el hashing de la contraseña si aplica.
            };

            // Ejecuta la consulta y obtiene el resultado
            DataTable dt = _dbHelper.ExecuteDataTable(storedProcedure, parameters);

            if (dt.Rows.Count == 1)
            {
                // Mapear la primera fila del DataTable a un objeto Administrador
                DataRow row = dt.Rows[0];
                return new Administrador
                {
                    AdminID = Convert.ToInt32(row["AdminID"]),
                    Nombre = row["Nombre"].ToString(),
                    Email = row["Email"].ToString(),
                    Rol = row["Rol"].ToString(),
                    Activo = Convert.ToBoolean(row["Activo"])
                };
            }
            return null; // Credenciales inválidas o administrador no encontrado
        }
    }
}