using Microsoft.AspNetCore.Mvc;

namespace ToeicWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class HomeAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
