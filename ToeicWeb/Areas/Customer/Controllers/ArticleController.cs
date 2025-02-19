using Microsoft.AspNetCore.Mvc;
using Toeic.Models.ViewModels;
using Toeic.Models;
using Microsoft.AspNetCore.Identity;
using Toeic.DataAccess;

namespace ToeicWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public ArticleController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult Index()
        {
            List<Article> article = _db.Articles.ToList();
            return View(article);
        }
        [HttpGet]
        public async Task<IActionResult> ArticleDetail(int articleId)
        {
            var article = await _db.Articles.FindAsync(articleId);
            if (article == null)
            {
                return NotFound();
            }

            var viewModel = new ArticleVM
            {
                Id = article.Id,
                Ten_bai = article.Ten_bai,
                Noi_dung = article.Noi_dung,
                Description = article.Description,
                ImageUrl = article.ImageUrl, // ✅ Lấy đường dẫn ảnh từ DB
                Article = article
            };

            ViewData["NavbarType"] = "_NavbarBack";

            return View(viewModel);
        }

    }
}
