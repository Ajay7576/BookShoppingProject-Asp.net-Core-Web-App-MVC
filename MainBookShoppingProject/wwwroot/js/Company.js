var dataTable;
$(document).ready(function (){
    loadDataTable();
})
function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": ({
            "url":"/Admin/Company/GetAll"
        }),
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "city", "width": "15%" },
            { "data": "state", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "isAuthorizedCompany", "width": "10%",
                "render": function (data) {
                    if (data) {
                        return `<input type="checkbox"checked disabled />`;
                    }
                    else {
                        return `<input type="checkbox"disabled />`;
                    }
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                     <div class="text-center">
                    <a class="btn btn-outline-info" href="/Admin/Company/Upsert/${data}">
                     <i class="fas fa-pen-nib"></i>
                    </a>
                    <a class="btn btn-outline-danger" OnClick=Delete("/Admin/Company/Delete/${data}")>
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
        title: "Do you want Delete",
        text: "Data Deleted",
        buttons: true,
        dangerMode: true,
        icon:"success"
    }).then((willdelete) => {
        if (willdelete) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else
                        toastr.error(data.message);
                }
            })
        }
    })
}