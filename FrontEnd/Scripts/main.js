// main.js - Lógica global para Navbar, Carrito y Sesión

// 1. Función global para actualizar el numerito del carrito en el Navbar
function actualizarGlobalCarrito() {
    const carrito = JSON.parse(localStorage.getItem('eCommerceCarrito')) || [];
    const contador = document.getElementById('contador-carrito');
    if (contador) {
        // Sumamos las cantidades de todos los productos
        const total = carrito.reduce((sum, item) => sum + item.Cantidad, 0);
        contador.innerText = total;
    }
}

// 2. Función para verificar si hay un usuario logueado y cambiar el Navbar
function verificarSesionGlobal() {
    const usuarioJson = localStorage.getItem('usuarioSesion');
    const seccion = document.getElementById('seccion-usuario');

    if (usuarioJson && seccion) {
        const usuario = JSON.parse(usuarioJson);
        seccion.innerHTML = `
            <div class="nav-item dropdown">
                <a class="nav-link dropdown-toggle text-white" href="#" id="userDropdown" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                    👤 Hola, ${usuario.Nombre}
                </a>
                <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="userDropdown">
                    <li><a class="dropdown-item" href="#" onclick="cerrarSesion()">Cerrar Sesión</a></li>
                </ul>
            </div>
        `;
    }
}

// 3. Función para cerrar sesión
function cerrarSesion() {
    localStorage.removeItem('usuarioSesion');
    // Redirigir al inicio (ajusta la ruta según tu proyecto)
    window.location.href = '/Home/Index'; 
}

// Ejecutar cuando el HTML esté listo
document.addEventListener('DOMContentLoaded', function () {
    actualizarGlobalCarrito();
    verificarSesionGlobal();
});