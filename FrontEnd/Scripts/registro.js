// Scripts/registro.js

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('registroForm');
    const mensajeError = document.getElementById('mensajeError');

    form.onsubmit = async (e) => {
        e.preventDefault();

        // Limpiar mensajes previos
        mensajeError.style.display = 'none';
        mensajeError.innerText = '';

        const usuario = {
            NombreCompleto: document.getElementById('NombreCompleto').value,
            Email: document.getElementById('Email').value,
            // Se envía como PasswordHash para que el controlador lo reciba correctamente
            PasswordHash: document.getElementById('PasswordHash').value
        };

        try {
            // URL de tu API detectada en la configuración
            const response = await fetch('https://localhost:44389/api/autenticacion/registrar', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(usuario)
            });

            const result = await response.json();

            if (response.ok) {
                alert("¡Registro exitoso! Ya puedes iniciar sesión.");
                // Redirigir al Index del controlador Login
                window.location.href = '/Login/Index';
            } else {
                // Manejo de errores de la API (ej. Correo duplicado)
                mensajeError.innerText = result.ExceptionMessage || "Error al registrar el usuario.";
                mensajeError.style.display = 'block';
            }
        } catch (error) {
            console.error("Error en la petición:", error);
            mensajeError.innerText = "No se pudo conectar con el servidor de la API.";
            mensajeError.style.display = 'block';
        }
    };
});