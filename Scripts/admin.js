//const { json } = require("d3");

$(document).ready(function () {
    // INSERT Category
    $("#AddCategoryForm").submit(function (e) {
        e.preventDefault();

        var formUrl = $(this).attr('action');
        var formData = $(this).serialize();
        var checkForm = $(this).closest("form");
        var token = $("#menu").attr("token");

        if (checkForm.valid()) {
            $.ajax({
                type: "PUT",
                url: formUrl,
                data: formData,
                headers: {
                    'X-RequestVerificationToken': token
                },
                success: function (data) {
                    if (data.Success) {
                        SendMessage(data.Message, 'success');
                        LoadCategories();
                    }
                    else {
                        SendMessage(data.Message, 'error');
                    }
                }
            });
        }
    });

    // DELETE Category
    $(document).on('click', '.delete-category', function (e) {
        e.preventDefault();
        var name = $(this).attr("category-name");
        var url = "/api/category/delete/" + $(this).attr("category-id");
        var token = $("#menu").attr("token");

        bootbox.confirm({
            title: "Delete this category?",
            message: "Do you want to delete category <b class=\"text-danger\">" + name + "</b>?<br/> This cannot be undone.",
            buttons: {
                cancel: {
                    label: 'Cancel'
                },
                confirm: {
                    label: 'Confirm',
                    className: 'btn-danger m-l-20'
                }
            },
            callback: function (result) {
                console.log('This was logged in the callback: ' + result);
                if (result) {
                    $.ajax({
                        type: "DELETE",
                        url: url,
                        dataType: "json",
                        headers: {
                            'X-RequestVerificationToken': token
                        }
                    })
                    .done(function (data) {
                        if (data.Success) {
                            SendMessage(data.Message, 'info');
                            LoadCategories();
                        }
                        else {
                            SendMessage(data.Message, 'error');
                        }
                    });

                }
            }
        }).find('.modal-content').css({
            'margin-top': function () {
                var w = $(window).height();
                var b = $(".modal-dialog").height();
                // should not be (w-h)/2
                var h = (w - b) / 2 - 100;
                return h + "px";
            }
        });
    });
});

// Load Categories
function LoadCategories() {
    $.ajax({
        // LINK GetCategoryTree
        url: '/api/category/getTree',
        method: 'get',
        dataType: 'json',
        success: function (data) {
            $('#menu').html("");
            buildMenu($('#menu'), data);
            buildOptionMenu(data);
        }
    });
}

function buildOptionMenu(items) {
    $.each(items, function () {
        var l = 0;
        if (this.Category.ParentCategoryId)
            l = 3;
        var option = $('<option value="' + this.Category.Id + '">' + "&nbsp;".repeat(l) + this.Category.Name + '</option>');
        option.appendTo("#ParentCategoryId");
        if (this.SubCategories && this.SubCategories.length > 0) {
            buildOptionMenu(this.SubCategories)
        }
    });
}

function buildMenu(parent, items) {
    $.each(items, function () {
        if (!this.Category.Active) {
            this.Category.Name = this.Category.Name + " (disabled)";
        }

		var li = $('<li class="li-item"><span class="category" category-id="' + this.Category.Id + '">' + this.Category.Name + '</span>' +
            ' &nbsp;<a href="#" category-id="' + this.Category.Id + '" class="delete-category link-info" category-name="' + this.Category.Name +'"><i class="icofont icofont-close"></i></a></li>');
        li.appendTo(parent);

        if (this.SubCategories && this.SubCategories.length > 0) {
            var ul = $('<ul class="ul-inner"></ul>');
            ul.appendTo(li);
            buildMenu(ul, this.SubCategories);
        }
    });
};


function SendMessage(message, type) {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-left",
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