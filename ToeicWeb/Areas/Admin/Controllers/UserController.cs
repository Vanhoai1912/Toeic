using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toeic.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Toeic.DataAccess;
using Toeic.Models.ViewModels;
using Toeic.Utility;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public UserController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult RoleManagment(string userId)
        {
            string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId),
                RoleList = _db.Roles.Select( i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                })
            };
            RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name;
            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {

            string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagmentVM.ApplicationUser.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name;


            if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
            {
                 ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagmentVM.ApplicationUser.Id);

                _db.ApplicationUsers.Update(applicationUser);
                _db.SaveChanges();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }

            return RedirectToAction("Index");
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUsers.ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            }
            return Json(new { data = objUserList });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Lỗi" });
            }

            if (objFromDb.LockoutEnd !=null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // tk hiện đang bị khóa
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                // tk được mở khóa rồi -> có thể khóa 100 ngày
                objFromDb.LockoutEnd = DateTime.Now.AddDays(100);
            }
           
            _db.SaveChanges();

            return Json(new { success = true, message = "Thành công" });
        }
        #endregion
    }

}