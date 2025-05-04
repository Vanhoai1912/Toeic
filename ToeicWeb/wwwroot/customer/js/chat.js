document.addEventListener("DOMContentLoaded", function () {
    const chatIcon = document.getElementById("chat-icon");
    const chatContainer = document.getElementById("chat-container");
    const chatHeader = document.getElementById("chat-header");
    const chatBody = document.getElementById("chat-body");
    const chatInput = document.getElementById("chat-input");
    const sendBtn = document.getElementById("send-btn");

    let chatType = "ai"; // Mặc định chat với AI
    let userId = null;
    let adminMessages = [];
    let aiMessages = [];

    // Escape nội dung HTML
    function escapeHtml(text) {
        const div = document.createElement("div");
        div.textContent = text;
        return div.innerHTML;
    }

    // Hiệu ứng mở chat
    chatIcon.addEventListener("click", function () {
        chatContainer.classList.toggle("show");
    });

    // Cuộn xuống cuối chat
    const scrollToBottom = () => chatBody.scrollTo({ top: chatBody.scrollHeight, behavior: "smooth" });

    // Hiển thị tin nhắn
    const showMessage = (sender, message, isUser = false) => {
        const msgDiv = document.createElement("div");
        msgDiv.classList.add("message", isUser ? "user" : "bot");

        let formattedMessage = message;

        try {
            const data = typeof message === "string" ? JSON.parse(message) : message;

            if (data.ten_thi) {
                formattedMessage = `
                <strong>${data.ten_thi}</strong><br>
                ${data.cach_dung}<br>
                <strong>Công thức:</strong> ${data.cong_thuc}<br>
                <strong>Ví dụ:</strong>
                <ul>
            `;

                if (Array.isArray(data.vi_du)) {
                    data.vi_du.forEach(example => {
                        formattedMessage += `<li>${example}</li>`;
                    });
                }

                formattedMessage += `</ul>`;
            }
        } catch (e) {
            formattedMessage = escapeHtml(formattedMessage); // Escape nếu không phải JSON
        }

        msgDiv.innerHTML = formattedMessage;
        chatBody.appendChild(msgDiv);
        scrollToBottom();
    };

    const showError = (message) => showMessage("Lỗi", message, false);

    const showChatMessages = () => {
        chatBody.innerHTML = ""; // Xóa hiển thị cũ
        const messages = chatType === "admin" ? adminMessages : aiMessages;
        messages.forEach(msg => chatBody.insertAdjacentHTML("beforeend", msg));
        scrollToBottom();
    };


    document.querySelectorAll(".chat-option-btn").forEach(button => {
        button.addEventListener("click", async function () {
            document.querySelectorAll(".chat-option-btn").forEach(btn => btn.classList.remove("active"));
            this.classList.add("active");

            chatType = this.dataset.chat;

            if (chatType === "admin") {
                await getUserId();
                if (!userId) {
                    chatBody.innerHTML = "";
                    showError("Bạn cần đăng nhập để chat với Admin.");
                    return;
                }
                await loadAdminMessages();
            }

            showChatMessages();
        });
    });


    const sendMessage = async () => {
        const input = chatInput.value.trim();
        if (!input) return;

        // Giao diện đẹp hơn: user avatar + bong bóng
        const wrapper = document.createElement("div");
        wrapper.className = "chat-message user mb-2 flex items-start justify-end gap-2";

        const avatar = document.createElement("img");
        avatar.src = "/customer/images/user-avatar.png" // Thay bằng link avatar của bạn
        avatar.alt = "Bạn";
        avatar.className = "avatar w-8 h-8 rounded-full mx-0";

        const bubble = document.createElement("div");
        bubble.className = "chat-bubble user px-4 py-2 rounded-xl";
        bubble.textContent = input;

        wrapper.appendChild(bubble);
        wrapper.appendChild(avatar);
        chatBody.appendChild(wrapper);
        scrollToBottom();
        chatInput.value = "";

        try {
            const response = await fetch(`/customer/api/chat/send/${chatType}`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ messageText: input })
            });

            const body = await response.json();

            if (!response.ok) {
                showError(body?.error || "Đã xảy ra lỗi.");
                return;
            }

            if (body.reply?.trim()) {
                const wrapper = document.createElement("div");
                wrapper.className = "chat-message bot mb-2 flex items-start gap-2";

                const avatar = document.createElement("img");
                avatar.src = "/customer/images/admin-avatar.png";
                avatar.alt = chatType === "admin" ? "Admin" : "AI";
                avatar.className = "avatar w-8 h-8 rounded-full mx-0";

                const bubble = document.createElement("div");
                bubble.className = "chat-bubble bot px-4 py-2 rounded-xl";
                bubble.textContent = body.reply;

                wrapper.appendChild(avatar);
                wrapper.appendChild(bubble);
                chatBody.appendChild(wrapper);
                scrollToBottom();
            } else {
                setTimeout(() => {
                    const bubble = document.createElement("div");
                    bubble.className = "chat-bubble bot mb-2";
                    bubble.textContent = "Hệ thống: Tin nhắn đã gửi thành công.";
                    chatBody.appendChild(bubble);
                    scrollToBottom();
                }, 1000);
            }
        } catch (error) {
            showError("Không thể kết nối đến server.");
        }
    };



    const getUserId = async () => {
        try {
            const response = await fetch("/customer/api/chat");
            const data = await response.json();

            if (response.status === 401) throw new Error("Bạn chưa đăng nhập.");
            if (!data.userId) throw new Error("Không lấy được userId.");

            userId = data.userId;
        } catch (error) {
            userId = null;
        }
    };

    const loadAdminMessages = async () => {
        try {
            const response = await fetch("/customer/api/chat");
            if (response.status === 401) {
                showError("Bạn cần đăng nhập để xem tin nhắn với Admin.");
                return;
            }

            const data = await response.json();
            if (!data.userId) throw new Error("Không lấy được userId.");
            userId = data.userId;

            if (!Array.isArray(data.messages)) return;

            adminMessages = [];
            chatBody.innerHTML = "";

            data.messages.forEach(msg => {
                const isUser = msg.senderId === userId;

                const wrapper = document.createElement("div");
                wrapper.className = `chat-message ${isUser ? "user justify-end" : "bot"} mb-2 flex items-start gap-2`;

                const avatar = document.createElement("img");
                avatar.src = isUser ? "/customer/images/user-avatar.png" : "/customer/images/admin-avatar.png";
                avatar.alt = isUser ? "Bạn" : "Admin";
                avatar.className = "avatar w-8 h-8 rounded-full mx-0";

                const bubble = document.createElement("div");
                bubble.className = `chat-bubble ${isUser ? "user" : "bot"} px-4 py-2 rounded-xl`;
                bubble.textContent = msg.messageText;

                if (isUser) {
                    wrapper.appendChild(bubble);
                    wrapper.appendChild(avatar);
                } else {
                    wrapper.appendChild(avatar);
                    wrapper.appendChild(bubble);
                }

                chatBody.appendChild(wrapper);
                adminMessages.push(wrapper.outerHTML);
            });

            scrollToBottom();
        } catch (error) {
            showError(error.message);
        }
    };




    sendBtn.addEventListener("click", sendMessage);
    chatInput.addEventListener("keypress", (e) => {
        if (e.key === "Enter") sendMessage();
    });

    window.onload = showChatMessages;
});

// Toggle khung chat khi nhấn icon
document.getElementById("chat-icon").addEventListener("click", (e) => {
    e.stopPropagation(); // Ngăn sự kiện click lan ra ngoài
    const chatContainer = document.getElementById("chat-container");
    chatContainer.style.display = (chatContainer.style.display === "flex") ? "none" : "flex";
});

// Đóng chat khi nhấn nút close
document.getElementById("close-chat").addEventListener("click", (e) => {
    e.stopPropagation();
    document.getElementById("chat-container").style.display = "none";
});

// Đóng chat khi click ra ngoài
document.addEventListener("click", (e) => {
    const chatContainer = document.getElementById("chat-container");
    const chatIcon = document.getElementById("chat-icon");

    if (!chatContainer.contains(e.target) && !chatIcon.contains(e.target)) {
        chatContainer.style.display = "none";
    }
});

// Ngăn click bên trong khung chat bị lan ra ngoài
document.getElementById("chat-container").addEventListener("click", (e) => {
    e.stopPropagation();
});


