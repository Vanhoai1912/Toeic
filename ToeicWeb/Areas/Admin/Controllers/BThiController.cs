using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Toeic.DataAccess;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class BThiController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public BThiController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
