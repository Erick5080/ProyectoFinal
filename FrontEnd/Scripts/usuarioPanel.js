// Scripts/usuarioPanel.js

document.addEventListener('DOMContentLoaded', function () {
    // Inicializar la carga de órdenes
    cargarOrdenes();
});

/**
 * Obtiene las órdenes del cliente desde el controlador de C# 
 * y las renderiza en la tabla dinámica.
 */
async function cargarOrdenes() {
    const tbody = document.getElementById('tablaOrdenes');
    const url = tbody.getAttribute('data-url'); // Obtenemos la URL desde un data-attribute

    try {
        const response = await fetch(url);

        if (!response.ok) throw new Error("Error en la petición");

        const data = await response.json();

        // Si no hay datos o hay un error devuelto por el servidor
        if (!data || data.length === 0 || data.error) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="5" class="text-center py-4 text-muted">
                        <i class="fas fa-info-circle"></i> No tienes pedidos registrados aún.
                    </td>
                </tr>`;
            return;
        }

        tbody.innerHTML = ''; // Limpiar el spinner de carga

        data.forEach(orden => {
            const fecha = new Date(orden.FechaOrden).toLocaleDateString();
            const total = typeof orden.Total === 'number' ? orden.Total.toFixed(2) : "0.00";

            tbody.innerHTML += `
                <tr>
                    <td class="fw-bold text-primary">#${orden.OrdenID}</td>
                    <td>${fecha}</td>
                    <td class="fw-bold text-dark">$${total}</td>
                    <td>
                        <span class="badge ${getBadgeColor(orden.Estado)}">
                            ${orden.Estado}
                        </span>
                    </td>
                    <td>
                        <button class="btn btn-sm btn-light border" onclick="verDetalle(${orden.OrdenID})">
                            <i class="fas fa-eye text-primary"></i> Detalle
                        </button>
                    </td>
                </tr>
            `;
        });
    } catch (error) {
        console.error("Error al cargar órdenes:", error);
        tbody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center text-danger py-4">
                    <i class="fas fa-exclamation-triangle"></i> No se pudo cargar el historial. Intente más tarde.
                </td>
            </tr>`;
    }
}

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