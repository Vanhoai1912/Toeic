using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Toeic.DataAccess;
using Toeic.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toeic.Utility;
using Microsoft.AspNetCore.Identity;

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





        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            try
            {
                var sender = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                if (sender == null)
                {
                    return BadRequest("Người gửi không tồn tại trong hệ thống.");
                }

                if (string.IsNullOrWhiteSpace(message.MessageText) || string.IsNullOrWhiteSpace(message.ReceiverId))
                {
                    return BadRequest("Tin nhắn hoặc ID người nhận không hợp lệ.");
                }

                // Gán SenderId là ID thật trong bảng AspNetUsers
                message.SenderId = sender.Id;
                message.Timestamp = DateTime.UtcNow;

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }



        #region API CALLS
        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                // Lấy danh sách ID của Admin từ bảng UserRoles
                var adminRoleId = _context.Roles
                    .Where(r => r.Name == "Admin") // Tìm Role có tên "Admin"
                    .Select(r => r.Id)
                    .FirstOrDefault();

                var adminUserIds = _context.UserRoles
                    .Where(ur => ur.RoleId == adminRoleId)
                    .Select(ur => ur.UserId)
                    .ToList();

                // Lọc người dùng có tin nhắn nhưng không phải Admin
                var users = _context.ApplicationUsers
                    .Where(u => _context.Messages.Any(m => m.SenderId == u.Id) && !adminUserIds.Contains(u.Id))
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
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }


        #endregion



    }
}
