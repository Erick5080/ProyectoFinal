CREATE PROCEDURE PA_ObtenerProductoPorID
    @ProductoID int
AS
BEGIN
    SELECT * FROM Productos WHERE ProductoID = @ProductoID
END