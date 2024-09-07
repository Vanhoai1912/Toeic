using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToeicWeb.Data;
using ToeicWeb.Models;
using ToeicWeb.Utility;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class QuanlybaidocController : Controller
    {
        private readonly ApplicationDbContext _db;

        public QuanlybaidocController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Insert(Bai_tap_doc model)
        {
            if (ModelState.IsValid)
            {
                _db.Bai_tap_docs.Add(model);
                _db.SaveChanges();
                return Json(new { success = true, message = "Thêm mã thành công" });
            }
            return Json(new { success = false, message = "Không thể lưu mã bài tập đọc mới" });
        }


        #region API CALLS
        [HttpGet]
        public JsonResult Edit(int id)
        {
            var baitapdoc = _db.Bai_tap_docs.Find(id);
            return Json(baitapdoc);
        }

        [HttpPost]
        public JsonResult Update(Bai_tap_doc model)
        {
            if (ModelState.IsValid)
            {
                _db.Bai_tap_docs.Update(model);
                _db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật mã thành công" });
            }
            return Json(new { success = true, message = "Model validation failed" });
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            List<Bai_tap_doc> objBtapdocList = _db.Bai_tap_docs.ToList();

            return Json(new { data = objBtapdocList });
        }

        [HttpDelete]
        public JsonResult Delete(int? id)
        {
            var baitapdoc = _db.Bai_tap_docs.Find(id);
            if(baitapdoc != null)
            {
                _db.Bai_tap_docs.Remove(baitapdoc);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }
        #endregion
    }
}
