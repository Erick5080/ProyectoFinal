// FrontEnd/Scripts/catalogo.js

const API_BASE_URL = 'https://localhost:44389/api';

// A. FUNCIÓN PARA CREAR LA TARJETA DE PRODUCTO
function crearTarjetaProducto(producto) {
    // Limpiamos la ruta de la imagen (quita el ~ de ASP.NET si existe)
    const imagenLimpia = producto.ImagenURL ? producto.ImagenURL.replace('~', '') : '/img/no-image.png';

    return `
        <div class="col-lg-3 col-md-4 col-sm-6 mb-4">
            <div class="card h-100 shadow-sm product-card text-center">
                <img class="card-img-top" src="${imagenLimpia}" alt="${producto.Nombre}" 
                     style="height: 180px; object-fit: contain; padding: 10px;">
                <div class="card-body">
                    <h5 class="card-title text-truncate">${producto.Nombre}</h5>
                    <h4 class="text-primary">$${producto.PrecioUnitario.toFixed(2)}</h4>
                </div>
                <div class="card-footer bg-light border-top-0"> 
                    <button class="btn btn-primary btn-add-to-cart" data-product-id="${producto.ProductoID}">
                        <i class="fas fa-cart-plus"></i> Añadir al Carrito
                    </button>
                </div>
            </div>
        </div>
    `;
}

// B. FUNCIÓN PARA CARGAR EL CATÁLOGO DESDE LA API
async function cargarCatalogo() {
    const container = document.getElementById('catalogo-principal');
    if (!container) return;

    container.innerHTML = '<div class="col-12 text-center text-muted"><div class="spinner-border spinner-border-sm"></div> Cargando productos...</div>';

    try {
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

        if (productos.length === 0) {
            container.innerHTML = '<div class="col-12 text-center text-muted">No hay productos disponibles en este momento.</div>';
            return;
        }

        // Generamos el HTML de todas las tarjetas
        let htmlContent = '';
        productos.forEach(producto => {
            htmlContent += crearTarjetaProducto(producto);
        });

        container.innerHTML = htmlContent;

    } catch (error) {
        console.error('Error al cargar el catálogo:', error);
        container.innerHTML = '<div class="col-12 text-center text-danger">⚠️ Error de conexión con la API. Asegúrate de que el proyecto API esté ejecutándose.</div>';
    }
}

// C. INICIALIZACIÓN
document.addEventListener('DOMContentLoaded', () => {
    cargarCatalogo();
});