using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Toeic.DataAccess;
using Toeic.Models.ViewModels;
using Toeic.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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


        public IActionResult ThiTH()
        {
            // Lấy danh sách bài thi
            List<Ma_bai_thi> mabaitaps = _db.Mabaithis.ToList();

            // Lấy danh sách các bài thi đã làm của người dùng hiện tại
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID người dùng từ Claims
            var completedTests = _db.TestResults
                                    .Where(tr => tr.ApplicationUserId == userId)
                                    .Select(tr => tr.MabaithiId)
                                    .ToList();

            // Tạo tuple chứa cả hai danh sách
            var model = Tuple.Create(mabaitaps, completedTests);

            // Đặt ViewData nếu cần
            ViewData["NavbarType"] = "_NavbarBack";

            return View(model);
        }

        // NGHE
        public IActionResult ThiNGHE()
        {
            // Lấy danh sách bài thi
            List<Ma_bai_thi> mabaitaps = _db.Mabaithis.ToList();

            // Lấy danh sách các bài thi đã làm của người dùng hiện tại
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID người dùng từ Claims
            var completedTests = _db.TestResults
                                    .Where(tr => tr.ApplicationUserId == userId)
                                    .Select(tr => tr.MabaithiId)
                                    .ToList();

            // Tạo tuple chứa cả hai danh sách
            var model = Tuple.Create(mabaitaps, completedTests);

            // Đặt ViewData nếu cần
            ViewData["NavbarType"] = "_NavbarBack";

            return View(model);
        }

        // DOC
        public IActionResult ThiDOC()
        {
            // Lấy danh sách bài thi
            List<Ma_bai_thi> mabaitaps = _db.Mabaithis.ToList();

            // Lấy danh sách các bài thi đã làm của người dùng hiện tại
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID người dùng từ Claims
            var completedTests = _db.TestResults
                                    .Where(tr => tr.ApplicationUserId == userId)
                                    .Select(tr => tr.MabaithiId)
                                    .ToList();

            // Tạo tuple chứa cả hai danh sách
            var model = Tuple.Create(mabaitaps, completedTests);

            // Đặt ViewData nếu cần
            ViewData["NavbarType"] = "_NavbarBack";

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PracticeNGHE(int baiTapId)
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

        [HttpGet]
        public async Task<IActionResult> PracticeDOC(int baiTapId)
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


        [HttpGet]
        public async Task<IActionResult> PracticeTH(int baiTapId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID của người dùng hiện tại

            // Kiểm tra xem người dùng đã có kết quả của bài thi này chưa
            var testResult = await _db.TestResults
                                      .FirstOrDefaultAsync(r => r.MabaithiId == baiTapId && r.ApplicationUserId == userId);

            if (testResult != null)
            {
                // Nếu đã có kết quả, chuyển hướng tới trang kết quả
                return RedirectToAction("ResultDetail", new { baiTapId = baiTapId });
            }

            // Nếu chưa có kết quả, hiển thị trang làm bài
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
        public async Task<IActionResult> ResultDetail(IFormCollection form, int baiTapId)
        {
            int correctAnswers = 0;
            int incorrectAnswers = 0;
            int skippedQuestions = 0;

            var questionid = form["questionIds"].Select(int.Parse).ToList();
            var cauhoiList = await _db.Cauhoibaithis
                                      .Where(q => questionid.Contains(q.Id))
                                      .ToListAsync();

            // Lấy thời gian làm bài từ form
            var totalTimeString = form["TotalTime"];
            TimeSpan totalTime = TimeSpan.Parse(totalTimeString);

            // Lấy userId từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Nếu user chưa đăng nhập
            if (userId == null)
            {
                // Hiển thị thông báo cho người dùng rằng kết quả sẽ không được lưu
                TempData["Notification"] = "Bạn chưa đăng nhập, kết quả sẽ không được lưu.";

                // Xử lý câu trả lời nhưng không lưu kết quả vào cơ sở dữ liệu
                var userAnswers = new List<UserAnswer>();
                foreach (var cauHoi in cauhoiList)
                {
                    var key = $"cauHoi_{cauHoi.Id}";
                    var selectedAnswer = string.Empty;

                    if (form.ContainsKey(key))
                    {
                        selectedAnswer = form[key].FirstOrDefault();
                    }

                    userAnswers.Add(new UserAnswer
                    {
                        CauHoiId = cauHoi.Id,
                        Answer = selectedAnswer,
                        IsCorrect = selectedAnswer == cauHoi.Dap_an_dung
                    });

                    if (selectedAnswer == cauHoi.Dap_an_dung)
                    {
                        correctAnswers++;
                    }
                    else if (string.IsNullOrEmpty(selectedAnswer))
                    {
                        skippedQuestions++;
                    }
                    else
                    {
                        incorrectAnswers++;
                    }
                }

                // Hiển thị kết quả mà không lưu
                var baiTap = await _db.Mabaithis.FindAsync(baiTapId);
                var cauHoiListForView = await _db.Cauhoibaithis
                                                 .Where(c => c.Ma_bai_thiId == baiTapId)
                                                 .ToListAsync();

                var viewModel = new TracNghiemViewModel
                {
                    Baithi = baiTap,
                    CauHoiBaiThiList = cauHoiListForView,
                    UserAnswers = userAnswers,
                    TestResult = new TestResult
                    {
                        CorrectAnswers = correctAnswers,
                        IncorrectAnswers = incorrectAnswers,
                        SkippedQuestions = skippedQuestions,
                        Duration = totalTime
                    }
                };

                ViewData["NavbarType"] = "_NavbarThoat";
                return View("ResultDetail", viewModel);
            }

            // Nếu user đã đăng nhập, lưu kết quả như bình thường
            var testResult = new TestResult
            {
                MabaithiId = baiTapId,
                ApplicationUserId = userId,
                CorrectAnswers = correctAnswers,
                IncorrectAnswers = incorrectAnswers,
                SkippedQuestions = skippedQuestions,
                CompletionDate = DateTime.Now,
                Duration = totalTime
            };

            _db.TestResults.Add(testResult);
            await _db.SaveChangesAsync();

            // Lưu câu trả lời vào cơ sở dữ liệu
            foreach (var cauHoi in cauhoiList)
            {
                var key = $"cauHoi_{cauHoi.Id}";
                var selectedAnswer = string.Empty;

                if (form.ContainsKey(key))
                {
                    selectedAnswer = form[key].FirstOrDefault();
                }

                var userAnswer = new UserAnswer
                {
                    TestResultId = testResult.Id,
                    CauHoiId = cauHoi.Id,
                    Answer = selectedAnswer,
                    IsCorrect = selectedAnswer == cauHoi.Dap_an_dung
                };

                if (userAnswer.IsCorrect)
                {
                    correctAnswers++;
                }
                else if (string.IsNullOrEmpty(selectedAnswer))
                {
                    skippedQuestions++;
                }
                else
                {
                    incorrectAnswers++;
                }

                _db.UserAnswers.Add(userAnswer);
            }

            // Cập nhật lại kết quả bài thi
            testResult.CorrectAnswers = correctAnswers;
            testResult.IncorrectAnswers = incorrectAnswers;
            testResult.SkippedQuestions = skippedQuestions;

            await _db.SaveChangesAsync();

            // Hiển thị kết quả
            var userAnswersSaved = await _db.UserAnswers
                                            .Where(ua => ua.TestResultId == testResult.Id)
                                            .ToListAsync();

            var baiTapDetails = await _db.Mabaithis.FindAsync(baiTapId);
            var cauHoiListForResult = await _db.Cauhoibaithis
                                               .Where(c => c.Ma_bai_thiId == baiTapId)
                                               .ToListAsync();

            var viewModelLoggedIn = new TracNghiemViewModel
            {
                Baithi = baiTapDetails,
                CauHoiBaiThiList = cauHoiListForResult,
                UserAnswers = userAnswersSaved,
                TestResult = testResult
            };

            ViewData["NavbarType"] = "_NavbarThoat";
            return View("ResultDetail", viewModelLoggedIn);
        }



        [HttpGet]
        public async Task<IActionResult> ResultDetail(int baiTapId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Lấy kết quả từ bảng TestResult
            var testResult = await _db.TestResults
                                      .FirstOrDefaultAsync(r => r.MabaithiId == baiTapId && r.ApplicationUserId == userId);

            if (testResult == null)
            {
                return NotFound();
            }

            var baiTap = await _db.Mabaithis.FindAsync(baiTapId);

            // Lấy danh sách câu hỏi
            var cauHoiList = await _db.Cauhoibaithis
                                      .Where(c => c.Ma_bai_thiId == baiTapId)
                                      .ToListAsync();

            // Lấy danh sách câu trả lời của người dùng từ bảng UserAnswer
            var userAnswers = await _db.UserAnswers
                                       .Where(ua => ua.TestResultId == testResult.Id)
                                       .ToListAsync();

            // Tạo view model và gán câu trả lời của người dùng vào từng câu hỏi
            // Tạo view model và gán câu trả lời của người dùng vào từng câu hỏi
            var viewModel = new TracNghiemViewModel
            {
                Baithi = baiTap,
                CauHoiBaiThiList = cauHoiList,
                UserAnswers = userAnswers, // Câu trả lời của người dùng
                TestResult = testResult // Thêm TestResult vào ViewModel
            };
            ViewBag.UserAnswers = userAnswers;  // Dùng để hiển thị trên view
            ViewData["NavbarType"] = "_NavbarBack";

            return View(viewModel);
        }



    }
}
