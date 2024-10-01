using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToeicWeb.Data;
using ToeicWeb.Models;
using ToeicWeb.Models.ViewModels;

namespace ToeicWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class VocaController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public VocaController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult Index()
        {
            List<Ma_bai_tu_vung> mabaituvung = _db.Mabaituvungs.ToList();
            return View(mabaituvung);
        }
        [HttpGet]
        public async Task<IActionResult> VocabularyDetail(int mabaituvungId)
        {
            var mabai = await _db.Mabaituvungs.FindAsync(mabaituvungId);
            if (mabai == null)
            {
                return NotFound();
            }

            var noidunglist = await _db.Noidungbaituvungs
                                           .Where(c => c.Ma_bai_tu_vungId == mabaituvungId)
                                           .OrderBy(c => c.So_thu_tu)
                                           .ToListAsync();

            var viewModel = new BaituvungVM
            {
                Mabaituvung = mabai,
                Noidungbaituvungs = noidunglist
            };
            ViewData["NavbarType"] = "_NavbarBack";

            return View(viewModel);
        }
        
    }
}
