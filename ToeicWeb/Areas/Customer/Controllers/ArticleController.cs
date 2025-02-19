using Microsoft.AspNetCore.Mvc;

namespace ToeicWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ArticleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ArticleDetail()
        {
            // Đặt ViewData cho loại Navbar (nếu cần)
            ViewData["NavbarType"] = "_NavbarBack";
            return View();
        }
    }
}
