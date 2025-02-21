$(document).ready(function () {
    loadUserList();
    $("#chat-container").hide(); // Ẩn khung chat ban đầu
});

// 🟢 Load danh sách người dùng
function loadUserList() {
    $.ajax({
        url: "/Admin/AdminChat/GetUsers",
        type: "GET",
        success: function (data) {
            let userListHtml = "";
            data.forEach(user => {
                userListHtml += `
                    <div class="user-item" onclick="loadChat('${user.id}', '${user.name}')">
                       <img src="/adminn/img/user-profile-icon-free-vector.jpg" class="user-avatar" alt="Avatar">
                        <span class="user-name">${user.name}</span>
                    </div>
                `;
            });
            $("#user-list").html(userListHtml);
        },
        error: function () {
            alert("Lỗi khi tải danh sách người dùng.");
        }
    });
}

// 🟢 Load tin nhắn giữa Admin và User
function loadChat(userId, userName) {
    $("#chat-container").show();  // Hiển thị khung chat
    $("#chat-container").attr("data-user-id", userId);  // Lưu userId để gửi tin nhắn sau này
    $("#chat-title").text(`Đang chat với ${userName}`);  // Cập nhật tiêu đề chat

    $.ajax({
        url: `/Admin/AdminChat/messages/${userId}`,
        type: "GET",
        success: function (data) {
            let chatHtml = "";
            data.forEach(msg => {
                let sender = msg.senderId === userId ? "Người dùng" : "Admin";
                chatHtml += `<p><strong>${sender}:</strong> ${msg.messageText}</p>`;
            });
            $("#chat-box").html(chatHtml);
        },
        error: function () {
            alert("Lỗi khi tải tin nhắn.");
        }
    });
}

// 🟢 Gửi tin nhắn từ Admin đến User
function sendMessage() {
    let messageText = $("#message-input").val();
    let receiverId = $("#chat-container").attr("data-user-id");  // Lấy userId từ container

    if (!messageText.trim()) return;

    $.ajax({
        url: "/admin/adminchat/send",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ receiverId, messageText }),
        success: function () {
            $("#message-input").val(""); // Xóa nội dung input sau khi gửi
            loadChat(receiverId, $("#chat-title").text().replace("Đang chat với ", "")); // Reload lại chat
        },
        error: function () {
            alert("Gửi tin nhắn thất bại.");
        }
    });
}
