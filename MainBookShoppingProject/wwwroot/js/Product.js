var dataTable;
$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        //"aLengthMenu": [[1, 2, 3, 4,10], [1, 2, 3, 4,10, "All"]],
        //"iDisplayLength":4,
        "ajax": {
            "url":"/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "discription", "width": "15% " },
            { "data": "author", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            {
                "data": "price", "width": "15%",
                "render": function (data, type, row) {
                    return row.title + '<br>(' + row.price + ')' + '<br>(' + row.author + ')';
                }
                },
            {
                "data": "id",
                "render": function (data) {
                    return `
                       <div class="text-center">
                    <a class="btn btn-outline-primary" href="/Admin/Product/Upsert/${data}">
                   <i class="fas fa-pen-nib"></i>
                     </a>
                    <a class="btn btn-outline-danger" Onclick=Delete("/Admin/Product/Delete/${data}")>
                    <i class="fas fa-skull-crossbones"></i>
                    </a>
                      </div>
                        `;
                }

            }
        ]
    })
}
function Delete(url) {
    swal({
        title: "want to delete data?",
        text: "Delete information!!",
        buttons: true,
        icon: "warning",
        dangerMode: true
    }).then((willdelete) => {
        if (willdelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}
