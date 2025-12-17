const API_BASE_URL = 'https://localhost:44389/api';

// A. FUNCIÓN PARA CREAR LA TARJETA DE PRODUCTO
function crearTarjetaProducto(producto) {
    // CORRECCIÓN: Quitamos el "~" de la ruta de la imagen para que el navegador la entienda
    const imagenLimpia = producto.ImagenURL.replace('~', '');

    return `
        <div class="col-lg-3 col-md-4 col-sm-6 mb-4">
            <div class="card h-100 shadow-sm product-card text-center">
                <img class="card-img-top" src="${imagenLimpia}" alt="${producto.Nombre}" style="height: 180px; object-fit: contain; padding: 10px;">
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
    if (!container) return; // Seguridad por si el ID no existe

    container.innerHTML = '<div class="col-12 text-center text-muted">Cargando...</div>';

    try {
        // CORRECCIÓN SINTAXIS: El objeto de configuración (headers) DEBE ir dentro del fetch
        const response = await fetch(`${API_BASE_URL}/productos/obtener`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.status === 404) {
            container.innerHTML = '<div class="col-12 text-center text-danger">No se encontraron productos activos.</div>';
            return;
        }

        if (!response.ok) {
            throw new Error(`Error HTTP! Estado: ${response.status}`);
        }

        const productos = await response.json();
        console.log("Productos cargados exitosamente:", productos);

        let htmlContent = '';
        productos.forEach(producto => {
            htmlContent += crearTarjetaProducto(producto);
        });

        container.innerHTML = htmlContent;

    } catch (error) {
        console.error('Error al cargar el catálogo:', error);
        container.innerHTML = '<div class="col-12 text-center text-danger">⚠️ Error de conexión con la API.</div>';
    }
}

// C. LÓGICA DEL CARRO DE COMPRAS
const CARRITO_KEY = 'eCommerceCarrito';

function actualizarContadorCarrito(totalProductos) {
    const contador = document.getElementById('contador-carrito');
    if (contador) {
        contador.innerText = totalProductos;
    }
}

async function agregarAlCarrito(productoId) {
    try {
        const response = await fetch(`${API_BASE_URL}/productos/obtener/${productoId}`);
        if (!response.ok) throw new Error("Error al obtener producto");

        const producto = await response.json();
        let carrito = JSON.parse(localStorage.getItem(CARRITO_KEY)) || [];
        const productoExistente = carrito.find(item => item.ProductoID === productoId);

        if (productoExistente) {
            productoExistente.Cantidad += 1;
        } else {
            carrito.push({
                ProductoID: productoId,
                Cantidad: 1,
                PrecioUnitarioEnVenta: producto.PrecioUnitario,
                Nombre: producto.Nombre,
                ImagenURL: producto.ImagenURL
            });
        }

        localStorage.setItem(CARRITO_KEY, JSON.stringify(carrito));
        alert(`"${producto.Nombre}" añadido!`);
        actualizarContadorCarrito(carrito.length);

    } catch (error) {
        console.error("Error al añadir al carrito:", error);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    cargarCatalogo();
    let carrito = JSON.parse(localStorage.getItem(CARRITO_KEY)) || [];
    actualizarContadorCarrito(carrito.length);
});