USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_InsertarOrden]    Script Date: 12/18/2025 12:36:49 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- 1. Procedimiento para Insertar la Orden (Encabezado)
CREATE PROCEDURE [dbo].[PA_InsertarOrden]
    @ClienteID INT = NULL,
    @Total DECIMAL(10, 2)
AS
BEGIN
    -- Insertar el encabezado
    INSERT INTO Ordenes (
        ClienteID,
        Total
    )
    VALUES (
        @ClienteID,
        @Total
    );

    -- Devolver el ID de la orden recién insertada
    SELECT SCOPE_IDENTITY() AS OrdenID;
END
GO


