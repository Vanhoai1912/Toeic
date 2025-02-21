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

function loadChat(userId, userName) {
    console.log("📌 Đang tải tin nhắn của User ID:", userId);

    $("#chat-container").show();
    $("#chat-container").attr("data-user-id", userId);
    $("#chat-title").text(`Đang chat với ${userName}`);

    let url = `/Admin/AdminChat/GetMessages?userId=${userId}`;  // 🛠 Sửa lại URL

    console.log("📌 Gọi API:", url);

    $.ajax({
        url: url,
        type: "GET",
        success: function (data) {
            console.log("📌 Tin nhắn nhận được:", data);
            let chatHtml = "";
            data.forEach(msg => {
                let sender = msg.senderId === userId ? "Người dùng" : "Admin";
                chatHtml += `<p><strong>${sender}:</strong> ${msg.messageText}</p>`;
            });
            $("#chat-box").html(chatHtml);
        },
        error: function (xhr) {
            console.error("❌ Lỗi khi tải tin nhắn:", xhr.responseText);
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
