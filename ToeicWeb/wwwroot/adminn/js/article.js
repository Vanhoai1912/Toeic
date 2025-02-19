var dataTable;

$(document).ready(function () {
    // Khi modal mở, xóa aria-hidden
    $('#ArticleModal').on('shown.bs.modal', function () {
        $(this).removeAttr('aria-hidden');
    });

    // Khi modal ẩn, đặt lại aria-hidden
    $('#ArticleModal').on('hide.bs.modal', function () {
        $(this).attr('aria-hidden', 'true');
        HideModal();
    });

    loadDataTable();
});
;

// Read data
function loadDataTable() {
    if ($.fn.dataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Article/GetAll"
        },
        "columns": [
            { "data": "id", "width": "10%", "className": "text-start" },
            { "data": "ten_bai", "width": "25%" },
            {
                "data": "createdAt",
                "render": function (data) {
                    let date = new Date(data);
                    return date.toLocaleDateString('vi-VN'); // Hiển thị theo định dạng VN (dd/mm/yyyy)
                },
                "width": "20%"
            },
            {
                "data": "updatedAt",
                "render": function (data) {
                    let date = new Date(data);
                    return date.toLocaleDateString('vi-VN'); // Hiển thị theo định dạng VN (dd/mm/yyyy)
                },
                "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="#" onclick="Edit(${data})"
                            class="btn btn-primary ms-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                            <a onClick=Delete('/Admin/Article/Delete/${data}')
                            class="btn btn-danger ms-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                        </div>
                    `;
                },
                "width": "25%"
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
    var newImageFileArticle = $('#NewImageFileArticle').get(0).files[0];

    if (newImageFileArticle) {
        formData.append('ImageFileArticle', newImageFileArticle);
    }

    // Lấy nội dung từ TinyMCE
    var noi_dung_value = tinymce.get('Noi_dung').getContent();
    formData.append('Noi_dung', noi_dung_value);

    formData.append('Ten_bai', $('#Ten_bai').val());
    formData.append('Description', $('#Description').val());


    $.ajax({
        url: '/Admin/Article/Create',
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

function Edit(id) {
    $.ajax({
        url: '/Admin/Article/Edit?id=' + id,
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
                    noi_dung: response.data.noi_dung,
                    description: response.data.description,

                };

                $('#ArticleModal').modal('show');
                $('#modalTitle').text('Sửa bài báo');
                $('#Save').css('display', 'none');
                $('#Update').css('display', 'block');

                // Điền các giá trị vào modal
                $('#Id').val(response.data.id);
                $('#Ten_bai').val(response.data.ten_bai);
                $('#Description').val(response.data.description);


                // Khởi tạo lại TinyMCE với nội dung
                initTinyMCE(response.data.noi_dung);

                // Hiển thị tên file ảnh
                if (response.data.filePathImageArticle) {
                    var fileNameArticle = response.data.filePathImageArticle.split('\\').pop().split('/').pop();
                    $('#ImageFileArticle').text(fileNameArticle); // Cập nhật tên file vào span
                    $('#ImageFileArticleInfo').show(); // Hiện thị thông tin file
                } else {
                    $('#ImageFileArticleInfo').hide(); // Ẩn thông tin file nếu không có
                }

                // Reset phần input file mới
                $('#NewImageFileArticle').val('');
            }
        },
        error: function () {
            alert('Không thể đọc dữ liệu');
        }
    });
}

function Update() {
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
                handleUpdate();
            }
        });
    } else {
        // Nếu TinyMCE đã được khởi tạo, gọi trực tiếp
        handleUpdate();
    }
}

// Hàm xử lý việc cập nhật dữ liệu
function handleUpdate() {
    var tenbai = $('#Ten_bai').val();
    var description = $('#Description').val();
    var noi_dung_value = tinymce.get('Noi_dung').getContent();
    var newImageFileArticle = $('#NewImageFileArticle').get(0).files[0];

    // Kiểm tra sự thay đổi của tiêu đề, nội dung và file ảnh
    var tieuDeChanged = tenbai !== originalData.ten_bai;
    var noiDungChanged = noi_dung_value !== originalData.noi_dung;
    var descriptionChanged = description !== originalData.description;
    var imageFileChanged = false;

    // Kiểm tra nếu có tệp mới và so sánh với file cũ (nếu có)
    if (newImageFileArticle) {
        var newFileName = newImageFileArticle.name; // Lấy tên file mới
        if (originalData.imageFileArticle) {
            var oldFileName = originalData.imageFileArticle; // Tên file cũ từ dữ liệu ban đầu
            imageFileChanged = newFileName !== oldFileName; // So sánh tên file
        } else {
            imageFileChanged = true; // Nếu không có file cũ, nhưng có file mới
        }
    }

    // Tạo đối tượng FormData để gửi dữ liệu
    var formData = new FormData();
    formData.append('Id', $('#Id').val());
    formData.append('Ten_bai', tenbai);
    formData.append('Description', description);
    formData.append('Noi_dung', noi_dung_value);

    if (newImageFileArticle) {
        formData.append('ImageFileArticle', newImageFileArticle);
    }

    // Gửi yêu cầu AJAX để cập nhật
    $.ajax({
        url: '/Admin/Article/Update',  // URL của action Update trong controller
        data: formData,
        type: 'POST',
        contentType: false,
        processData: false,
        success: function (response) {
            if (!response.success) {
                // Hiển thị thông báo lỗi nếu không có thay đổi hoặc có lỗi
                toastr.info(response.message);  // Hiển thị thông báo từ phản hồi server
            } else {
                // Nếu cập nhật thành công, hiển thị thông báo thành công và xử lý các bước tiếp theo
                HideModal();
                loadDataTable();
                ClearData();
                toastr.success(response.message);  // Hiển thị thông báo thành công
            }
        },
        error: function (xhr) {
            console.error(xhr);
            toastr.error(xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : 'Đã xảy ra lỗi.');
        }
    });
}


function initTinyMCE(content) {
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

            // Đặt nội dung cũ vào trình soạn thảo sau khi khởi tạo TinyMCE
            editor.on('init', function () {
                editor.setContent(content || "");  // Đặt nội dung từ biến content vào editor
            });
        },
        plugins: 'anchor autolink charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount',
        toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image media table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
    });
}



function Validate() {
    var isValid = true;

    // Kiểm tra tên bài
    if ($('#Ten_bai').val().trim() === "") {
        $('#Ten_bai').css('border-color', 'Red');
        $('#Ten_baiError').text('Vui lòng nhập tên bài.').show();
        isValid = false;
    } else {
        $('#Ten_bai').css('border-color', 'lightgrey');
        $('#Ten_baiError').text('').hide();
    }

      // Kiểm tra tên bài
    if ($('#Description').val().trim() === "") {
        $('#Description').css('border-color', 'Red');
        $('#DescriptionError').text('Vui lòng nhập tên bài.').show();
        isValid = false;
    } else {
        $('#Description').css('border-color', 'lightgrey');
        $('#DescriptionError').text('').hide();
    }

    // Kiểm tra nội dung từ TinyMCE
    var editor = tinymce.get('Noi_dung');
    if (editor) {
        var noi_dung_value = editor.getContent();
        if (noi_dung_value.trim() === "") {
            // Sử dụng jQuery để tìm phần tử chứa
            $(editor.getContainer()).css('border', '1px solid red');
            $('#Noi_dungError').text('Vui lòng nhập nội dung.').show();
            isValid = false;
        } else {
            $(editor.getContainer()).css('border', '1px solid lightgrey');
            $('#Noi_dungError').text('').hide();
        }
    } else {
        console.error('Editor TinyMCE chưa được khởi tạo hoặc không tìm thấy.');
    }

    // Kiểm tra tệp hình ảnh
    var newImageFileArticleInput = $('#NewImageFileArticle').get(0);
    if (newImageFileArticleInput && newImageFileArticleInput.files.length === 0) {
        $('#NewImageFileArticle').css('border-color', 'Red');
        $('#ImageFileArticleError').text('Vui lòng chọn file ảnh.').show();
        isValid = false;
    } else {
        $('#NewImageFileArticle').css('border-color', 'lightgrey');
        $('#ImageFileArticleError').text('').hide();
    }

    return isValid;
}


$('#btnAdd').click(function () {
    ClearData(); // Xóa dữ liệu trước khi mở modal
    $('#ArticleModal').modal('show');
    $('#modalTitle').text('Thêm bài báo mới');
    $('#Save').css('display', 'block');
    $('#Update').css('display', 'none');
});

var isHidingModal = false; // Khai báo biến chỉ một lần

function HideModal() {
    if (isHidingModal) return; // Ngăn chặn vòng lặp
    isHidingModal = true;

    ClearData();
    $('#ArticleModal').modal('hide');

    $('#ImageFileArticleInfo').hide();

    isHidingModal = false; // Đặt lại biến cờ
}
function ClearData() {
    $('#Ten_bai').val('');
    $('#Description').val('');
    tinymce.get('Noi_dung').setContent(''); // Xóa nội dung TinyMCE
    $('#NewImageFileArticle').val('');

    // Ẩn các lỗi và khôi phục lại màu border
    $('#Ten_baiError').text('').hide();
    $('#Noi_dungError').text('').hide();
    $('#DescriptionError').text('').hide();
    $('#ImageFileArticleError').text('').hide();

    $('#Ten_bai').css('border-color', 'lightgrey');
    $('#Description').css('border-color', 'lightgrey');
    $('#NewImageFileArticle').css('border-color', 'lightgrey');

    // Khôi phục viền cho nội dung
    tinymce.get('Noi_dung').getContainer().style.border = "1px solid lightgrey"; // Khôi phục màu border cho TinyMCE
}



$('#Description').on('input', function () {
    if ($(this).val().trim() !== "") {
        $('#Description').css('border-color', 'lightgrey');
        $('#DescriptionError').text('').hide();
    }
});

$('#Description').on('input', function () {
    if ($(this).val().trim() !== "") {
        $('#Description').css('border-color', 'lightgrey');
        $('#DescriptionError').text('').hide();
    }
});


$('#NewImageFileArticle').on('change', function () {
    if (this.files.length > 0) {
        $('#NewImageFileArticle').css('border-color', 'lightgrey');
        $('#ImageFileArticleError').text('').hide();
    }
});








