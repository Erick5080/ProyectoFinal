
// Obtener la URL base del API. Usamos una variable global definida en el layout 
// o una constante aquí (tendrás que actualizar el puerto real en esta línea).
const API_BASE_URL = 'https://localhost:XXXXX/api/';

// Función para desactivar un producto
async function desactivarProducto(productoId, nombreProducto) {
    if (!confirm(`¿Está seguro que desea DESACTIVAR el producto "${nombreProducto}" (ID: ${productoId})? Esto lo ocultará del catálogo.`)) {
        return;
    }

    try {
        // Llama al endpoint DELETE api/productos/eliminar/{id}
        const response = await fetch(`${API_BASE_URL}productos/eliminar/${productoId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            alert(`✅ Producto "${nombreProducto}" desactivado correctamente.`);
            // Recargar la página para ver el cambio
            window.location.reload();
        } else if (response.status === 404) {
            alert(`❌ Producto ID ${productoId} no encontrado.`);
        } else {
            const errorText = await response.text();
            alert(`❌ Error al desactivar el producto. Detalle: ${errorText}`);
        }
    } catch (error) {
        console.error('Error de red al desactivar:', error);
        alert('Error de conexión con el servidor API.');
    }
}