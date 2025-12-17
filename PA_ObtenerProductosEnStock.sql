USE ProyectoFinal;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PA_ObtenerProductosEnStock')
    DROP PROCEDURE PA_ObtenerProductosEnStock;
GO

CREATE PROCEDURE PA_ObtenerProductosEnStock
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        ProductoID, 
        Nombre, 
        Descripcion, 
        PrecioUnitario, 
        Stock, 
        FechaRegistro,
        ImagenURL, 
        VentasAcumuladas,
        Activo
    FROM Productos
    WHERE Activo = 1; -- Solo productos vigentes
END
GO