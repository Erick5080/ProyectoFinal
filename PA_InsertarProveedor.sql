USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_InsertarProveedor]    Script Date: 12/18/2025 12:37:29 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Procedimiento Almacenado para Registrar Proveedores
CREATE PROCEDURE [dbo].[PA_InsertarProveedor]
    -- Parámetros de entrada obligatorios
    @Nombre VARCHAR(100),
    @ContactoNombre VARCHAR(100) = NULL, -- Opcional
    @Telefono VARCHAR(20) = NULL,      -- Opcional
    @Email VARCHAR(100) = NULL         -- Opcional
AS
BEGIN
    -- Verificar si el proveedor ya existe por nombre
    IF EXISTS (SELECT 1 FROM Proveedores WHERE Nombre = @Nombre)
    BEGIN
        -- Usaremos un valor negativo para indicar un error de duplicidad
        SELECT -1 AS ProveedorID;
        RETURN;
    END

    -- Insertar el nuevo proveedor
    INSERT INTO Proveedores (
        Nombre, 
        ContactoNombre, 
        Telefono, 
        Email
    )
    VALUES (
        @Nombre, 
        @ContactoNombre, 
        @Telefono, 
        @Email
    );

    -- Devolver el ID del proveedor recién insertado
    SELECT SCOPE_IDENTITY() AS ProveedorID;
END
GO


