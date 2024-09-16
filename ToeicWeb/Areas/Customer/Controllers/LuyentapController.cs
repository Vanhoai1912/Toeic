using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToeicWeb.Data;
using ToeicWeb.Models;

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

        // Phương thức GET để hiển thị bài tập
        [HttpGet]
        public IActionResult PracticePart5(int id)
        {
           
            return View(); 
        }

        // Phương thức POST để xử lý bài tập
        [HttpPost]
        public ActionResult PracticePart5(string lessionId)
        {
        //    this.lessionService = new LessonService();
        //    LessonDto lesson = this.lessionService.GetById(lessionId);
        //    List<QuestionDto> questions = this.lessionService.GetQuestion(lessionId);

        //    ViewData["lesson"] = lesson;
            return View();
        }

        // Phương thức POST để hiển thị kết quả
        [HttpPost]
        public IActionResult ResultPart5(List<int> questionIds, Dictionary<string, string> answers)
        {
           
            return View();
        }



    }
}
