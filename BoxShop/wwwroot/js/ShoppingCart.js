var items = 0;
var addProductUrl = { productUrl : '@Url.Action("AddProductToCart", "store")'}

$(document).ready(function () {

    $('.buy').on("click", function () {

        var id = $(this).attr('id');
        items++;

        $.ajax({
            url: addProductUrl.productUrl,
            data: { "id": id },
            type: "GET",
            success: function (product) {
                $(product).insertAfter($('#product-placeholder'));
            },
            error: function () {
                alert('Error adding item to cart');
            },
        });
    });

    $(document).on("click", ".removeProduct", function () {
        $(this).closest("tr").remove();
        items--;
    });

    $('#checkoutBtn').on("click", function () {
    });

});