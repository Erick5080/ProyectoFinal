USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_ActualizarProveedor]    Script Date: 12/18/2025 12:24:09 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Procedimiento Almacenado para Actualizar un Proveedor
CREATE PROCEDURE [dbo].[PA_ActualizarProveedor]
    @ProveedorID INT,
    @Nombre VARCHAR(100),
    @ContactoNombre VARCHAR(100) = NULL,
    @Telefono VARCHAR(20) = NULL,
    @Email VARCHAR(100) = NULL
AS
BEGIN
    UPDATE Proveedores
    SET
        Nombre = @Nombre,
        ContactoNombre = @ContactoNombre,
        Telefono = @Telefono,
        Email = @Email
    WHERE
        ProveedorID = @ProveedorID;

    -- Devolver las filas afectadas
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO


