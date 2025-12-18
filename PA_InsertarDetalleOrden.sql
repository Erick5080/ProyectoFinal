USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_InsertarDetalleOrden]    Script Date: 12/18/2025 12:35:23 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- 2. Procedimiento para Insertar el Detalle de la Orden
CREATE PROCEDURE [dbo].[PA_InsertarDetalleOrden]
    @OrdenID INT,
    @ProductoID INT,
    @Cantidad INT,
    @PrecioUnitarioEnVenta DECIMAL(10, 2),
    @SubTotal DECIMAL(10, 2)
AS
BEGIN
    INSERT INTO DetalleOrdenes (
        OrdenID,
        ProductoID,
        Cantidad,
        PrecioUnitarioEnVenta,
        SubTotal
    )
    VALUES (
        @OrdenID,
        @ProductoID,
        @Cantidad,
        @PrecioUnitarioEnVenta,
        @SubTotal
    );
END
GO


