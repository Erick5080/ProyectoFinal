const API_BASE_URL = 'https://localhost:XXXXX/api';

// A. FUNCIÓN PARA CREAR LA TARJETA DE PRODUCTO
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

// B. FUNCIÓN PARA CARGAR EL CATÁLOGO COMPLETO
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

// C. LÓGICA DEL CARRO DE COMPRAS
// Constante para la clave del carrito en localStorage
const CARRITO_KEY = 'eCommerceCarrito';

// Función auxiliar para actualizar el número de productos en el nav
function actualizarContadorCarrito(totalProductos) {
    // Necesitarás un elemento en tu layout principal (ej: <span id="contador-carrito">0</span>)
    const contador = document.getElementById('contador-carrito');
    if (contador) {
        contador.innerText = totalProductos;
    }
}

// Función principal para añadir un producto
async function agregarAlCarrito(productoId) {
    // 1. Asumimos una cantidad de 1 por clic en el botón del catálogo
    const cantidad = 1;

    try {
        // A. Obtener el precio y otros datos del producto de la API
        const response = await fetch(`${API_BASE_URL}/productos/obtener/${productoId}`);

        if (!response.ok) {
            throw new Error(`Producto ${productoId} no encontrado o inactivo.`);
        }

        const producto = await response.json();
        const precioUnitarioEnVenta = producto.PrecioUnitario; // Precio fijo en el momento de la venta

        // B. Obtener el carrito actual del localStorage
        let carrito = JSON.parse(localStorage.getItem(CARRITO_KEY)) || [];

        // C. Verificar si el producto ya está en el carrito
        const productoExistente = carrito.find(item => item.ProductoID === productoId);

        if (productoExistente) {
            // Si existe, solo incrementa la cantidad
            productoExistente.Cantidad += cantidad;
        } else {
            // Si no existe, agrégalo al carrito con la información necesaria
            carrito.push({
                ProductoID: productoId,
                Cantidad: cantidad,
                PrecioUnitarioEnVenta: precioUnitarioEnVenta,
                Nombre: producto.Nombre, // Útil para mostrar en la vista del carrito
                ImagenURL: producto.ImagenURL // Útil para mostrar en la vista del carrito
            });
        }

        // D. Guardar el carrito actualizado en localStorage
        localStorage.setItem(CARRITO_KEY, JSON.stringify(carrito));

        // E. Retroalimentación y actualización del contador
        alert(`"${producto.Nombre}" añadido al carrito!`);
        actualizarContadorCarrito(carrito.length);

    } catch (error) {
        console.error("Error al añadir al carrito:", error);
        alert(`Hubo un error al añadir el producto al carrito. Detalle: ${error.message}`);
    }
}

// D. INICIALIZACIÓN
document.addEventListener('DOMContentLoaded', () => {
    cargarCatalogo(); // Ejecuta la carga del catálogo

    // Al cargar la página, inicializa el contador del carrito
    let carrito = JSON.parse(localStorage.getItem(CARRITO_KEY)) || [];
    actualizarContadorCarrito(carrito.length);
});