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
    // Kiểm tra và khởi tạo TinyMCE nếu chưa khởi tạo
    if (!tinymce.get('Noi_dung')) {
        tinymce.init({
            selector: '#Noi_dung',
            setup: function (editor) {
                editor.on('change', function () {
                    editor.save();  // Cập nhật nội dung textarea
                });
            },
            init_instance_callback: function (editor) {
                // Khi TinyMCE đã sẵn sàng, gọi hàm xử lý
                handleCreate();
            }
        });
    } else {
        // Nếu TinyMCE đã được khởi tạo, gọi trực tiếp
        handleCreate();
    }
}

function handleCreate() {
    // Xác minh dữ liệu từ form
    var result = Validate();
    if (!result) {
        return false;
    }

    var formData = new FormData();
    var newImageFileGra = $('#NewImageFileGra').get(0).files[0];

    if (newImageFileGra) {
        formData.append('ImageFileGrammar', newImageFileGra);
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


function initTinyMCE() {
    if (tinymce.get('Noi_dung')) {
        tinymce.get('Noi_dung').remove();  // Hủy bỏ nếu đã tồn tại
    }

    tinymce.init({
        selector: '#Noi_dung',
        setup: function (editor) {
            editor.on('input', function () {
                if (editor.getContent().trim() !== "") {
                    editor.getContainer().style.border = "1px solid lightgrey";
                    $('#Noi_dungError').text('').hide();
                } else {
                    editor.getContainer().style.border = "1px solid red";
                }
            });
        },
        plugins: 'anchor autolink charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount',
        toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image media table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
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


    // Kiểm tra nội dung từ TinyMCE sau khi khởi tạo
    var editor = tinymce.get('Noi_dung');
    if (editor) {
        var noi_dung_value = editor.getContent();

        if (noi_dung_value.trim() == "") {
            editor.getContainer().style.border = "1px solid red";
            $('#Noi_dungError').text('Vui lòng nhập nội dung.').show();
            isValid = false;
        } else {
            editor.getContainer().style.border = "1px solid lightgrey";
            $('#Noi_dungError').text('').hide();
        }
    } else {
        console.error('Editor TinyMCE chưa được khởi tạo hoặc không tìm thấy.');
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
    tinymce.get('Noi_dung').setContent(''); // Xóa nội dung TinyMCE
    $('#NewImageFileGra').val('');

    // Ẩn các lỗi và khôi phục lại màu border
    $('#Ten_baiError').text('').hide();
    $('#Noi_dungError').text('').hide();
    $('#ImageFileGraError').text('').hide();

    $('#Ten_bai').css('border-color', 'lightgrey');
    $('#NewImageFileGra').css('border-color', 'lightgrey');

    // Khôi phục viền cho nội dung
    tinymce.get('Noi_dung').getContainer().style.border = "1px solid lightgrey"; // Khôi phục màu border cho TinyMCE
}



$('#Ten_bai').on('input', function () {
    if ($(this).val().trim() !== "") {
        $('#Ten_bai').css('border-color', 'lightgrey');
        $('#Ten_baiError').text('').hide();
    }
});


$('#NewImageFileGra').on('change', function () {
    if (this.files.length > 0) {
        $('#NewImageFileGra').css('border-color', 'lightgrey');
        $('#ImageFileGraError').text('').hide();
    }
});








