USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_ActualizarProducto]    Script Date: 12/18/2025 12:21:24 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Procedimiento Almacenado para Actualizar un Producto (Admin)
ALTER PROCEDURE [dbo].[PA_ActualizarProducto]
    @ProductoID INT,
    @Nombre VARCHAR(100),
    @Descripcion VARCHAR(MAX),
    @PrecioUnitario DECIMAL(10, 2),
    @Stock INT,
    @ImagenURL VARCHAR(255),
    @Activo BIT -- También permite al admin activarlo/desactivarlo
AS
BEGIN
    UPDATE Productos
    SET
        Nombre = @Nombre,
        Descripcion = @Descripcion,
        PrecioUnitario = @PrecioUnitario,
        Stock = @Stock,
        ImagenURL = @ImagenURL,
        Activo = @Activo
    WHERE
        ProductoID = @ProductoID;

    -- Devolver las filas afectadas (1 si fue exitoso, 0 si no se encontró)
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO


