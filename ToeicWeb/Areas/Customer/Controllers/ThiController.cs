using Microsoft.AspNetCore.Mvc;

namespace ToeicWeb.Areas.Customer.Controllers
{
    public class ThiController : Controller
    {
        [Area("Customer")]

        public IActionResult Index()
        {
            return View();
        }
    }
}
