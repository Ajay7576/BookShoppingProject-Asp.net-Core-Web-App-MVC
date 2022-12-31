var dataTable;
$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "email", "width": "20%" },
            { "data": "phoneNumber", "width": "20%" },
            { "data": "company.name", "width": "20%" },
            { "data": "role", "width": "20%" },
            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        // user locked
                        return ` 
                          <div class="text-center">
                          <a onclick=LockunLock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer">
                           UnLock
                          </a>
                          </div>
                              `;
                    }
                    else {
                        return ` 
                          <div class="text-center">
                          <a onclick=LockunLock('${data.id}') class="btn btn-success text-white" style="cursor:pointer">
                           Lock
                          </a>
                          </div>
                           `;
                    }
                }
            }

        ]
    })
}

function LockunLock(id) {
        $.ajax({
            type: "POST",
            url: "/Admin/User/LockUnLock",
            data: JSON.stringify(id),
            contentType: "application/json",
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