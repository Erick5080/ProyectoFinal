USE [ProyectoFinal]
GO

/****** Object:  StoredProcedure [dbo].[PA_ValidarUsuario]    Script Date: 12/19/2025 9:28:53 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PA_ValidarUsuario]
    @Email varchar(100),
    @Password varchar(255)
AS
BEGIN
    SELECT UsuarioID, NombreCompleto, Email 
    FROM Usuarios 
    WHERE Email = @Email AND PasswordHash = @Password AND Activo = 1
END
GO


