// Scripts/loginAdmin.js
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('formLoginAdmin');

    form.onsubmit = async (e) => {
        e.preventDefault();

        const model = {
            Email: document.getElementById('txtEmail').value,
            Password: document.getElementById('txtPassword').value
        };

        try {
            // Llamada a la API
            const response = await fetch('https://localhost:44389/api/autenticacion/adminlogin', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(model)
            });

            if (response.ok) {
                const data = await response.json();
                // Redirigimos al controlador de la Web para guardar la sesión de C#
                window.location.href = `/Login/EstablecerSesionAdmin?adminId=${data.userId}&nombre=${data.nombre}&rol=${data.role}`;
            } else {
                alert("Credenciales de administrador incorrectas o cuenta inactiva.");
            }
        } catch (error) {
            console.error("Error:", error);
            alert("No se pudo conectar con la API.");
        }
    };
});