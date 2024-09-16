using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using ToeicWeb.Data;
using ToeicWeb.Models;
using ToeicWeb.Models.ViewModels;

namespace ToeicWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class LuyentapController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LuyentapController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Part5()
        {
            List<Ma_bai_tap_doc> mabaitaps = _db.Mabaitapdocs.ToList();
            ViewData["NavbarType"] = "_NavbarBack";
            return View(mabaitaps);
        }

        [HttpGet] 
        // Hiển thị bài trắc nghiệm
        public async Task<IActionResult> PracticePart5(int baiTapId)
        {
            var baiTap = await _db.Mabaitapdocs.FindAsync(baiTapId);
            if (baiTap == null)
            {
                return NotFound();
            }

            var cauHoiList = await _db.Cauhoibaitapdocs
                                           .Where(c => c.Ma_bai_tap_docId == baiTapId)
                                           .OrderBy(c => c.Thu_tu_cau)
                                           .ToListAsync();

            var viewModel = new TracNghiemViewModel
            {
                BaiTap = baiTap,
                CauHoiList = cauHoiList
            };
            ViewData["NavbarType"] = "_NavbarLuyentap";

            return View(viewModel);
        }

        // Xử lý kết quả trắc nghiệm


        [HttpPost]
        public async Task<IActionResult> ResultPart5(IFormCollection form)
        {
            int correctAnswers = 0;
            int incorrectAnswers = 0;
            int skippedQuestions = 0;

            // Lấy danh sách ID câu hỏi từ form
            var questionIds = form["questionIds"].Select(int.Parse).ToList();

            // Truy vấn danh sách câu hỏi từ cơ sở dữ liệu dựa trên danh sách ID
            var cauHoiList = await _db.Cauhoibaitapdocs
                                      .Where(q => questionIds.Contains(q.Id))
                                      .ToListAsync();

            // Lặp qua danh sách câu hỏi vừa truy vấn
            foreach (var cauHoi in cauHoiList)
            {
                var key = $"cauHoi_{cauHoi.Id}";

                // Kiểm tra nếu form chứa khóa này hay không
                if (form.ContainsKey(key))
                {
                    var selectedAnswer = form[key]; // User's selected answer

                    if (string.IsNullOrEmpty(selectedAnswer))
                    {
                        // Nếu không chọn câu trả lời, tăng số câu bị bỏ qua
                        skippedQuestions++;
                    }
                    else if (selectedAnswer == cauHoi.Dap_an_dung)
                    {
                        // Nếu câu trả lời đúng
                        correctAnswers++;
                    }
                    else
                    {
                        // Nếu câu trả lời sai
                        incorrectAnswers++;
                    }
                }
                else
                {
                    // Nếu không có trong form, nghĩa là câu này bị bỏ qua
                    skippedQuestions++;
                }
            }

            // Set view data to pass the results to the view
            ViewData["NavbarType"] = "_NavbarBack";
            ViewBag.CorrectAnswers = correctAnswers;
            ViewBag.IncorrectAnswers = incorrectAnswers;
            ViewBag.SkippedQuestions = skippedQuestions;

            return View();
        }




    }
}
