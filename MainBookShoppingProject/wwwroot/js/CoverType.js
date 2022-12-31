var dataTable;
$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    dataTable = $('#tdlData').DataTable({
        "ajax": {
            "url": "/Admin/CoverType/GetAll"
        },
        "columns": [
            { "data": "name", "width": "70%" },
            {
                "data": "id",
                "render": function(data) {
                    return `
                     <div class="text-primary">
                        <a class="btn btn-outline-info" href="/Admin/CoverType/Upsert/${data}">
                          <i class="fas fa-pen-nib"></i>
                        </a>
                        <a class="btn btn-outline-danger" onclick=Delete("/Admin/CoverType/Delete/${data}")>
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
        title: "want to delete this data?",
        text: "Delete information",
        buttons: true,
        icon: "warning",
        dangerModel: true
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
                    else {
                        toastr.error(data.message);
                    }
                }

            })

        }
    })
    
}
