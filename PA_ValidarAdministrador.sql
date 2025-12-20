CREATE   PROCEDURE [dbo].[PA_ValidarAdministrador]
    @Email varchar(100),
    @Password varchar(255)
AS
BEGIN
    SELECT 
        AdminID AS userId, 
        NombreCompleto AS nombre, 
        Rol AS role
    FROM dbo.Administradores
    WHERE Email = @Email 
      AND PasswordHash = @Password
      AND Activo = 1;
END
GO


