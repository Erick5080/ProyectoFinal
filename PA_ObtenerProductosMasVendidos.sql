USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_ObtenerProductosMasVendidos]    Script Date: 12/18/2025 12:40:23 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Procedimiento Almacenado para obtener los productos más vendidos
CREATE PROCEDURE [dbo].[PA_ObtenerProductosMasVendidos]
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
        VentasAcumuladas DESC; -- Ordenado por ventas de mayor a menor
END
GO


