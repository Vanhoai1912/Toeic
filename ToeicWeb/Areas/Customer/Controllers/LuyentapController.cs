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
            ViewData["NavbarType"] = "_NavbarBack";

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResultDetailPart5(IFormCollection form, int baiTapId)
        {
            int correctAnswers = 0;
            int incorrectAnswers = 0;
            int skippedQuestions = 0;

            // Lấy danh sách ID câu hỏi từ form
            var questionid = form["questionIds"].Select(int.Parse).ToList();

            // Truy vấn danh sách câu hỏi từ cơ sở dữ liệu dựa trên danh sách ID
            var cauhoiList = await _db.Cauhoibaitapdocs
                                      .Where(q => questionid.Contains(q.Id))
                                      .ToListAsync();

            // Lặp qua danh sách câu hỏi vừa truy vấn
            foreach (var cauHoi in cauhoiList)
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
            ViewBag.CorrectAnswers = correctAnswers;
            ViewBag.IncorrectAnswers = incorrectAnswers;
            ViewBag.SkippedQuestions = skippedQuestions;
            // Retrieve BaiTap from the database
            var baiTap = await _db.Mabaitapdocs.FindAsync(baiTapId);
            if (baiTap == null)
            {
                return NotFound();
            }

            // Retrieve the list of questions for the given baiTapId
            var cauHoiList = await _db.Cauhoibaitapdocs
                                      .Where(c => c.Ma_bai_tap_docId == baiTapId)
                                      .OrderBy(c => c.Thu_tu_cau)
                                      .ToListAsync();

            // Extract user answers from the form
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

            // Process the user answers and calculate results
            foreach (var question in cauHoiList)
            {
                if (userAnswers.TryGetValue(question.Id, out var userAnswer))
                {
                    question.UserAnswer = userAnswer; // Store the user's answer
                    question.IsCorrect = string.Equals(userAnswer, question.Dap_an_dung, StringComparison.OrdinalIgnoreCase); // Check if the answer is correct
                }
            }

            // Create the view model
            var viewModel = new TracNghiemViewModel
            {
                BaiTap = baiTap,  // Store exercise details
                CauHoiList = cauHoiList,  // Store the list of questions and their results
                TotalTime = form["TotalTime"] // If you are tracking time
            };

            // Return the view with the results
            ViewData["NavbarType"] = "_NavbarThoat";

            return View(viewModel);
        }

        public IActionResult Part6()
        {
            List<Ma_bai_tap_doc> mabaitaps = _db.Mabaitapdocs.ToList();
            ViewData["NavbarType"] = "_NavbarBack";
            return View(mabaitaps);
        }

        [HttpGet]
        public async Task<IActionResult> PracticePart6(int baiTapId)
        {
            var baiTap = await _db.Mabaitapdocs
                                  .Include(bt => bt.CauHoiBaiTapDocs) // Lấy tất cả câu hỏi liên quan
                                  .FirstOrDefaultAsync(bt => bt.Id == baiTapId);

            if (baiTap == null)
            {
                return NotFound();
            }

            var cauHoiList = baiTap.CauHoiBaiTapDocs
                                   .OrderBy(c => c.Thu_tu_cau)
                                   .ToList();
            // Lấy các bài đọc liên quan từ danh sách câu hỏi
            var baiDocs = cauHoiList
                .Where(c => !string.IsNullOrEmpty(c.Bai_doc))
                .Select(c => c.Bai_doc)
                .Distinct()
                .ToList();

            var viewModel = new TracNghiemViewModel
            {
                BaiTap = baiTap,
                CauHoiList = cauHoiList,
                BaiDocs = baiDocs.ToDictionary(doc => doc, doc => doc)
            };

            ViewData["NavbarType"] = "_NavbarBack";

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> ResultDetailPart6(IFormCollection form, int baiTapId)
        {
            int correctAnswers = 0;
            int incorrectAnswers = 0;
            int skippedQuestions = 0;

            // Lấy danh sách ID câu hỏi từ form
            var questionid = form["questionIds"].Select(int.Parse).ToList();

            // Truy vấn danh sách câu hỏi từ cơ sở dữ liệu dựa trên danh sách ID
            var cauhoiList = await _db.Cauhoibaitapdocs
                                      .Where(q => questionid.Contains(q.Id))
                                      .ToListAsync();

            // Lặp qua danh sách câu hỏi vừa truy vấn
            foreach (var cauHoi in cauhoiList)
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
            ViewBag.CorrectAnswers = correctAnswers;
            ViewBag.IncorrectAnswers = incorrectAnswers;
            ViewBag.SkippedQuestions = skippedQuestions;
            // Retrieve BaiTap from the database
            var baiTap = await _db.Mabaitapdocs.FindAsync(baiTapId);
            if (baiTap == null)
            {
                return NotFound();
            }

            // Retrieve the list of questions for the given baiTapId
            var cauHoiList = await _db.Cauhoibaitapdocs
                                      .Where(c => c.Ma_bai_tap_docId == baiTapId)
                                      .OrderBy(c => c.Thu_tu_cau)
                                      .ToListAsync();

            // Extract user answers from the form
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

            // Process the user answers and calculate results
            foreach (var question in cauHoiList)
            {
                if (userAnswers.TryGetValue(question.Id, out var userAnswer))
                {
                    question.UserAnswer = userAnswer; // Store the user's answer
                    question.IsCorrect = string.Equals(userAnswer, question.Dap_an_dung, StringComparison.OrdinalIgnoreCase); // Check if the answer is correct
                }
            }

            // Lấy các bài đọc liên quan từ danh sách câu hỏi
            var baiDocs = cauHoiList
                .Where(c => !string.IsNullOrEmpty(c.Bai_doc))
                .Select(c => c.Bai_doc)
                .Distinct()
                .ToList();

            // Create the view model
            var viewModel = new TracNghiemViewModel
            {
                BaiTap = baiTap,  // Store exercise details
                CauHoiList = cauHoiList,  // Store the list of questions and their results
                TotalTime = form["TotalTime"], // If you are tracking time
                BaiDocs = baiDocs.ToDictionary(doc => doc, doc => doc)

            };

            // Return the view with the results
            ViewData["NavbarType"] = "_NavbarThoat";

            return View(viewModel);
        }

        public IActionResult Part7()
        {
            List<Ma_bai_tap_doc> mabaitaps = _db.Mabaitapdocs.ToList();
            ViewData["NavbarType"] = "_NavbarBack";
            return View(mabaitaps);
        }

        [HttpGet]
        public async Task<IActionResult> PracticePart7(int baiTapId)
        {
            var baiTap = await _db.Mabaitapdocs
                                  .Include(bt => bt.CauHoiBaiTapDocs) // Lấy tất cả câu hỏi liên quan
                                  .FirstOrDefaultAsync(bt => bt.Id == baiTapId);

            if (baiTap == null)
            {
                return NotFound();
            }

            var cauHoiList = baiTap.CauHoiBaiTapDocs
                                   .OrderBy(c => c.Thu_tu_cau)
                                   .ToList();
            // Lấy các bài đọc liên quan từ danh sách câu hỏi
            var baiDocs = cauHoiList
                .Where(c => !string.IsNullOrEmpty(c.Bai_doc))
                .Select(c => c.Bai_doc)
                .Distinct()
                .ToList();

            var viewModel = new TracNghiemViewModel
            {
                BaiTap = baiTap,
                CauHoiList = cauHoiList,
                BaiDocs = baiDocs.ToDictionary(doc => doc, doc => doc)
            };

            ViewData["NavbarType"] = "_NavbarBack";

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> ResultDetailPart7(IFormCollection form, int baiTapId)
        {
            int correctAnswers = 0;
            int incorrectAnswers = 0;
            int skippedQuestions = 0;

            // Lấy danh sách ID câu hỏi từ form
            var questionid = form["questionIds"].Select(int.Parse).ToList();

            // Truy vấn danh sách câu hỏi từ cơ sở dữ liệu dựa trên danh sách ID
            var cauhoiList = await _db.Cauhoibaitapdocs
                                      .Where(q => questionid.Contains(q.Id))
                                      .ToListAsync();

            // Lặp qua danh sách câu hỏi vừa truy vấn
            foreach (var cauHoi in cauhoiList)
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
            ViewBag.CorrectAnswers = correctAnswers;
            ViewBag.IncorrectAnswers = incorrectAnswers;
            ViewBag.SkippedQuestions = skippedQuestions;
            // Retrieve BaiTap from the database
            var baiTap = await _db.Mabaitapdocs.FindAsync(baiTapId);
            if (baiTap == null)
            {
                return NotFound();
            }

            // Retrieve the list of questions for the given baiTapId
            var cauHoiList = await _db.Cauhoibaitapdocs
                                      .Where(c => c.Ma_bai_tap_docId == baiTapId)
                                      .OrderBy(c => c.Thu_tu_cau)
                                      .ToListAsync();

            // Extract user answers from the form
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

            // Process the user answers and calculate results
            foreach (var question in cauHoiList)
            {
                if (userAnswers.TryGetValue(question.Id, out var userAnswer))
                {
                    question.UserAnswer = userAnswer; // Store the user's answer
                    question.IsCorrect = string.Equals(userAnswer, question.Dap_an_dung, StringComparison.OrdinalIgnoreCase); // Check if the answer is correct
                }
            }

            // Lấy các bài đọc liên quan từ danh sách câu hỏi
            var baiDocs = cauHoiList
                .Where(c => !string.IsNullOrEmpty(c.Bai_doc))
                .Select(c => c.Bai_doc)
                .Distinct()
                .ToList();

            // Create the view model
            var viewModel = new TracNghiemViewModel
            {
                BaiTap = baiTap,  // Store exercise details
                CauHoiList = cauHoiList,  // Store the list of questions and their results
                TotalTime = form["TotalTime"], // If you are tracking time
                BaiDocs = baiDocs.ToDictionary(doc => doc, doc => doc)

            };

            // Return the view with the results
            ViewData["NavbarType"] = "_NavbarThoat";

            return View(viewModel);
        }

    }
}
