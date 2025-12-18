USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_DesactivarProducto]    Script Date: 12/18/2025 12:28:16 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Procedimiento Almacenado para Desactivar (Eliminación Lógica) un Producto
CREATE PROCEDURE [dbo].[PA_DesactivarProducto]
    @ProductoID INT
AS
BEGIN
    UPDATE Productos
    SET
        Activo = 0 -- Marcado como inactivo
    WHERE
        ProductoID = @ProductoID;

    -- Devolver las filas afectadas
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO


