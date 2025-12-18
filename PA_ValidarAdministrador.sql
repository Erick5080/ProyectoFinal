CREATE PROCEDURE PA_ValidarAdministrador
    @Correo NVARCHAR(100),
    @Clave NVARCHAR(100)
AS
BEGIN
    -- Buscamos al usuario que coincida con credenciales y que sea Admin (RolID = 1)
    SELECT UsuarioID, Nombre, Correo, RolID
    FROM Usuarios 
    WHERE Correo = @Correo 
      AND Clave = @Clave 
      AND RolID = 1; -- Filtramos para que solo entren administradores
END