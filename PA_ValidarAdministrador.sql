CREATE PROCEDURE PA_ValidarAdministrador
    @Correo NVARCHAR(100),
    @Clave NVARCHAR(100)
AS
BEGIN
    -- Seleccionamos las columnas con sus nombres REALES de la tabla Usuarios
    SELECT 
        UsuarioID, 
        NombreCompleto, -- En tu tabla se llama NombreCompleto, no Nombre
        Email           -- En tu tabla se llama Email, no Correo
    FROM Usuarios 
    WHERE Email = @Correo      -- Usamos Email en lugar de Correo
      AND PasswordHash = @Clave -- Usamos PasswordHash en lugar de Clave
END