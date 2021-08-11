$(document).ready(function () {
    // Add product to cart
    $(".addToCart-btn").click(function (e) {
        e.preventDefault();
        var url = $(this).attr("href");

        $.ajax({
            type: "POST",
            url: url,
            dataType: "json",
        })
            .done(function (data) {
                if (data.Success) {
                    SendMessage(data.Message, "success");
                }
                else {
                    SendMessage(data.Message, "info");
                }
                $("#cart").load(location.href + " #cart>*", "");
            });
    });

    // Toggle side navigation
    $("#closeNav").click(function () {
        var margin = $(".side_nav").css("margin-left");
        $(this).toggleClass("active-nav");
        if (margin != "0px") {
            $(".side_nav").css("margin-left", "0");
            $(".content").css("padding-left", "360px");
        }
        else {
            $(".side_nav").css("margin-left", "-310px");
            $(".content").css("padding-left", "50px");
        }
    });

    // Responsive side nav
    $(window).resize(function () {
        if ($(window).width() < 1200) {
            $("#closeNav").removeClass('active-nav');
        } else {
            $("#closeNav").addClass('active-nav');
        }
    }).resize();

    // Submit form on change
    $(".order-list").change(function () {
        $("#search-product-form").submit();
    });
    $(".page-count").change(function () {
        $("#search-product-form").submit();
    });

    // Remove empty parameters from url
    $("#search-product-form").submit(function (e) {
        e.preventDefault();
        var query = $(this).serializeArray().filter(function (i) {
            return i.value;
        });
        window.location.href = $(this).attr('action') + (query ? '?' + $.param(query) : '');
    });


    // Product page quantity buttons
    $(document).on("click", ".plus", function () {
        var x = +$(this).parent().children(".product-quantity-value").val();
        $(this).parent().children(".product-quantity-value").val(x + 1).change();
    });

    $(document).on("click", ".minus", function () {
        var x = +$(this).parent().children(".product-quantity-value").val();
        if (x > 1)
            $(this).parent().children(".product-quantity-value").val(x - 1).change();
    });

    $(document).on("change", ".product-quantity-value", function () {
        if ($(this).val() < 1) {
            $(this).val(1);
        }
    });

    $(document).on("change", ".cart-item-qnt", function () {
        var productID = $(this).attr("for-product");
        var qnt = $(this).val();
        console.log("product: " + productID + " / value: " + qnt);

        $.ajax({
            type: "POST",
            url: "Cart/Update",
            dataType: "json",
            data: {
                id: productID,
                qnt: qnt
            }
        })
            .done(function (data) {
                if (!data.Success) {
                    SendMessage(data.Message, "info");
                }
                $("#user-cart").load(location.href + " #user-cart>*", "");
            });
    });
});

// Message function
function SendMessage(message, type) {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    toastr[type](message);
}