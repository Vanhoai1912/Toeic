using Microsoft.AspNetCore.Mvc;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuanlybaingeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Update()
        {
            return View();
        }   

    }


}
