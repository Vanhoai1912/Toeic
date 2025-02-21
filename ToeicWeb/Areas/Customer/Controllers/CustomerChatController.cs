using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpPost("{chatType}")]
        public async Task<IActionResult> SendMessage(string chatType, [FromBody] Message request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.MessageText))
            {
                return BadRequest("Tin nhắn không hợp lệ.");
            }

            Console.WriteLine($"Nhận request từ {request.Sender} -> {request.Receiver}: {request.MessageText}");

            // Lưu tin nhắn của người dùng vào DB
            var userMessage = new Message
            {
                Sender = request.Sender,
                Receiver = request.Receiver,
                MessageText = request.MessageText,
                IsFromAI = false,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(userMessage);
            await _context.SaveChangesAsync();

            if (chatType == "admin")
            {
                // Kiểm tra xem user đã từng gửi tin nhắn cho admin chưa
                bool hasPreviousMessages = _context.Messages
                    .Any(m => m.Sender == request.Sender && m.Receiver == "Admin");

                // Nếu đã gửi tin nhắn trước đó, không trả về phản hồi nữa
                if (hasPreviousMessages)
                {
                    return Ok(new { reply = "" }); // Trả về chuỗi rỗng, không hiển thị tin nhắn mới
                }

                // Nếu đây là lần đầu, trả về thông báo mặc định
                return Ok(new { reply = "Admin sẽ trả lời tin nhắn của bạn sớm nhất." });
            }

            try
            {
                // Nếu chat với AI, gọi API Gemini
                string botReply = await _geminiService.GetChatbotResponse(request.MessageText);

                var botMessage = new Message
                {
                    Sender = "Chatbot",
                    Receiver = request.Sender,
                    MessageText = botReply,
                    IsFromAI = true,
                    Timestamp = DateTime.UtcNow
                };

                _context.Messages.Add(botMessage);
                await _context.SaveChangesAsync();

                return Ok(new { reply = botReply });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }



    }
}

