CREATE PROCEDURE PA_ValidarUsuario
    @Email varchar(100),
    @Password varchar(255)
AS
BEGIN
    SELECT UsuarioID, NombreCompleto, Email 
    FROM Usuarios 
    WHERE Email = @Email AND PasswordHash = @Password AND Activo = 1
END