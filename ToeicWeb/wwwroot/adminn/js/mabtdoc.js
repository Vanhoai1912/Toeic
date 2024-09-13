﻿var dataTable;

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
            "url": "/Admin/BTdoc/GetAll"
        },
        "columns": [
            { "data": "id", "width": "25%", "className": "text-start" },
            { "data": "tieu_de", "width": "30%" },
            { "data": "part", "width": "15%", "className": "text-start" },

            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="#" onclick="Edit(${data})"
                        class="btn btn-primary ms-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                        <a onClick=Delete('/Admin/BTdoc/Delete/${data}')
                        class="btn btn-danger ms-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>
                    `;
                },
                "width": "40%"
            }
        ]
    });
}

// Create data
function Create() {
    var result = Validate();
    if (result == false) {
        return false;
    }

    var formData = new FormData();
    formData.append('ExcelFile', $('#ExcelFile')[0].files[0]);
    formData.append('Tieu_de', $('#Tieu_de').val());
    formData.append('Part', $('#Part').val());

    $.ajax({
        url: '/Admin/BTdoc/Create',
        data: formData,
        type: 'POST',
        contentType: false,
        processData: false,

        success: function (response) {
            if (!response.success) {
                toastr.error(response.message);
            } else {
                HideModal();
                loadDataTable();
                toastr.success(response.message);
            }
        },
        error: function (xhr, status, error) {
            toastr.error(xhr.responseJSON.message);
        }
    });
}

// Edit
function Edit(id) {
    $.ajax({
        url: '/Admin/BTdoc/Edit?id=' + id,
        type: 'get',
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        success: function (response) {
            if (response == null || response == undefined) {
                alert('Không thể đọc dữ liệu');
            } else if (response.length == 0) {
                alert('Không có dữ liệu với id' + id);
            } else {
                $('#BaitapdocModal').modal('show');
                $('#modalTitle').text('Sửa bài tập đọc ');
                $('#Save').css('display', 'none');
                $('#Update').css('display', 'block');
                $('#Id').val(response.id);  // Lưu id trong trường ẩn
                $('#Tieu_de').val(response.tieu_de);
                $('#Part').val(response.part);
                // Hiển thị file Excel nếu có
                $('#ExcelFile').val(response.filePath);
            }
        },
        error: function () {
            alert('Không thể đọc dữ liệu');
        }
    });
}

// Update
function Update() {
    var result = Validate();
    if (result == false) {
        return false;
    }

    var formData = new FormData();
    var tieuDe = $('#Tieu_de').val();
    var part = $('#Part').val();
    var excelFile = $('#ExcelFile')[0].files[0];  // Lấy file excel từ input

    var id = $('#Id').val();  // Lấy id từ trường ẩn

    formData.append('id', id);
    formData.append('tieu_de', tieuDe);
    formData.append('part', part);

    if (excelFile) {
        formData.append('FileExcel', excelFile);
    }

    $.ajax({
        url: '/Admin/BTdoc/Update',
        type: 'post',
        data: formData,
        processData: false,  // Không xử lý dữ liệu dưới dạng string
        contentType: false,  // Để ajax tự xác định content-type cho FormData
        success: function (data) {
            if (data.success) {
                HideModal();
                loadDataTable();
                toastr.success(data.message);
            } else {
                toastr.error(data.message);

            }
        },
        error: function () {
            alert('Lỗi khi cập nhật dữ liệu');
        }
    });
}
// Delete data
function Delete(url) {
    Swal.fire({
        title: 'Bạn có chắc chắn muốn xóa không?',
        text: "Bạn sẽ không thể hoàn tác!",
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
                        
                        Swal.fire({
                            title: "Đã xóa!",
                            text: "Tập tin đã bị xóa.",
                            icon: "success"
                        });
                        dataTable.ajax.reload();
                        //clearFormData();
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
    $('#modalTitle').text('Thêm bài tập đọc mới');
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
    $('#Part').prop('selectedIndex', 0);

    $('#Tieu_de').val('');
    $('#Tieu_de').css('border-color', 'lightgrey');
    $('#ExcelFile').val('');
    $('#ExcelFile').css('border-color', 'lightgrey');
}

function Validate() {
    var isValid = true;
    if ($('#Part').prop('selectedIndex') == 0) {
        $('#Part').css('border-color', 'Red');
        isValid = false;
    } else {
        $('#Part').css('border-color', 'lightgrey');
    }
    if ($('#Tieu_de').val().trim() == "") {
        $('#Tieu_de').css('border-color', 'Red');
        isValid = false;
    } else {
        $('#Tieu_de').css('border-color', 'lightgrey');
    }
    if ($('#ExcelFile').get(0).files.length === 0) {
        $('#ExcelFile').css('border-color', 'Red');
        isValid = false;
    } else {
        $('#ExcelFile').css('border-color', 'lightgrey');
    }
    return isValid;
}

$('#Part').change(function () {
    Validate();
})
$('#Tieu_de').change(function () {
    Validate();
})
$('#ExcelFile').change(function () {
    Validate();
})