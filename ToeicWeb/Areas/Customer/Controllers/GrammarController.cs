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
            var mabai = await _db.Mabainguphaps.FindAsync(mabainguphapId);
            if (mabai == null)
            {
                return NotFound();
            }

            var noidunglist = await _db.Noidungbainguphaps
                                           .Where(c => c.Ma_bai_ngu_phapId == mabainguphapId)
                                           .ToListAsync();

            var viewModel = new BainguphapVM
            {
                Mabainguphap = mabai,
                Noidungbainguphaps = noidunglist
            };
            ViewData["NavbarType"] = "_NavbarBack";
            return View(viewModel);
        }

    }
}
