// Вернет данные вместе с токеном против подделки
function getRequestVerificationToken(data) {
    data.__RequestVerificationToken = $("input[name=__RequestVerificationToken]").val();
    return data;
}

// Запрос на удаление элемента
function deleteItem(id, uri, event) {
    event.preventDefault();
    swal({
            title: "Attention",
            text: "Do you really want to delete an item?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes",
            cancelButtonText: "No",
            closeOnConfirm: false,
            closeOnCancel: true
    },
    function (isConfirm) {
        if (isConfirm) {
            $.ajax({
                cache: false,
                async: true,
                url: uri,
                type: "POST",
                dateType: JSON,
                data: getRequestVerificationToken({ "id": id }),
                success: function (result) {
                    swal({
                        title: result.title,
                        text: result.message,
                        type: result.messageType
                    },
                    function () {
                        if (result.messageType === "success") {
                            $(`[data-id="${id}"]`).remove();
                        }
                    });
                }
            });
        }
    });
}