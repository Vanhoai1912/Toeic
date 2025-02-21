using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Toeic.DataAccess;
using Toeic.Models;
using ToeicWeb.Services;

namespace ToeicWeb.Areas.Customer.Controllers
{
    [Route("customer/api/chat")]
    [ApiController]
    [Area("Customer")]
    public class CustomerChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly GeminiService _geminiService;

        public CustomerChatController(ApplicationDbContext context, GeminiService geminiService)
        {
            _context = context;
            _geminiService = geminiService;
        }

        public class SendMessageDto
        {
            public string MessageText { get; set; }
        }
        [HttpPost("send/{chatType}")]
        public async Task<IActionResult> SendMessage(string chatType, [FromBody] SendMessageDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.MessageText))
            {
                return BadRequest(new { error = "Tin nhắn không hợp lệ." });
            }

            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(senderId))
            {
                return Unauthorized(new { error = "Bạn cần đăng nhập để gửi tin nhắn." });
            }

            try
            {
                string? receiverId = null;
                string? aiReply = null;

                if (chatType == "admin")
                {
                    var adminUser = await _context.Users
                        .Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new { user, userRole })
                        .Join(_context.Roles, ur => ur.userRole.RoleId, role => role.Id, (ur, role) => new { ur.user, role })
                        .Where(x => x.role.Name == "Admin")
                        .Select(x => x.user)
                        .FirstOrDefaultAsync();

                    if (adminUser == null)
                    {
                        return NotFound(new { error = "Không tìm thấy Admin." });
                    }

                    receiverId = adminUser.Id;
                }
                else if (chatType == "ai")
                {
                    aiReply = await _geminiService.GetChatbotResponse(request.MessageText);

                    if (string.IsNullOrWhiteSpace(aiReply))
                    {
                        return StatusCode(500, new { error = "AI không trả lời." });
                    }

                    // Đảm bảo SenderId không bị null khi lưu tin nhắn AI
                    receiverId = senderId;
                }
                else
                {
                    return BadRequest(new { error = "Loại chat không hợp lệ. Chọn 'admin' hoặc 'ai'." });
                }

                var userMessage = new Message
                {
                    SenderId = senderId,
                    ReceiverId = receiverId, // Đảm bảo luôn có giá trị khi lưu
                    MessageText = request.MessageText,
                    IsFromAI = false,
                    Timestamp = DateTime.UtcNow
                };

                await _context.Messages.AddAsync(userMessage);

                if (chatType == "ai")
                {
                    var aiMessage = new Message
                    {
                        SenderId = senderId, // Gán senderId để tránh lỗi NULL
                        ReceiverId = senderId, // AI trả lời trực tiếp cho người dùng
                        MessageText = aiReply,
                        IsFromAI = true,
                        Timestamp = DateTime.UtcNow
                    };

                    await _context.Messages.AddAsync(aiMessage);
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    var detailedError = dbEx.InnerException?.InnerException?.Message
                                        ?? dbEx.InnerException?.Message
                                        ?? dbEx.Message;

                    return StatusCode(500, new
                    {
                        error = "Lỗi khi lưu thay đổi vào cơ sở dữ liệu.",
                        details = detailedError
                    });
                }

                if (chatType == "ai")
                {
                    return Ok(new { reply = aiReply });
                }

                return Ok(new { success = true });
                // Không trả về tin nhắn mặc định khi gửi đến admin

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Đã xảy ra lỗi nội bộ.", details = ex.Message });
            }
        }

    }
}
