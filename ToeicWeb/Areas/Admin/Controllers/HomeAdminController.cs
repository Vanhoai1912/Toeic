using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toeic.Utility;

namespace ToeicWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

    public class HomeAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        
    }
}
