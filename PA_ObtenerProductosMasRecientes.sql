USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_ObtenerProductosMasRecientes]    Script Date: 12/18/2025 12:40:02 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Procedimiento Almacenado para obtener los productos más recientes
CREATE PROCEDURE [dbo].[PA_ObtenerProductosMasRecientes]
AS
BEGIN
    SELECT TOP 5
        ProductoID,
        Nombre,
        PrecioUnitario,
        ImagenURL
    FROM
        Productos
    WHERE
        Activo = 1 
        AND Stock > 0
    ORDER BY
        FechaRegistro DESC; -- Ordenado por fecha de más reciente a más antiguo
END
GO


