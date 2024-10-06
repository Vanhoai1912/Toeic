using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toeic.DataAccess;
using Toeic.Models;
using Toeic.Models.ViewModels;

namespace ToeicWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class GrammarController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public GrammarController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult Index()
        {
            List<Ma_bai_ngu_phap> mabainguphap = _db.Mabainguphaps.ToList();
            return View(mabainguphap);
        }
        [HttpGet]
        public async Task<IActionResult> GrammarDetail(int mabainguphapId)
        {
            // Tìm bài ngữ pháp theo ID
            var mabai = await _db.Mabainguphaps.FindAsync(mabainguphapId);
            if (mabai == null)
            {
                return NotFound();
            }

            // Tạo ViewModel với dữ liệu từ Mabainguphap
            var viewModel = new BainguphapVM
            {
                Id = mabai.Id,
                Ten_bai = mabai.Ten_bai,
                ImageFileGrammar = null, // Không cần thiết, nhưng có thể để lại nếu cần
                Noi_dung = mabai.Noi_dung, // Lấy nội dung từ Mabainguphap
                Mabainguphap = mabai // Thêm vào nếu cần thiết
            };

            // Đặt ViewData cho loại Navbar (nếu cần)
            ViewData["NavbarType"] = "_NavbarBack";

            // Trả về view với ViewModel
            return View(viewModel);
        }



    }
}
