USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_DisminuirStock]    Script Date: 12/18/2025 12:33:32 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Procedimiento Almacenado para Disminuir Stock tras una venta (Cliente)
CREATE PROCEDURE [dbo].[PA_DisminuirStock]
    @ProductoID INT,
    @CantidadComprada INT
AS
BEGIN
    -- 1. Verificar si hay suficiente stock
    IF (SELECT Stock FROM Productos WHERE ProductoID = @ProductoID) >= @CantidadComprada
    BEGIN
        -- 2. Actualizar el stock y las ventas acumuladas
        UPDATE Productos
        SET
            Stock = Stock - @CantidadComprada,
            VentasAcumuladas = VentasAcumuladas + @CantidadComprada
        WHERE
            ProductoID = @ProductoID;

        -- Devolver 1 (Éxito)
        SELECT 1 AS Resultado;
    END
    ELSE
    BEGIN
        -- Devolver 0 (Stock insuficiente)
        SELECT 0 AS Resultado;
    END
END
GO


