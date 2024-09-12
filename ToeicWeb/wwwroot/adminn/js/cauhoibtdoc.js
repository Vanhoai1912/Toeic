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
            "url": "/Admin/CauhoiBTdoc/GetAll"
        },
        "columns": [
            { "data": "id", "width": "20%", "className": "text-start" },
            { "data": "ma_bai_tap_doc.tieu_de", "width": "40%" },
            { "data": "ma_bai_tap_doc.part", "width": "10%" },

            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="#" onclick="Edit(${data})"
                        class="btn btn-primary ms-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                        <a onClick=Delete('/Admin/CauhoiBTdoc/Delete/${data}')
                        class="btn btn-danger ms-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>
                    `;
                },
                "width": "30%"
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


//$('#btnAdd').click(function () {
//    $('#CauhoiBTdocModal').modal('show');
//    $('#modalTitle').text('Thêm bài tập đọc mới');
//    $('#Save').css('display', 'block');
//    $('#Update').css('display', 'none');
//});
//function HideModal() {
//    ClearData();
//    $('#uploadModal').modal('hide');
//}
//function ClearData() {
//    $('#Tieu_de').val('');
//    $('#Tieu_de').css('border-color', 'lightgrey');
//}