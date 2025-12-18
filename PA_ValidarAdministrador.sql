USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_ValidarAdministrador]    Script Date: 12/18/2025 12:26:50 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PA_ValidarAdministrador]
    @Correo NVARCHAR(100),
    @Clave NVARCHAR(100)
AS
BEGIN
    SELECT 
        UsuarioID, 
        NombreCompleto, 
        Email,
        1 AS RolID -- Forzamos el ID 1 para que el JS active el menú de Gestión
    FROM Usuarios 
    WHERE Email = @Correo 
      AND PasswordHash = @Clave
END
GO


