const API_BASE_URL = 'https://localhost:44389/api/';

/**
 * Función para eliminar un proveedor haciendo una llamada DELETE a la API.
 * @param {number} proveedorId - El ID del proveedor a eliminar.
 * @param {string} nombreProveedor - El nombre del proveedor (solo para confirmación).
 */
async function eliminarProveedor(proveedorId, nombreProveedor) {
    if (!confirm(`¿Está seguro que desea ELIMINAR al proveedor "${nombreProveedor}" (ID: ${proveedorId})? Esto podría afectar a los productos asociados.`)) {
        return;
    }

    try {
        // Llama al endpoint DELETE api/proveedores/eliminar/{id}
        const response = await fetch(`${API_BASE_URL}proveedores/eliminar/${proveedorId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            alert(`✅ Proveedor "${nombreProveedor}" eliminado correctamente.`);
            // Recargar la página para actualizar la lista
            window.location.reload();
        } else if (response.status === 404) {
            alert(`❌ Proveedor ID ${proveedorId} no encontrado.`);
        } else {
            const errorText = await response.text();
            alert(`❌ Error al eliminar el proveedor. Detalle: ${errorText}`);
        }
    } catch (error) {
        console.error('Error de red al eliminar:', error);
        alert('Error de conexión con el servidor API.');
    }
}