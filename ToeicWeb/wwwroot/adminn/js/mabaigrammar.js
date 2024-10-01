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
            "url": "/Admin/Grammar/GetAll"
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
                        <a onClick=Delete('/Admin/Grammar/Delete/${data}')
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

function Create() {
    var result = Validate();
    if (!result) {
        return false;
    }

    var formData = new FormData();
    var newImageFileGra = $('#NewImageFileGra').get(0).files[0];

    if (newImageFileGra) {
        formData.append('ImageFileGra', newImageFileGra);
    }

    // Lấy nội dung từ TinyMCE
    var noi_dung_value = tinymce.get('Noi_dung').getContent();
    formData.append('Noi_dung', noi_dung_value);

    formData.append('Ten_bai', $('#Ten_bai').val());

    $.ajax({
        url: '/Admin/Grammar/Create',
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



function Validate() {
    var isValid = true;

    if ($('#Ten_bai').val().trim() == "") {
        $('#Ten_bai').css('border-color', 'Red');
        $('#Ten_baiError').text('Vui lòng nhập tên bài.').show();
        isValid = false;
    } else {
        $('#Ten_bai').css('border-color', 'lightgrey');
        $('#Ten_baiError').text('').hide();
    }

    // Kiểm tra nội dung từ TinyMCE
    var noi_dung_value = tinymce.get('Noi_dung').getContent();
    if (noi_dung_value.trim() == "") {
        $('#Noi_dung').css('border-color', 'Red');
        $('#Noi_dungError').text('Vui lòng nhập nội dung.').show();
        isValid = false;
    } else {
        $('#Noi_dung').css('border-color', 'lightgrey');
        $('#Noi_dungError').text('').hide();
    }

    var newImageFileGraInput = $('#NewImageFileGra').get(0);
    if (newImageFileGraInput && newImageFileGraInput.files.length === 0) {
        $('#NewImageFileGra').css('border-color', 'Red');
        $('#ImageFileGraError').text('Vui lòng chọn file ảnh.').show();
        isValid = false;
    } else {
        $('#NewImageFileGra').css('border-color', 'lightgrey');
        $('#ImageFileGraError').text('').hide();
    }

    return isValid;
}



$('#btnAdd').click(function () {
    $('#BaigrammarModal').modal('show');
    $('#modalTitle').text('Thêm bài ngữ pháp mới');
    $('#Save').css('display', 'block');
    $('#Update').css('display', 'none');
});

function HideModal() {
    ClearData();
    $('#BaigrammarModal').modal('hide');
}

function ClearData() {
    $('#Ten_bai').val('');
    $('#Noi_dung').val('');

    $('#NewImageFileGra').val('');

    
    // Ẩn các lỗi và khôi phục lại màu 
    $('#Ten_baiError').text('').hide();
    $('#Noi_dungError').text('').hide();
    $('#ImageFileGraError').text('').hide();

    $('#Ten_bai').css('border-color', 'lightgrey');
    $('#Noi_dung').css('border-color', 'lightgrey');
    $('#NewImageFileGra').css('border-color', 'lightgrey');

}

$('#Ten_bai').on('input', function () {
    if ($(this).val().trim() !== "") {
        $('#Ten_bai').css('border-color', 'lightgrey');
        $('#Ten_baiError').text('').hide();
    }
});

$('#Noi_dung').on('input', function () {
    if ($(this).val().trim() !== "") {
        $('#Noi_dung').css('border-color', 'lightgrey');
        $('#Noi_dungError').text('').hide();
    }
});

$('#NewImageFileGra').on('change', function () {
    if (this.files.length > 0) {
        $('#NewImageFileGra').css('border-color', 'lightgrey');
        $('#ImageFileGraError').text('').hide();
    }
});







