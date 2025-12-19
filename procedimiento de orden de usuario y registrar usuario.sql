CREATE PROCEDURE PA_ObtenerOrdenesPorUsuario
    @UsuarioID int

AS
BEGIN
    SELECT 
        OrdenID, 
        FechaOrden, 
        Total, 
        Estado 
    FROM Ordenes 
    WHERE ClienteID = @UsuarioID 
    ORDER BY FechaOrden DESC
END
GO

-- Procedimiento para registrar nuevos usuarios
CREATE PROCEDURE PA_RegistrarUsuario
    @Email varchar(100),
    @PasswordHash varchar(255),
    @NombreCompleto varchar(150)
AS
BEGIN
    INSERT INTO Usuarios (Email, PasswordHash, NombreCompleto, FechaRegistro, Activo)
    VALUES (@Email, @PasswordHash, @NombreCompleto, GETDATE(), 1)
END
GO