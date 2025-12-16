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

    // --- 2. Lógica para ACTUALIZAR Cantidad en la Vista del Carrito (/Carrito/Index)
    $('.quantity-input').change(function () {

        // Se dispara cuando el valor del input de cantidad cambia
        $(document).on('change', '.quantity-input', function () {
            var input = $(this);
            var productId = input.data('id');
            var newQuantity = parseInt(input.val());

            // Validación básica: no permitir cantidades negativas o no numéricas
            if (isNaN(newQuantity) || newQuantity < 1) {
                newQuantity = 1;
                input.val(1);
            }

            // Llamada AJAX para actualizar
            $.ajax({
                url: window.location.origin + '/Carrito/ActualizarCantidad',
                type: 'POST',
                data: {
                    productoId: productId,
                    cantidad: newQuantity
                },
                success: function (response) {
                    if (response.success) {
                        // Actualizar el subtotal de la fila
                        var row = input.closest('tr');
                        row.find('.item-subtotal').text(response.newItemSubtotalText);

                        // Actualizar el total de la compra en el pie de página
                        $('#total-compra').text(response.newTotalText);

                        // Actualizar el contador global del carrito
                        $('#cart-count').text(response.newCount);

                        // Si la cantidad es 0, recargar para eliminar la fila si la lógica del controlador lo permite
                        if (newQuantity === 0) {
                            window.location.reload();
                        }
                    } else {
                        alert('Error al actualizar la cantidad: ' + response.message);
                    }
                },
                error: function () {
                    alert("Error de conexión al actualizar.");
                }
            });
        });

        // --- 3. Lógica para ELIMINAR Item en la Vista del Carrito (/Carrito/Index) ---
        // Se dispara al hacer clic en el botón de la papelera
        $('.btn-remove-item').click(function () {

        $(document).on('click', '.btn-remove-item', function () {
            if (!confirm("¿Está seguro de que desea eliminar este producto del carrito?")) {
                return;
            }

            var button = $(this);
            var productId = button.data('id');

            // Llamada AJAX para eliminar
            $.ajax({
                url: window.location.origin + '/Carrito/Eliminar',
                type: 'POST',
                data: {
                    productoId: productId
                },
                success: function (response) {
                    if (response.success) {
                        // Eliminar la fila de la tabla con animación
                        button.closest('tr').fadeOut(300, function () {
                            $(this).remove();

                            // Si no quedan filas en la tabla, recargar para mostrar el mensaje de carrito vacío
                            if ($('.quantity-input').length === 0) {
                                window.location.reload();
                            } else {
                                // Actualizar el total y el contador
                                $('#total-compra').text(response.newTotalText);
                                $('#cart-count').text(response.newCount);
                            }
                        });

                    } else {
                        alert('Error al eliminar el producto: ' + response.message);
                    }
                },
                error: function () {
                    alert("Error de conexión al eliminar.");
                }
            });
        });
    });

    });
});