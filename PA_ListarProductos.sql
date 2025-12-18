USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_ListarProductos]    Script Date: 12/18/2025 12:37:59 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[PA_ListarProductos]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        ProductoID,
        Nombre,
        Descripcion,
        PrecioUnitario,
        Stock,
        ImagenURL,
        Activo
    FROM Productos
    WHERE Activo = 1; -- Solo productos vigentes
END
GO


