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

        public Administrador GetAdminByCredentials(string email, string password)
        {
            string storedProcedure = "PA_ValidarAdministrador";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Correo", email), 
        new SqlParameter("@Clave", password)
            };

            DataTable dt = _dbHelper.ExecuteDataTable(storedProcedure, parameters);

            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];
                return new Administrador
                {
                    AdminID = Convert.ToInt32(row["UsuarioID"]),
                    Nombre = row["NombreCompleto"].ToString(),
                    Email = row["Email"].ToString(),
                };
            }
            return null;
        }
    }
}