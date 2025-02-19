var dataTable;

$(document).ready(function () {
    $('#BaithiModal').on('hide.bs.modal', function () {
        HideModal();
    });
    loadDataTable();
});

// Xử lý sự thay đổi của ExamType
//function handleExamTypeChange() {
//    $('#ExamType').on('change', function () {
//        var examType = $(this).val();
//        var audioFileSection = $('#audioFileSection'); // Div chứa input thêm file nghe

//        // Nếu loại bài thi là "đọc", ẩn mục thêm file nghe
//        if (examType == 'đọc') {
//            audioFileSection.hide();
//            $('#NewAudioFile').val(''); // Xóa giá trị đã chọn
//            $('#NewAudioFile').css('border-color', 'lightgrey');
//            $('#AudioFileError').text('').hide();
//        } else {
//            audioFileSection.show();
//        }

//        if (examType != "") {
//            $('#ExamType').css('border-color', 'lightgrey');
//            $('#ExamTypeError').text('').hide();
//        }
//    });

//    // Kiểm tra và xử lý ngay khi trang load
//    var examType = $('#ExamType').val();
//    if (examType == 'đọc') {
//        $('#audioFileSection').hide(); // Ẩn phần file nghe nếu trang load với loại "đọc"
//    } else {
//        $('#audioFileSection').show();
//    }
//}


// Create data
function Create() {
    var result = Validate();
    if (!result) {
        return false;
    }

    var formData = new FormData();
    var newExcelFile = $('#NewExcelFile').get(0).files[0];
    var newImageFiles = $('#NewImageFile').get(0).files;
    var newAudioFiles = $('#NewAudioFile').get(0).files;


    formData.append('ExcelFile', newExcelFile);

    for (var i = 0; i < newImageFiles.length; i++) {
        formData.append('ImageFile', newImageFiles[i]);
    }

    // Chỉ thêm file âm thanh nếu không phải bài thi "đọc"
    var examType = $('#ExamType').val();
    if (examType != 'đọc') {

        
        for (var i = 0; i < newAudioFiles.length; i++) {
            formData.append('AudioFile', newAudioFiles[i]);
        }
    }

    formData.append('Tieu_de', $('#Tieu_de').val());
    formData.append('ExamType', $('#ExamType').val());

    $.ajax({
        url: '/Admin/BThi/Create',
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
        url: '/Admin/BThi/Edit?id=' + id,
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
                    tieu_de: response.data.tieu_de,
                    examType: response.data.examType
                };

                $('#BaithiModal').modal('show');
                $('#modalTitle').text('Sửa bài thi');
                $('#Save').css('display', 'none');
                $('#Update').css('display', 'block');

                // Điền các giá trị vào modal
                $('#Id').val(response.data.id);
                $('#Tieu_de').val(response.data.tieu_de);
                $('#ExamType').val(response.data.examType);

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

                // Hiển thị số file nghe đã thêm
                if (response.numberOfAudios) {
                    $('#AudioFiles').text(response.numberOfAudios);
                    $('#NumberOfAudios').show();
                } else {
                    $('#NumberOfAudios').hide();
                }

                // Reset phần input file mới
                $('#NewExcelFile').val('');
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

    var tieuDe = $('#Tieu_de').val();
    var examType = $('#ExamType').val();
    var excelFile = $('#NewExcelFile').get(0).files[0];  // Lấy file excel từ input
    var newImageFiles = $('#NewImageFile').get(0).files;
    var newAudioFiles = $('#NewAudioFile').get(0).files;


    var id = $('#Id').val();  // Lấy id từ trường ẩn

    // Kiểm tra có thay đổi nào không
    var isChanged = true;
    if (tieuDe != originalData.tieu_de || examType != originalData.examType || (excelFile !== undefined && excelFile !== null) ||
        newImageFiles.length > 0 || newAudioFiles.length > 0) {
        isChanged = false;
    }
    if (isChanged) {
        toastr.info("Không có thay đổi nào để cập nhật");
        return;
    }

    var formData = new FormData();
    formData.append('id', id);
    formData.append('tieu_de', tieuDe);
    formData.append('ExamType', examType);

    if (excelFile) {
        formData.append('ExcelFile', excelFile);
    }

    if (newImageFiles.length > 0) {
        for (var i = 0; i < newImageFiles.length; i++) {
            formData.append('ImageFile', newImageFiles[i]);
        }
    }

    // Chỉ thêm file âm thanh nếu không phải bài thi "đọc"
    if (examType != 'đọc') {
        for (var i = 0; i < newAudioFiles.length; i++) {
            formData.append('AudioFile', newAudioFiles[i]);
        }
    }

    $.ajax({
        url: '/Admin/BThi/Update',
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

// Read data
// Read data
function loadDataTable() {
    if ($.fn.dataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/BThi/GetAll"
        },
        "columns": [
            { "data": "id", "width": "25%", "className": "text-start" },
            { "data": "tieu_de", "width": "30%" },
            { "data": "examType", "width": "15%", "className": "text-start" },

            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">

                        <a href="#" onclick="GetUserbyTestResult(${data})"
                        class="btn btn-primary ms-2"> View User</a>

                        <a href="#" onclick="Edit(${data})"
                        class="btn btn-primary ms-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                        <a onClick=Delete('/Admin/BThi/Delete/${data}')
                        class="btn btn-danger ms-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>
                    `;
                },
                "width": "40%"
            }
        ]
    });
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

    if ($('#ExamType').prop('selectedIndex') == 0 || $('#ExamType').val() === "") {
        $('#ExamType').css('border-color', 'Red');
        $('#ExamTypeError').text('Vui lòng chọn loại bài thi.').show();
        isValid = false;
    } else {
        $('#ExamType').css('border-color', 'lightgrey');
        $('#ExamTypeError').text('').hide();
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

$('#btnAdd').click(function () {
    $('#BaithiModal').modal('show');
    $('#modalTitle').text('Thêm bài thi mới');
    $('#Save').css('display', 'block');
    $('#Update').css('display', 'none');
    //handleExamTypeChange(); // Đảm bảo xử lý lại khi mở modal
});

var isHidingModal = false;

function HideModal() {
    if (isHidingModal) return; // Ngăn chặn vòng lặp
    isHidingModal = true;

    ClearData();
    $('#BaithiModal').modal('hide');
    $('#ExcelFileInfo').hide();
    $('#NumberOfImages').hide();
    $('#NumberOfAudios').hide();

    isHidingModal = false; // Đặt lại biến cờ
}
function ClearData() {
    $('#Tieu_de').val('');
    $('#ExamType').val('');
    $('#NewExcelFile').val('');
    $('#NewImageFile').val('');
    $('#NewAudioFile').val('');

    // Ẩn các lỗi và khôi phục lại màu 
    $('#Tieu_deError').text('').hide();
    $('#ExamTypeError').text('').hide();
    $('#ExcelFileError').text('').hide();
    $('#AudioFileError').text('').hide();

    $('#Tieu_de').css('border-color', 'lightgrey');
    $('#ExamType').css('border-color', 'lightgrey');
    $('#NewExcelFile').css('border-color', 'lightgrey');
    $('#NewAudioFile').css('border-color', 'lightgrey');
}

$('#Tieu_de').on('input', function () {
    if ($(this).val().trim() !== "") {
        $('#Tieu_de').css('border-color', 'lightgrey');
        $('#Tieu_deError').text('').hide();
    }
});

$('#ExamType').on('change', function () {
    if ($(this).val() !== "") {
        $('#ExamType').css('border-color', 'lightgrey');
        $('#ExamTypeError').text('').hide();
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
function GetUserbyTestResult(id) {
    // Gọi API để lấy dữ liệu người dùng đã làm bài thi
    $.ajax({
        url: '/Admin/BThi/GetUsersByTestResult?maBaiThiId=' + id,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        success: function (response) {
            if (response.success) {
                // Hiển thị dữ liệu người dùng trong modal hoặc một vị trí nào đó
                var totalUsers = response.data.length;
                $('#totalUsers').text('Tổng số người đã làm bài thi: ' + totalUsers);
                ShowUserResults(response.data);
            } else {
                toastr.error(response.message);
            }

        },
        error: function () {
            toastr.error("Có lỗi xảy ra khi lấy dữ liệu.");
        }
    });
}
function ShowUserResults(data) {
    var tableBody = $('#userResultsBody');
    tableBody.empty(); // Xóa dữ liệu cũ


    data.forEach(function (userResult) {
        var completionDate = new Date(userResult.completionDate);

        // Định dạng ngày tháng theo dd/MM/yyyy
        var formattedDate = completionDate.toLocaleDateString('vi-VN');
        var row = '<tr>' +
            '<td>' + (userResult.userName) + '</td>' + // Kiểm tra xem UserName có tồn tại không
            '<td>' + (userResult.email) + '</td>' +
            '<td>' + (userResult.correctAnswers) + '</td>' +
            '<td>' + (userResult.incorrectAnswers) + '</td>' +
            '<td>' + (userResult.skippedQuestions) + '</td>' +
            '<td>' + (formattedDate) + '</td>' +
            '</tr>';
        tableBody.append(row);
    });


    // Hiển thị modal
    $('#UserResultsModal').modal('show');
}