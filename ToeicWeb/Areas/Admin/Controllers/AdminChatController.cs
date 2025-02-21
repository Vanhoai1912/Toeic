using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Toeic.DataAccess;
using Toeic.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toeic.Utility;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminChatController : Controller

    {
        private readonly ApplicationDbContext _context;

        public AdminChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // ✅ Trả về giao diện
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId không hợp lệ.");

            var messages = await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderBy(m => m.Timestamp)
                .Select(m => new
                {
                    m.Id,
                    m.MessageText,
                    m.Timestamp,
                    SenderId = m.SenderId,
                    SenderName = m.Sender != null ? m.Sender.Name : "Ẩn danh"
                })
                .ToListAsync();

            return Json(messages);
        }




        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            if (string.IsNullOrWhiteSpace(message.MessageText))
                return BadRequest("Tin nhắn không hợp lệ.");

            message.SenderId = User.Identity.Name; // ✅ Lưu tin nhắn của Admin
            message.Timestamp = DateTime.UtcNow;

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _context.ApplicationUsers
                    .Select(u => new {
                        id = u.Id,
                        name = u.Name ?? "Người dùng ẩn danh",
                        avatar = "/images/default-avatar.png"
                    })
                    .ToList();

                return Json(users);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi lấy danh sách user: {ex.Message}");
            }
        }
        #endregion



    }
}
