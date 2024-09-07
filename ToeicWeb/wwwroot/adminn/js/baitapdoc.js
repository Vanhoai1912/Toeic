var dataTable;

$(document).ready(function () {
    loadDataTable();
});

// Read data
function loadDataTable() {
    if ($.fn.dataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Quanlybaidoc/GetAll"
        },
        "columns": [
            { "data": "id", "width": "25%", "className": "text-start" },
            { "data": "part", "width": "45%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="#" onclick="Edit(${data})"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                        <a onClick=Delete('/Admin/Quanlybaidoc/Delete/${data}')
                        class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>
                    `;
                },
                "width": "40%"
            }
        ]
    });
}

// Insert data
function Insert() {
    var result = Validate();
    if (result == false) {
        return false;
    }

    var formData = new Object();
    formData.id = $('#Id').val();
    formData.part = $('#Part').val();

    $.ajax({
        url: '/Admin/Quanlybaidoc/Insert',
        data: formData,
        type: 'post',
        success: function (response) {
            if (response == null || response == undefined || response.length == 0) {
                /* alert('Không thể lưu mã bài tập đọc mới');*/
                toastr.error(response.message);

            } else {
                HideModal();
                loadDataTable();
                //alert(response);
                toastr.success(response.message);
            }
        },
        error: function () {
            //alert('Không thể lưu mã bài tập đọc mới');
            toastr.error(response.message);
        }
    });

}

// Edit
function Edit(id) {
    $.ajax({
        url: '/Admin/Quanlybaidoc/Edit?id=' + id,
        type: 'get',
        contentType: 'application/json; charset=uft-8',
        datatype: 'json',
        success: function (response) {
            if (response == null || response == undefined) {
                alert('Không thể đọc dữ liệu');
            } else if (response.length == 0) {
                alert('Không có dữ liệu với id' + id);
            } else {
                $('#BaitapdocModal').modal('show');
                $('#modalTitle').text('Sửa mã bài tập đọc ');
                $('#Save').css('display', 'none');
                $('#Update').css('display', 'block');
                $('#Id').val(response.id);
                $('#Part').val(response.part);

            }
        },
        error: function () {
            alert('Không thể đọc dữ liệu');
        }

    });
}

// Update data
function Update() {
    var result = Validate();
    if (result == false) {
        return false;
    }
    var formData = new Object();
    formData.id = $('#Id').val();
    formData.part = $('#Part').val();

    $.ajax({
        url: '/Admin/Quanlybaidoc/Update',
        data: formData,
        type: 'post',
        success: function (response) {
            if (response == null || response == undefined || response.length == 0) {
                //alert('Không thể lưu mã bài tập đọc mới');
                toastr.error(response.message);

            } else {
                HideModal();
                loadDataTable();
                //alert(response);
                toastr.success(response.message);

            }
        },
        error: function () {
            //alert('Không thể lưu mã bài tập đọc mới');
            toastr.error(response.message);

        }
    });
}

// Delete data
function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}


$('#btnAdd').click(function () {
    $('#BaitapdocModal').modal('show');
    $('#modalTitle').text('Thêm mã bài tập đọc mới');
    $('#Save').css('display', 'block');
    $('#Update').css('display', 'none');
});

function HideModal() {
    ClearData();
    $('#BaitapdocModal').modal('hide');
}

function ClearData() {
    $('#Part').val('');
    $('#Part').css('border-color', 'lightgrey');
}

function Validate() {
    var isValid = true;
    if ($('#Part').val().trim() == "") {
        $('#Part').css('border-color', 'Red');
        isValid = false;
    } else {
        $('#Part').css('border-color', 'lightgrey');
    }
    return isValid;
}

$('#Part').change(function () {
    Validate();
})