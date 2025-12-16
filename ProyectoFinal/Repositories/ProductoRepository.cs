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
    public class ProductoRepository
    {
        private readonly DBHelper _dbHelper = new DBHelper();

        private Producto MapRowToProducto(DataRow row)
        {
            return new Producto
            {
                ProductoID = Convert.ToInt32(row["ProductoID"]),
                ProveedorID = Convert.ToInt32(row["ProveedorID"]),
                Nombre = row["Nombre"].ToString(),
                PrecioUnitario = Convert.ToDecimal(row["PrecioUnitario"]),
                Stock = Convert.ToInt32(row["Stock"]),
                Activo = Convert.ToBoolean(row["Activo"])
                // Agrega aquí todas las propiedades de tu modelo Producto
            };
        }

        // 1. LISTAR TODOS (usado por /Administracion/Productos)
        public List<Producto> GetAllProductos()
        {
            List<Producto> productos = new List<Producto>();
            // Asegúrate que este SP exista: SP_ListarProductos
            DataTable dt = _dbHelper.ExecuteDataTable("SP_ListarProductos");

            foreach (DataRow row in dt.Rows)
            {
                productos.Add(MapRowToProducto(row));
            }
            return productos;
        }

        // 2. OBTENER POR ID (usado por CarritoController.Agregar y /Administracion/EditarProducto)
        public Producto GetProductoById(int id)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ProductoID", id)
            };
            // Asegúrate que este SP exista: SP_ObtenerProductoPorId
            DataTable dt = _dbHelper.ExecuteDataTable("SP_ObtenerProductoPorId", parameters);

            if (dt.Rows.Count == 1)
            {
                return MapRowToProducto(dt.Rows[0]);
            }
            return null;
        }

        // 3. INSERTAR (usado por /Administracion/RegistrarProducto)
        public int InsertProducto(Producto producto)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ProveedorID", producto.ProveedorID),
                new SqlParameter("@Nombre", producto.Nombre),
                new SqlParameter("@PrecioUnitario", producto.PrecioUnitario),
                new SqlParameter("@Stock", producto.Stock),
                new SqlParameter("@Activo", producto.Activo)
            };
            // Asegúrate que este SP exista: SP_InsertarProducto
            return _dbHelper.ExecuteNonQuery("SP_InsertarProducto", parameters);
        }

        // 4. ACTUALIZAR (usado por /Administracion/EditarProducto)
        public int UpdateProducto(Producto producto)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ProductoID", producto.ProductoID),
                new SqlParameter("@ProveedorID", producto.ProveedorID),
                new SqlParameter("@Nombre", producto.Nombre),
                new SqlParameter("@PrecioUnitario", producto.PrecioUnitario),
                new SqlParameter("@Stock", producto.Stock),
                new SqlParameter("@Activo", producto.Activo)
            };
            // Asegúrate que este SP exista: SP_ActualizarProducto
            return _dbHelper.ExecuteNonQuery("SP_ActualizarProducto", parameters);
        }

        // 5. ELIMINAR (o desactivar)
        public int DeleteProducto(int id)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ProductoID", id)
            };
            // Puedes usar SP_EliminarProducto o SP_DesactivarProducto
            return _dbHelper.ExecuteNonQuery("SP_EliminarProducto", parameters);
        }
    }
}