// Scripts/usuarioPanel.js

document.addEventListener('DOMContentLoaded', function () {
    // Inicializar la carga de órdenes
    cargarOrdenes();
});


async function cargarOrdenes() {
    // 1. Obtener el ID del usuario desde el campo oculto de la vista
    const usuarioId = document.getElementById('sessionUserId').value;
    const tabla = document.getElementById('tablaOrdenes');

    if (!usuarioId || usuarioId === "") {
        tabla.innerHTML = '<tr><td colspan="5" class="text-center text-danger">Error: No se detectó sesión de usuario.</td></tr>';
        return;
    }

    // 2. Llamada a la API (Asegúrate de que el puerto coincida con tu API)
    // Se usa el endpoint que filtra por usuarioId
    fetch(`https://localhost:44389/api/autenticacion/ordenes/${usuarioId}`)
        .then(response => {
            if (!response.ok) throw new Error("Error en la respuesta de la API");
            return response.json();
        })
        .then(data => {
            let html = '';

            if (data && data.length > 0) {
                data.forEach(orden => {
                    html += `
                        <tr>
                            <td><strong>#${orden.OrdenID}</strong></td>
                            <td>${new Date(orden.Fecha).toLocaleDateString()}</td>
                            <td>$${orden.Total.toFixed(2)}</td>
                            <td><span class="badge bg-success">${orden.Estado}</span></td>
                            <td class="text-center">
                                <button class="btn btn-sm btn-outline-primary">Ver Detalle</button>
                            </td>
                        </tr>`;
                });
            } else {
                html = '<tr><td colspan="5" class="text-center py-4 text-muted">No tienes pedidos registrados aún.</td></tr>';
            }

            tabla.innerHTML = html;
        })
        .catch(error => {
            console.error("Error:", error);
            tabla.innerHTML = '<tr><td colspan="5" class="text-center text-danger py-4">No se pudieron cargar las órdenes. Inténtalo más tarde.</td></tr>';
        });
}

// Cargar automáticamente al entrar a la página
document.addEventListener('DOMContentLoaded', cargarOrdenes);
/**
 * Define el color del badge según el estado de la orden
 */
function getBadgeColor(estado) {
    switch (estado?.toLowerCase()) {
        case 'completado': return 'bg-success';
        case 'pendiente': return 'bg-warning text-dark';
        case 'cancelado': return 'bg-danger';
        default: return 'bg-secondary';
    }
}

function verDetalle(id) {
    alert("Próximamente: Detalle de la orden #" + id);
}