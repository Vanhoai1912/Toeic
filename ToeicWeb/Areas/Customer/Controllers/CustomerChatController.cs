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
            if (chatType == "admin" && string.IsNullOrEmpty(senderId))
            {
                return Unauthorized(new { error = "Bạn cần đăng nhập để gửi tin nhắn cho Admin." });
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

                    // ✅ Chỉ lưu tin nhắn gửi Admin
                    var userMessage = new Message
                    {
                        SenderId = senderId,
                        ReceiverId = receiverId,
                        MessageText = request.MessageText,
                        IsFromAI = false,
                        Timestamp = DateTime.UtcNow
                    };

                    await _context.Messages.AddAsync(userMessage);
                    await _context.SaveChangesAsync();

                    return Ok(new { success = true });
                }
                else if (chatType == "ai")
                {
                    aiReply = await _geminiService.GetChatbotResponse(request.MessageText);

                    if (string.IsNullOrWhiteSpace(aiReply))
                    {
                        return StatusCode(500, new { error = "AI không trả lời." });
                    }

                    // ❌ Không lưu tin nhắn vào database khi chat với AI
                    return Ok(new { reply = aiReply });
                }
                else
                {
                    return BadRequest(new { error = "Loại chat không hợp lệ. Chọn 'admin' hoặc 'ai'." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Đã xảy ra lỗi nội bộ.", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserMessages()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "Bạn cần đăng nhập." });
                }

                var messages = await _context.Messages
                    .Where(m => (m.SenderId == userId || m.ReceiverId == userId) && !m.IsFromAI)
                    .OrderBy(m => m.Timestamp)
                    .Select(m => new
                    {
                        id = m.Id,
                        senderId = m.SenderId,
                        receiverId = m.ReceiverId,
                        messageText = m.MessageText,
                        timestamp = m.Timestamp
                    })
                    .ToListAsync();

                return Ok(new { userId, messages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi máy chủ nội bộ.", details = ex.Message });
            }
        }

        [HttpGet("current-user")] // Tạo API lấy userId
        public IActionResult GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Bạn chưa đăng nhập." });
            }
            return Ok(new { userId });
        }

    }
}
