using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToeicWeb.Data;
using ToeicWeb.Models;
using ToeicWeb.Utility;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class QuanlybaidocController : Controller
    {
        private readonly ApplicationDbContext _db;

        public QuanlybaidocController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Bai_tap_doc> baitapdocs = _db.Bai_tap_doc.ToList();
            return View(baitapdocs);
        }
      
        public IActionResult Create()
        {
            Bai_tap_doc baitapdoc = new Bai_tap_doc();
            return PartialView("_AddBaitapdocPartialView", baitapdoc);
        }

        [HttpPost]
        public IActionResult Create(Bai_tap_doc baitapdoc)
        {
            _db.Bai_tap_doc.Add(baitapdoc);
            _db.SaveChanges();
            TempData["success"] = "Tạo thành công!";
            return RedirectToAction("Index");
        }
    }
}
