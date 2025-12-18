USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_ObtenerProductosMasBaratos]    Script Date: 12/18/2025 12:39:34 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Procedimiento Almacenado para obtener los productos más baratos
CREATE PROCEDURE [dbo].[PA_ObtenerProductosMasBaratos]
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
        PrecioUnitario ASC; -- Ordenado por precio de menor a mayor
END
GO


