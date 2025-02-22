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

    // Hiệu ứng mở chat
    chatIcon.addEventListener("click", function () {
        chatContainer.classList.toggle("show");
    });

    // Ẩn chatbox khi bấm ngoài
    document.addEventListener("click", function (e) {
        if (!chatContainer.contains(e.target) && !chatIcon.contains(e.target)) {
            chatContainer.classList.remove("show");
        }
    });

    // Cuộn xuống cuối chat
    const scrollToBottom = () => chatBody.scrollTo({ top: chatBody.scrollHeight, behavior: "smooth" });

    // Hiển thị tin nhắn trên giao diện
    const showMessage = (sender, message, isUser = false) => {
        const msgDiv = document.createElement("div");
        msgDiv.classList.add("message", isUser ? "user" : "bot");
        msgDiv.innerHTML = `<strong>${sender}:</strong> ${message}`;

        if (chatType === "admin") {
            adminMessages.push(msgDiv.outerHTML);
        } else {
            aiMessages.push(msgDiv.outerHTML);
        }

        chatBody.appendChild(msgDiv);
        scrollToBottom();
    };

    // Hiển thị lỗi
    const showError = (message) => showMessage("Lỗi", message, false);

    // Hiển thị lại tin nhắn khi chuyển đổi
    const showChatMessages = () => {
        chatBody.innerHTML = `<p class="chat-notice">Bạn đang chat với ${chatType === "admin" ? "Admin" : "AI"}.</p>`;

        const messages = chatType === "admin" ? adminMessages : aiMessages;
        messages.forEach(msg => chatBody.insertAdjacentHTML("beforeend", msg));

        scrollToBottom();
    };

    // Chuyển giữa AI/Admin
    document.querySelectorAll(".chat-option").forEach(button => {
        button.addEventListener("click", async function () {
            chatType = this.dataset.chat;
            chatHeader.textContent = chatType === "admin" ? "Chat với Admin" : "Chat với AI";
            chatHeader.style.backgroundColor = chatType === "admin" ? "#dc3545" : "#007bff";

            if (chatType === "admin") {
                await getUserId();
                if (!userId) {
                    showChatMessages();
                    showError("Bạn cần đăng nhập để chat với Admin.");
                    return;
                }
                await loadAdminMessages();
            }

            showChatMessages();
        });
    });

    // Gửi tin nhắn
    const sendMessage = async () => {
        const input = chatInput.value.trim();
        if (!input) return;

        showMessage("Bạn", input, true);
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
                showMessage(chatType === "admin" ? "Admin" : "AI", body.reply, false);
            } else {
                setTimeout(() => showMessage("Hệ thống", "Tin nhắn đã gửi thành công.", false), 1000);
            }
        } catch (error) {
            showError("Không thể kết nối đến server.");
        }
    };

    // Lấy userId
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

    // Tải tin nhắn với Admin
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

            adminMessages = data.messages.map(msg => {
                const sender = msg.senderId === userId ? "Bạn" : "Admin";
                return `<div class="message ${msg.senderId === userId ? "user" : "bot"}"><strong>${sender}:</strong> ${msg.messageText}</div>`;
            });

            if (chatType === "admin") {
                showChatMessages();
            }
        } catch (error) {
            showError(error.message);
        }
    };

    // Bấm gửi hoặc nhấn Enter
    sendBtn.addEventListener("click", sendMessage);
    chatInput.addEventListener("keypress", (e) => {
        if (e.key === "Enter") sendMessage();
    });

    // Load tin nhắn AI mặc định
    window.onload = showChatMessages;
});
