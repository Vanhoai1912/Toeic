using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToeicWeb.Utility;

namespace ToeicWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class HomeAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
