// LÓGICA DE CARRITO - FrontEnd/Scripts/carrito.js

$(document).ready(function () {
    
    // --- 1. Lógica para AÑADIR al Carrito desde el Catálogo ---
    // Selector: Se usa la clase 'btn-add-to-cart' definida en el Catálogo
    $('.btn-add-to-cart').click(function () {
        
        var button = $(this); // Referencia al botón clicado
        var productId = button.data('product-id');
        var quantity = 1; 

        // Deshabilitar botón temporalmente
        button.prop('disabled', true).text('Agregando...');

        // NOTA: Usamos window.location.origin para asegurar la URL base
        var url = window.location.origin + '/Carrito/Agregar'; 

        $.ajax({
            url: url,
            type: 'POST',
            data: {
                productoId: productId,
                cantidad: quantity
            },
            success: function (response) {
                if (response.success) {
                    // 1. Mostrar confirmación visual
                    button.text('¡Agregado!');
                    
                    // 2. Actualizar contador del carrito en la barra de navegación
                    // (Necesitas un elemento con ID 'cart-count' en tu _Layout)
                    $('#cart-count').text(response.count);

                    // Reestablecer el botón
                    setTimeout(function() {
                        button.html('<i class="fas fa-cart-plus"></i> Añadir al Carrito').prop('disabled', false);
                    }, 1000);

                } else {
                    alert('Error al agregar el producto: ' + response.message);
                    button.html('<i class="fas fa-cart-plus"></i> Añadir al Carrito').prop('disabled', false);
                }
            },
            error: function (xhr, status, error) {
                alert("Hubo un error de conexión con el servidor.");
                console.error(error);
                button.html('<i class="fas fa-cart-plus"></i> Añadir al Carrito').prop('disabled', false);
            }
        });
    });

    // --- 2. Lógica para ACTUALIZAR Cantidad en la Vista del Carrito (/Carrito/Index) ---
    // TAREA PENDIENTE
    $('.quantity-input').change(function() {
        // Implementar aquí la llamada AJAX a Carrito/ActualizarCantidad
        // y recargar o actualizar la tabla
    });

    // --- 3. Lógica para ELIMINAR Item en la Vista del Carrito (/Carrito/Index) ---
    // TAREA PENDIENTE
    $('.btn-remove-item').click(function() {
        // Implementar aquí la llamada AJAX a Carrito/Eliminar
        // y recargar o actualizar la tabla
    });
});