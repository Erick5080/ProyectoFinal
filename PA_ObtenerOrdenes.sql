USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_ObtenerOrdenes]    Script Date: 12/18/2025 12:38:29 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- 3. Procedimiento para Obtener Órdenes (Consulta para Administrador)
CREATE PROCEDURE [dbo].[PA_ObtenerOrdenes]
    @OrdenID INT = NULL 
AS
BEGIN
    SELECT 
        o.OrdenID,
        o.ClienteID,
        o.FechaOrden,
        o.Total,
        o.Estado,
        COUNT(d.DetalleOrdenID) AS CantidadProductos
    FROM
        Ordenes o
    LEFT JOIN 
        DetalleOrdenes d ON o.OrdenID = d.OrdenID
    WHERE
        (@OrdenID IS NULL OR o.OrdenID = @OrdenID)
    GROUP BY
        o.OrdenID, o.ClienteID, o.FechaOrden, o.Total, o.Estado
    ORDER BY
        o.FechaOrden DESC;
END
GO


