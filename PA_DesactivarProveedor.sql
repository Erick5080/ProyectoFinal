USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_DesactivarProveedor]    Script Date: 12/18/2025 12:32:40 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Procedimiento Almacenado para Desactivar (Eliminación Lógica) un Proveedor
CREATE PROCEDURE [dbo].[PA_DesactivarProveedor]
    @ProveedorID INT
AS
BEGIN
    UPDATE Proveedores
    SET
        Activo = 0 -- Marcado como inactivo
    WHERE
        ProveedorID = @ProveedorID;

    -- Devolver las filas afectadas
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO


