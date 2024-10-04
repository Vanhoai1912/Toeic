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
    if (!result) {
        return false;
    }
    var formData = new FormData();
    var newExcelFile = $('#NewExcelFile').get(0).files[0];
    var newImageFiles = $('#NewImageFile').get(0).files;

    if (newExcelFile) {
        formData.append('ExcelFile', newExcelFile);
    }
   
    if (newImageFiles.length > 0) {
        for (var i = 0; i < newImageFiles.length; i++) {
            formData.append('Image_bai_doc', newImageFiles[i]);
        }
    }
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
                ClearData();
                toastr.success(response.message);
            }
        },
        error: function (xhr) {
            console.error(xhr);
            toastr.error(xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.');
        }
    });
}

// Edit
function Edit(id) {
    $.ajax({
        url: '/Admin/BTdoc/Edit?id=' + id,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        success: function (response) {
            if (response == null || response == undefined) {
                alert('Không thể đọc dữ liệu');
            } else if (response.success === false) {
                alert(response.message);
            } else {

                originalData = {
                    tieu_de: response.data.tieu_de,
                    part: response.data.part
                };

                $('#BaitapdocModal').modal('show');
                $('#modalTitle').text('Sửa bài tập đọcccc');
                $('#Save').css('display', 'none');
                $('#Update').css('display', 'block');


                // Điền các giá trị vào modal
                $('#Id').val(response.data.id);
                $('#Tieu_de').val(response.data.tieu_de);
                $('#Part').val(response.data.part);

                // Hiển thị thông tin file Excel hiện tại nếu có
                if (response.filePath) {
                    var fileName = response.filePath.split('\\').pop().split('/').pop();
                    $('#ExcelFile').text(fileName);
                    $('#ExcelFileInfo').show();
                } else {
                    $('#ExcelFileInfo').hide();
                }

                // Hiển thị số file ảnh đã thêm
                if (response.numberOfImages) {
                    $('#ImageFiles').text(response.numberOfImages);
                    $('#NumberOfImages').show();
                } else {
                    $('#NumberOfImages').hide();
                }


                // Reset phần input file mới
                $('#NewExcelFile').val('');
                $('#NewImageFile').val('');
            }
        },
        error: function () {
            alert('Không thể đọc dữ liệu');
        }
    });
}

//Update data
function Update() {
    var tieuDe = $('#Tieu_de').val();
    var part = $('#Part').val();
    var excelFile = $('#NewExcelFile')[0].files[0];  // Lấy file excel từ input
    var newImageFiles = $('#NewImageFile').get(0).files;

    var id = $('#Id').val();  // Lấy id từ trường ẩn

    var isChanged = true;
    if (tieuDe != originalData.tieu_de || part != originalData.part || (excelFile !== undefined && excelFile !== null) || newImageFiles.length > 0) {
        isChanged = false;
    }

    if (isChanged) {
        toastr.info("Không có thay đổi nào để cập nhật");
        return;
    }

    var formData = new FormData();
    formData.append('id', id);
    formData.append('tieu_de', tieuDe);
    formData.append('part', part);

    if (excelFile) {
        formData.append('ExcelFile', excelFile);
    }
    if (newImageFiles.length > 0) {
        for (var i = 0; i < newImageFiles.length; i++) {
            formData.append('Image_bai_doc', newImageFiles[i]);
        }
    }

    $.ajax({
        url: '/Admin/BTdoc/Update',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
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
                        clearFormData();
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
    $('#ExcelFileInfo').hide();
    $('#NumberOfImages').hide();
}



function ClearData() {
    $('#Part').val('');
    $('#Part').css('border-color', 'lightgrey');
    $('#Part').prop('selectedIndex', 0);

    $('#Tieu_de').val('');
    $('#Tieu_de').css('border-color', 'lightgrey');

    // Xóa thông tin file cũ và reset input file mới
    $('#ExcelFile').text('');
    $('#ExcelFileInfo').hide();
    $('#NewExcelFile').val('');
    $('#NewExcelFile').css('border-color', 'lightgrey');
}

function Validate() {
    var isValid = true;

    if ($('#Tieu_de').val().trim() == "") {
        $('#Tieu_de').css('border-color', 'Red');
        $('#Tieu_deError').text('Vui lòng nhập tiêu đề.').show();
        isValid = false;
    } else {
        $('#Tieu_de').css('border-color', 'lightgrey');
        $('#Tieu_deError').text('').hide();
    }

    if ($('#Part').prop('selectedIndex') == 0 || $('#Part').val() === "") {
        $('#Part').css('border-color', 'Red');
        $('#PartError').text('Vui lòng chọn Part.').show();
        isValid = false;
    } else {
        $('#Part').css('border-color', 'lightgrey');
        $('#PartError').text('').hide();
    }

    var newExcelFileInput = $('#NewExcelFile').get(0);
    if (newExcelFileInput && newExcelFileInput.files.length === 0) {
        $('#NewExcelFile').css('border-color', 'Red');
        $('#ExcelFileError').text('Vui lòng chọn file Excel.').show();
        isValid = false;
    } else {
        $('#NewExcelFile').css('border-color', 'lightgrey');
        $('#ExcelFileError').text('').hide();
    }

    return isValid;
}

function ClearData() {
    $('#Tieu_de').val('');
    $('#Part').val('');
    $('#NewExcelFile').val('');

    // Ẩn các lỗi và khôi phục lại màu 
    $('#Tieu_deError').text('').hide();
    $('#PartError').text('').hide();
    $('#ExcelFileError').text('').hide();

    $('#Tieu_de').css('border-color', 'lightgrey');
    $('#Part').css('border-color', 'lightgrey');
    $('#NewExcelFile').css('border-color', 'lightgrey');

    // Xóa thông tin file cũ và reset input file mới
    $('#ExcelFile').text('');
    $('#ExcelFileInfo').hide();
    $('#NewExcelFile').val('');
    $('#NewExcelFile').css('border-color', 'lightgrey');
}

$('#Tieu_de').on('input', function () {
    if ($(this).val().trim() !== "") {
        $('#Tieu_de').css('border-color', 'lightgrey');
        $('#Tieu_deError').text('').hide();
    }
});

$('#Part').on('change', function () {
    if ($(this).val() !== "") {
        $('#Part').css('border-color', 'lightgrey');
        $('#PartError').text('').hide();
    }
});

$('#NewExcelFile').on('change', function () {
    if (this.files.length > 0) {
        $('#NewExcelFile').css('border-color', 'lightgrey');
        $('#ExcelFileError').text('').hide();
    }
});