var dataTable;





$(document).ready(function () {
    $('#BaivocaModal').on('hide.bs.modal', function () {
        HideModal();
    });
    loadDataTable();

});


// Read data
function loadDataTable() {
    if ($.fn.dataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Voca/GetAll"
        },
        "columns": [
            { "data": "id", "width": "25%", "className": "text-start" },
            { "data": "ten_bai", "width": "35%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="#" onclick="Edit(${data})"
                        class="btn btn-primary ms-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                        <a onClick=Delete('/Admin/Voca/Delete/${data}')
                        class="btn btn-danger ms-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>
                    `;
                },
                "width": "40%"
            }
        ]
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
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
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
    var newImageFileMavoca = $('#NewImageFileMavoca').get(0).files[0];
    var newAudioFiles = $('#NewAudioFile').get(0).files;

    if (newExcelFile) {
        formData.append('ExcelFile', newExcelFile);
    }
    for (var i = 0; i < newImageFiles.length; i++) {
        formData.append('ImageFile', newImageFiles[i]);
    }
    for (var i = 0; i < newAudioFiles.length; i++) {
        formData.append('AudioFile', newAudioFiles[i]);
    }
    if (newImageFileMavoca) {
        formData.append('ImageFileMavoca', newImageFileMavoca);
    }
    formData.append('Ten_bai', $('#Ten_bai').val());

    $.ajax({
        url: '/Admin/Voca/Create',
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

// Edit data
function Edit(id) {
    $.ajax({
        url: '/Admin/Voca/Edit?id=' + id,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        success: function (response) {
            if (response == null || response == undefined) {
                alert('Không thể đọc dữ liệu');
            } else if (response.success === false) {
                alert(response.message);
            } else {

                // Lưu dữ liệu ban đầu
                originalData = {
                    ten_bai: response.data.ten_bai,
                };

                $('#BaivocaModal').modal('show');
                $('#modalTitle').text('Sửa bài từ vựng');
                $('#Save').css('display', 'none');
                $('#Update').css('display', 'block');

                // Điền các giá trị vào modal
                $('#Id').val(response.data.id);
                $('#Ten_bai').val(response.data.ten_bai);

                // Hiển thị thông tin file Excel hiện tại nếu có
                if (response.filePath) {
                    var fileName = response.filePath.split('\\').pop().split('/').pop();
                    $('#ExcelFile').text(fileName);
                    $('#ExcelFileInfo').show();
                } else {
                    $('#ExcelFileInfo').hide();
                }

                // Hiển thị file ảnh voca
                if (response.filePathImageVoca) {
                    var fileNameVoca = response.filePathImageVoca.split('\\').pop().split('/').pop(); // Sử dụng đúng biến filePathImageVoca
                    $('#ImageFileMavoca').text(fileNameVoca);
                    $('#ImageVocaFileInfo').show();
                } else {
                    $('#ImageVocaFileInfo').hide();
                }


                // Hiển thị số file ảnh đã thêm
                if (response.numberOfImages) {
                    $('#ImageFiles').text(response.numberOfImages);
                    $('#NumberOfImages').show();
                } else {
                    $('#NumberOfImages').hide();
                }

                // Hiển thị số file nghe đã thêm
                if (response.numberOfAudios) {
                    $('#AudioFiles').text(response.numberOfAudios);
                    $('#NumberOfAudios').show();
                } else {
                    $('#NumberOfAudios').hide();
                }

                // Reset phần input file mới
                $('#NewExcelFile').val('');
                $('#NewImageFileMavoca').val('');
                $('#NewImageFile').val('');
                $('#NewAudioFile').val('');


            }
        },
        error: function () {
            alert('Không thể đọc dữ liệu');
        }
    });
}

// Update data
function Update() {
    var tenbai = $('#Ten_bai').val();
    var excelFile = $('#NewExcelFile').get(0).files[0];  
    var newImageMavocaFile = $('#NewImageFileMavoca').get(0).files[0];  
    var newImageFiles = $('#NewImageFile').get(0).files;
    var newAudioFiles = $('#NewAudioFile').get(0).files;

    var id = $('#Id').val();  // Lấy id từ trường ẩn

    // Kiểm tra có thay đổi nào không
    var isChanged = true;
    if (tenbai != originalData.ten_bai || (excelFile !== undefined && excelFile !== null) ||
        newImageFiles.length > 0 || (newImageMavocaFile !== undefined && newImageMavocaFile !== null) || newAudioFiles.length > 0) {
        isChanged = false;
    }
    if (isChanged) {
        toastr.info("Không có thay đổi nào để cập nhật");
        return;
    }

    var formData = new FormData();
    formData.append('id', id);
    formData.append('ten_bai', tenbai);

    if (excelFile) {
        formData.append('ExcelFile', excelFile);
    }

    if (newImageFiles.length > 0) {
        for (var i = 0; i < newImageFiles.length; i++) {
            formData.append('ImageFile', newImageFiles[i]);
        }
    }

    if (newAudioFiles.length > 0) {
        for (var i = 0; i < newAudioFiles.length; i++) {
            formData.append('AudioFile', newAudioFiles[i]);
        }
    }
    if (newImageMavocaFile) {
        formData.append('ImageFileMavoca', newImageMavocaFile);
    }


    $.ajax({
        url: '/Admin/Voca/Update',
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

function Validate() {
    var isValid = true;

    if ($('#Ten_bai').val().trim() == "") {
        $('#Ten_bai').css('border-color', 'Red');
        $('#Ten_baiError').text('Vui lòng nhập tiêu đề.').show();
        isValid = false;
    } else {
        $('#Ten_bai').css('border-color', 'lightgrey');
        $('#Ten_baiError').text('').hide();
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

    var newAudioFileInput = $('#NewAudioFile').get(0);
    if (newAudioFileInput && newAudioFileInput.files.length === 0) {
        $('#NewAudioFile').css('border-color', 'Red');
        $('#AudioFileError').text('Vui lòng chọn file nghe.').show();
        isValid = false;
    } else {
        $('#NewAudioFile').css('border-color', 'lightgrey');
        $('#AudioFileError').text('').hide();
    }

    var newImageFileInput = $('#NewImageFile').get(0);
    if (newImageFileInput && newImageFileInput.files.length === 0) {
        $('#NewImageFile').css('border-color', 'Red');
        $('#ImageFileError').text('Vui lòng chọn file ảnh.').show();
        isValid = false;
    } else {
        $('#NewImageFile').css('border-color', 'lightgrey');
        $('#ImageFileError').text('').hide();
    }

    var newImageFileMavocaInput = $('#NewImageFileMavoca').get(0);
    if (newImageFileMavocaInput && newImageFileMavocaInput.files.length === 0) {
        $('#NewImageFileMavoca').css('border-color', 'Red');
        $('#ImageFileMavocaError').text('Vui lòng chọn file ảnh.').show();
        isValid = false;
    } else {
        $('#NewImageFileMavoca').css('border-color', 'lightgrey');
        $('#ImageFileMavocaError').text('').hide();
    }

    return isValid;
}



$('#btnAdd').click(function () {
    $('#BaivocaModal').modal('show');
    $('#modalTitle').text('Thêm bài từ vựng mới');
    $('#Save').css('display', 'block');
    $('#Update').css('display', 'none');
});

var isHidingModal = false; // Khai báo biến chỉ một lần

function HideModal() {
    if (isHidingModal) return; // Ngăn chặn vòng lặp
    isHidingModal = true;

    ClearData();
    $('#BaivocaModal').modal('hide');
    $('#ExcelFileInfo').hide();
    $('#ImageVocaFileInfo').hide();
    $('#NumberOfImages').hide();
    $('#NumberOfAudios').hide();

    isHidingModal = false; // Đặt lại biến cờ
}


function ClearData() {
    $('#Ten_bai').val('');
    $('#NewExcelFile').val('');
    $('#NewImageFile').val('');
    $('#NewAudioFile').val('');
    $('#NewImageFileMavoca').val('');

    
    // Ẩn các lỗi và khôi phục lại màu 
    $('#Ten_baiError').text('').hide();
    $('#ExcelFileError').text('').hide();
    $('#ImageFileError').text('').hide();
    $('#AudioFileError').text('').hide();
    $('#ImageFileMavocaError').text('').hide();

    $('#Ten_bai').css('border-color', 'lightgrey');
    $('#NewExcelFile').css('border-color', 'lightgrey');
    $('#NewImageFile').css('border-color', 'lightgrey');
    $('#NewAudioFile').css('border-color', 'lightgrey');
    $('#NewImageFileMavoca').css('border-color', 'lightgrey');

}

$('#Ten_bai').on('input', function () {
    if ($(this).val().trim() !== "") {
        $('#Ten_bai').css('border-color', 'lightgrey');
        $('#Ten_baiError').text('').hide();
    }
});

$('#NewExcelFile').on('change', function () {
    if (this.files.length > 0) {
        $('#NewExcelFile').css('border-color', 'lightgrey');
        $('#ExcelFileError').text('').hide();
    }
});

$('#NewAudioFile').on('change', function () {
    if (this.files.length > 0) {
        $('#NewAudioFile').css('border-color', 'lightgrey');
        $('#AudioFileError').text('').hide();
    }
});

$('#NewImageFile').on('change', function () {
    if (this.files.length > 0) {
        $('#NewImageFile').css('border-color', 'lightgrey');
        $('#ImageFileError').text('').hide();
    }
});

$('#NewImageFileMavoca').on('change', function () {
    if (this.files.length > 0) {
        $('#NewImageFileMavoca').css('border-color', 'lightgrey');
        $('#ImageFileMavocaError').text('').hide();
    }
});






