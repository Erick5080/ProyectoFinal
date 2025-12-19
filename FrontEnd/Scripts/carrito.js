// FrontEnd/Scripts/carrito.js

$(document).ready(function () {
    const URL_BASE = window.location.origin;

    // --- 1. AGREGAR PRODUCTO (Desde el Catálogo) ---
    // Usamos delegación de eventos $(document).on para que funcione incluso si 
    // los productos se cargan dinámicamente después de que la página cargue.
    $(document).on('click', '.btn-add-to-cart', function (e) {
        e.preventDefault();
        
        var button = $(this);
        var productId = button.data('product-id');
        var quantity = 1; 

        // Feedback visual inmediato
        button.prop('disabled', true).text('Agregando...');

        $.ajax({
            url: URL_BASE + '/Carrito/Agregar',
            type: 'POST',
            data: {
                productoID: productId,
                cantidad: quantity
            },
            success: function (response) {
                if (response.success) {
                    // Actualiza el numerito del carrito en el navbar (ID en tu layout)
                    // Según tu main.js el ID es 'contador-carrito'
                    $('#contador-carrito').text(response.count);
                    
                    button.addClass('btn-success').removeClass('btn-primary').text('¡Agregado!');
                    
                    setTimeout(function() {
                        button.prop('disabled', false)
                              .addClass('btn-primary')
                              .removeClass('btn-success')
                              .html('<i class="fas fa-cart-plus"></i> Añadir al Carrito');
                    }, 1500);
                } else {
                    alert('Error: ' + response.message);
                    button.prop('disabled', false).text('Reintentar');
                }
            },
            error: function () {
                alert("Error de comunicación con el servidor al agregar.");
                button.prop('disabled', false).text('Añadir al Carrito');
            }
        });
    });

    // --- 2. ACTUALIZAR CANTIDAD (Desde la Vista del Carrito /Carrito/Index) ---
    $(document).on('change', '.quantity-input', function () {
        var input = $(this);
        var productId = input.data('id');
        var newQuantity = parseInt(input.val());

        if (isNaN(newQuantity) || newQuantity < 1) {
            newQuantity = 1;
            input.val(1);
        }

        $.ajax({
            url: URL_BASE + '/Carrito/ActualizarCantidad',
            type: 'POST',
            data: {
                productoId: productId,
                cantidad: newQuantity
            },
            success: function (response) {
                if (response.success) {
                    // Actualizar subtotal de la fila y total general
                    var row = input.closest('tr');
                    row.find('.item-subtotal').text(response.newItemSubtotalText);
                    $('#total-compra').text(response.newTotalText);
                    $('#contador-carrito').text(response.newCount);

                    if (newQuantity === 0) {
                        window.location.reload();
                    }
                } else {
                    alert(response.message);
                }
            }
        });
    });

    // --- 3. ELIMINAR PRODUCTO (Desde la Vista del Carrito /Carrito/Index) ---
    $(document).on('click', '.btn-remove-item', function (e) {
        e.preventDefault();
        var button = $(this);
        var productId = button.data('id');

        if (!confirm("¿Eliminar este producto del carrito?")) return;

        $.ajax({
            url: URL_BASE + '/Carrito/Eliminar',
            type: 'POST',
            data: { productoId: productId },
            success: function (response) {
                if (response.success) {
                    // Eliminar fila con efecto
                    button.closest('tr').fadeOut(300, function () {
                        $(this).remove();
                        
                        // Si ya no hay productos, recargar para mostrar mensaje de "Carrito Vacío"
                        if ($('.btn-remove-item').length === 0) {
                            window.location.reload();
                        } else {
                            $('#total-compra').text(response.newTotalText);
                            $('#contador-carrito').text(response.newCount);
                        }
                    });
                }
            }
        });
    });
});