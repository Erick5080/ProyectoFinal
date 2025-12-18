USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_ObtenerProveedores]    Script Date: 12/18/2025 12:41:02 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Procedimiento Almacenado para obtener la información de los proveedores
CREATE PROCEDURE [dbo].[PA_ObtenerProveedores]
    @ProveedorID INT = NULL 
AS
BEGIN
    SELECT
        ProveedorID,
        Nombre,
        ContactoNombre,
        Telefono,
        Email
    FROM
        Proveedores
    WHERE
        (@ProveedorID IS NULL OR ProveedorID = @ProveedorID)
    ORDER BY
        Nombre;
END
GO


