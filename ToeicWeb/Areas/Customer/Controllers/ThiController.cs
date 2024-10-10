using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Toeic.DataAccess;
using Toeic.Models.ViewModels;
using Toeic.Models;
using Microsoft.EntityFrameworkCore;

namespace ToeicWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ThiController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ThiController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }


        // PART1
        public IActionResult ThiTH()
        {
            List<Ma_bai_thi> mabaitaps = _db.Mabaithis.ToList();
            ViewData["NavbarType"] = "_NavbarBack";
            return View(mabaitaps);
        }

        [HttpGet]
        public async Task<IActionResult> PracticeTH(int baiTapId)
        {
            var baiTap = await _db.Mabaithis.FindAsync(baiTapId);
            if (baiTap == null)
            {
                return NotFound();
            }

            var cauHoiList = await _db.Cauhoibaithis
                                           .Where(c => c.Ma_bai_thiId == baiTapId)
                                           .OrderBy(c => c.Thu_tu_cau)
                                           .ToListAsync();

            var viewModel = new TracNghiemViewModel
            {
                Baithi = baiTap,
                CauHoiBaiThiList = cauHoiList
            };
            ViewData["NavbarType"] = "_NavbarBack";

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResultDetailTH(IFormCollection form, int baiTapId)
        {
            int correctAnswers = 0;
            int incorrectAnswers = 0;
            int skippedQuestions = 0;

            //Lấy danh sách ID câu hỏi từ form
            var questionid = form["questionIds"].Select(int.Parse).ToList();

            // Truy vấn danh sách câu hỏi từ cơ sở dữ liệu dựa trên danh sách ID
            var cauhoiList = await _db.Cauhoibaithis
                                      .Where(q => questionid.Contains(q.Id))
                                      .ToListAsync();

            //Lặp qua danh sách câu hỏi vừa truy vấn
            foreach (var cauHoi in cauhoiList)
            {
                var key = $"cauHoi_{cauHoi.Id}";

                //Kiểm tra nếu form chứa khóa này hay không
                if (form.ContainsKey(key))
                {
                    var selectedAnswer = form[key]; // User's selected answer

                    if (string.IsNullOrEmpty(selectedAnswer))
                    {
                        //Nếu không chọn câu trả lời, tăng số câu bị bỏ qua
                        skippedQuestions++;
                    }
                    else if (selectedAnswer == cauHoi.Dap_an_dung)
                    {
                        //Nếu câu trả lời đúng
                        correctAnswers++;
                    }
                    else
                    {
                        //Nếu câu trả lời sai
                        incorrectAnswers++;
                    }
                }
                else
                {
                    //Nếu không có trong form, nghĩa là câu này bị bỏ qua
                    skippedQuestions++;
                }
            }

            // Set view data to pass the results to the view
            ViewBag.CorrectAnswers = correctAnswers;
            ViewBag.IncorrectAnswers = incorrectAnswers;
            ViewBag.SkippedQuestions = skippedQuestions;
            //Retrieve BaiTap from the database

            var baiTap = await _db.Mabaithis.FindAsync(baiTapId);
            if (baiTap == null)
            {
                return NotFound();
            }

            //Retrieve the list of questions for the given baiTapId

            var cauHoiList = await _db.Cauhoibaithis
                                      .Where(c => c.Ma_bai_thiId == baiTapId)
                                      .OrderBy(c => c.Thu_tu_cau)
                                      .ToListAsync();

            //Extract user answers from the form
            var questionIds = form["questionIds"].ToString().Split(',');
            var userAnswers = new Dictionary<int, string>();

            foreach (var questionIdStr in questionIds)
            {
                if (int.TryParse(questionIdStr, out int questionId))
                {
                    var answer = form[$"cauHoi_{questionId}"];
                    if (!string.IsNullOrEmpty(answer))
                    {
                        userAnswers.Add(questionId, answer);
                    }
                }
            }

            //Process the user answers and calculate results
            foreach (var question in cauHoiList)
            {
                if (userAnswers.TryGetValue(question.Id, out var userAnswer))
                {
                    question.UserAnswer = userAnswer; // Store the user's answer
                    question.IsCorrect = string.Equals(userAnswer, question.Dap_an_dung, StringComparison.OrdinalIgnoreCase); // Check if the answer is correct
                }
            }
            var totalTime = form["TotalTime"].ToString();

            //Create the view model
            var viewModel = new TracNghiemViewModel
            {
                Baithi = baiTap,
                CauHoiBaiThiList = cauHoiList,
                TotalTime = totalTime // Nhận thời gian làm bài từ form
            };


            //Return the view with the results
            ViewData["NavbarType"] = "_NavbarThoat";

            return View(viewModel);
        }
    }
}
