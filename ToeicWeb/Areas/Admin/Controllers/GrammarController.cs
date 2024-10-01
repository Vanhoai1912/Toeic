using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Linq;
using Toeic.DataAccess;
using Toeic.Models;
using Toeic.Models.ViewModels;
using Toeic.Utility;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class GrammarController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public GrammarController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new BainguphapVM
            {
                Mabainguphaps = _db.Mabainguphaps.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> Create(BainguphapVM viewModel)
        {
            try
            {
                var uploadImageGraFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "grammar", "bai " + viewModel.Ten_bai.ToString(), "imageGra");
                Directory.CreateDirectory(uploadImageGraFolderPath);

                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                var ImageGraFilePath = "";
                if (viewModel.ImageFileGrammar != null)
                {
                    var fileExtension = Path.GetExtension(viewModel.ImageFileGrammar.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(fileExtension) || !allowedImageExtensions.Contains(fileExtension))
                    {
                        return Json(new { success = false, message = "Invalid file extension for mã bài ngữ pháp image. Allowed extensions are: " + string.Join(", ", allowedImageExtensions) });
                    }

                    if (viewModel.ImageFileGrammar.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(viewModel.ImageFileGrammar.FileName);
                        var fileNameFul = fileName + fileExtension;
                        var filePath = Path.Combine(uploadImageGraFolderPath, fileNameFul);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.ImageFileGrammar.CopyToAsync(stream);
                        }

                        var relativeFilePathGra = Path.Combine("adminn", "upload", "grammar", "bai " + viewModel.Ten_bai.ToString(), "imageGra", fileNameFul).Replace("\\", "/");
                        ImageGraFilePath = relativeFilePathGra;
                    }
                }

                var mabainguphap = _db.Mabainguphaps.FirstOrDefault(c => c.Ten_bai == viewModel.Ten_bai);

                if (mabainguphap == null)
                {
                    mabainguphap = new Ma_bai_ngu_phap
                    {
                        Ten_bai = viewModel.Ten_bai,
                        ImageUrl = ImageGraFilePath
                    };
                    _db.Mabainguphaps.Add(mabainguphap);
                    await _db.SaveChangesAsync();
                }

                // Tạo mới nội dung bài ngữ pháp
                var noidung = new Noi_dung_bai_ngu_phap
                {
                    Noi_dung = viewModel.Noi_dung,  // Gán nội dung từ viewModel
                    Ma_bai_ngu_phapId = mabainguphap.Id
                };

                _db.Noidungbainguphaps.Add(noidung);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Thêm bài ngữ pháp mới thành công" });
            }
            catch (Exception ex)
            {
                // Log the exception to help diagnose the issue
                Console.WriteLine($"Error in Create: {ex}");
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = $"Lỗi khi thêm bài ngữ pháp mới: {innerException}" });
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Ma_bai_ngu_phap> objBnguphapList = _db.Mabainguphaps.ToList();

            return Json(new { data = objBnguphapList });
        }

        [HttpDelete]
        public JsonResult Delete(int? id)
        {
            var bainguphap = _db.Mabainguphaps.Find(id);
            if (bainguphap != null)
            {
                _db.Mabainguphaps.Remove(bainguphap);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }

        #endregion
    }
}
