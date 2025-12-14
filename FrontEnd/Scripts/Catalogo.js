const API_BASE_URL = 'https://localhost:XXXXX/api';

// =================================================================
// A. FUNCIÓN PARA CREAR LA TARJETA DE PRODUCTO (Genera HTML)
// =================================================================
function crearTarjetaProducto(producto) {
    // Utilizamos la misma estructura de tarjeta que en la sección de destacados
    return `
                <div class="col-lg-3 col-md-4 col-sm-6 mb-4">
                    <div class="card h-100 shadow-sm product-card text-center">
                        <img class="card-img-top" src="${producto.ImagenURL}" alt="${producto.Nombre}" style="height: 180px; object-fit: contain; padding: 10px;">
                        <div class="card-body">
                            <h5 class="card-title text-truncate">${producto.Nombre}</h5>
                            <h4 class="text-primary">$${producto.PrecioUnitario.toFixed(2)}</h4>
                        </div>
                        <div class="card-footer bg-light border-top-0">
                            <button class="btn btn-primary btn-sm btn-block" onclick="agregarAlCarrito(${producto.ProductoID})">Añadir al Carrito</button>
                        </div>
                    </div>
                </div>
            `;
}

// =================================================================
// B. FUNCIÓN PARA CARGAR EL CATÁLOGO COMPLETO (Consumo del GET)
// =================================================================
async function cargarCatalogo() {
    const container = document.getElementById('catalogo-principal');
    container.innerHTML = '<div class="col-12 text-center text-muted">Cargando...</div>';

    try {
        // Llama al endpoint de productos activos
        const response = await fetch(`${API_BASE_URL}/productos/obtener`);

        if (response.status === 404) {
            container.innerHTML = '<div class="col-12 text-center text-danger">No se encontraron productos activos.</div>';
            return;
        }
        if (!response.ok) {
            throw new Error(`Error HTTP! Estado: ${response.status}`);
        }

        const productos = await response.json();
        let htmlContent = '';

        productos.forEach(producto => {
            htmlContent += crearTarjetaProducto(producto);
        });

        container.innerHTML = htmlContent;

    } catch (error) {
        console.error('Error al cargar el catálogo:', error);
        // Si falla, es probable que la API no esté corriendo.
        container.innerHTML = '<div class="col-12 text-center text-danger">⚠️ Error de conexión. Asegúrese de que el proyecto API esté en ejecución.</div>';
    }
}

// =================================================================
// C. FUNCIÓN BÁSICA DE CARRO DE COMPRAS (Simulación)
// =================================================================
function agregarAlCarrito(productoId) {
    // Aquí iría la lógica del Front-End para manejar el carrito (añadir, sumar cantidades).
    // Luego, al finalizar la compra, se llamaría al endpoint de /ordenes/registrar.
    alert(`Producto ID ${productoId} añadido al carrito. ¡Listo para la venta!`);
}

// =================================================================
// D. INICIALIZACIÓN
// =================================================================
document.addEventListener('DOMContentLoaded', () => {
    cargarCatalogo(); // Ejecuta la carga del catálogo cuando la página esté lista
});