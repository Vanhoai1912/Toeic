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
                    <div class="user-item" data-user-id="${user.id}" data-user-name="${user.name}">
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

// 🟢 Sự kiện click trên user-item
$(document).on("click", ".user-item", function () {
    let userId = $(this).data("user-id");
    let userName = $(this).data("user-name");
    loadChat(userId, userName);
});

function loadChat(userId, userName) {
    $("#chat-container").css("display", "block").removeClass("d-none");
    $("#chat-container").attr("data-user-id", userId);
    $("#chat-title").text(`Đang chat với ${userName}`);

    let url = `/Admin/AdminChat/GetMessages?userId=${userId}`;

    $.ajax({
        url: url,
        type: "GET",
        success: function (data) {

            if (!data || data.length === 0) {
                $("#chat-box").html("<p class='text-muted text-center'>Chưa có tin nhắn nào.</p>");
                return;
            }

            let chatHtml = "";
            data.forEach(msg => {
                let isAdmin = msg.senderId !== userId; // Nếu senderId khác userId thì là Admin
                let messageClass = isAdmin ? "admin-message text-end" : "user-message text-start";
                let messageBubble = isAdmin ? "bg-primary text-white" : "bg-light text-dark";

                chatHtml += `
                    <div class="message ${messageClass}">
                        <div class="message-text ${messageBubble} p-2 rounded">
                            <strong>${isAdmin ? "Admin" : userName}:</strong> ${msg.messageText}
                        </div>
                    </div>
                `;
            });

            $("#chat-box").html(chatHtml);
            $("#chat-box").scrollTop($("#chat-box")[0].scrollHeight); // Cuộn xuống tin nhắn mới nhất
        },
        error: function (xhr) {
            alert("Lỗi khi tải tin nhắn.");
        }
    });
}



// 🟢 Gửi tin nhắn từ Admin đến User
$("#message-input").keypress(function (e) {
    if (e.which === 13) sendMessage();
});

$("#send-btn").click(function () {
    sendMessage();
});

function sendMessage() {
    let messageText = $("#message-input").val().trim();
    let receiverId = $("#chat-container").attr("data-user-id");

    if (!messageText) return;

    $.ajax({
        url: "/Admin/AdminChat/SendMessage",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ receiverId, messageText }),
        success: function () {
            $("#message-input").val("");
            loadChat(receiverId, $("#chat-title").text().replace("Đang chat với ", ""));
        },
        error: function () {
            alert("Gửi tin nhắn thất bại.");
        }
    });
}
