using Microsoft.AspNetCore.Mvc;

namespace ToeicWeb.Areas.Customer.Controllers
{
    public class ThiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
