using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                // Tạo đường dẫn thư mục lưu hình ảnh bài ngữ pháp
                var uploadImageGraFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "grammar", "bai " + viewModel.Ten_bai.ToString(), "imageGra");
                Directory.CreateDirectory(uploadImageGraFolderPath);

                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileFathImage = "";
                // Kiểm tra và xử lý tệp hình ảnh
                if (viewModel.ImageFileGrammar != null)
                {
                    var fileExtension = Path.GetExtension(viewModel.ImageFileGrammar.FileName).ToLowerInvariant();
                    if (!string.IsNullOrEmpty(fileExtension) && allowedImageExtensions.Contains(fileExtension))
                    {
                        // Lưu tệp hình ảnh lên máy chủ
                        var fileName = Path.GetFileNameWithoutExtension(viewModel.ImageFileGrammar.FileName);
                        var fileNameFul = fileName + fileExtension;
                        var filePath = Path.Combine(uploadImageGraFolderPath, fileNameFul);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.ImageFileGrammar.CopyToAsync(stream);
                        }
                        // Lưu đường dẫn tương đối để sử dụng trong ứng dụng web
                        string relativeFilePathGra = Path.Combine("adminn", "upload", "grammar", "bai " + viewModel.Ten_bai.ToString(), "imageGra", fileNameFul).Replace("\\", "/");
                        fileFathImage = relativeFilePathGra;
                    }
                }

                // Kiểm tra bài ngữ pháp trong cơ sở dữ liệu
                var mabainguphap = _db.Mabainguphaps.FirstOrDefault(c => c.Ten_bai == viewModel.Ten_bai);

                if (mabainguphap == null)
                {
                    // Tạo mới bản ghi Ma_bai_ngu_phap
                    mabainguphap = new Ma_bai_ngu_phap
                    {
                        Ten_bai = viewModel.Ten_bai,
                        ImageUrl = fileFathImage, // Lưu đường dẫn thư mục ảnh của bài ngữ pháp
                        Noi_dung = viewModel.Noi_dung // Lưu nội dung bài ngữ pháp
                    };
                    _db.Mabainguphaps.Add(mabainguphap);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // Cập nhật nội dung bài ngữ pháp nếu đã tồn tại
                    mabainguphap.ImageUrl = fileFathImage;
                    mabainguphap.Noi_dung = viewModel.Noi_dung; // Cập nhật nội dung bài ngữ pháp
                    await _db.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Thêm bài ngữ pháp mới thành công" });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để dễ chẩn đoán
                Console.WriteLine($"Error in Create: {ex}");
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = $"Lỗi khi thêm bài ngữ pháp mới: {innerException}" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> Edit(int id)
        {
            var mabai = await _db.Mabainguphaps.FindAsync(id);
            if (mabai == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài ngữ pháp." });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    id = mabai.Id,
                    ten_bai = mabai.Ten_bai,
                    noi_dung = mabai.Noi_dung,
                    filePathImageVoca = mabai.ImageUrl // Trả về đường dẫn ảnh
                }
            });
        }

        [HttpPost]
        public async Task<JsonResult> Update(BainguphapVM viewModel)
        {
            try
            {
                // Tìm bài ngữ pháp cần cập nhật
                var manguphap = await _db.Mabainguphaps.FindAsync(viewModel.Id);
                if (manguphap == null)
                {
                    return Json(new { success = false, message = "Bài ngữ pháp không tồn tại." });
                }

                // Kiểm tra thay đổi tiêu đề, hình ảnh và nội dung
                bool tieuDeChanged = manguphap.Ten_bai != viewModel.Ten_bai;
                bool noiDungChanged = manguphap.Noi_dung != viewModel.Noi_dung;

                // Kiểm tra nếu có tệp hình ảnh mới và so sánh với hình ảnh cũ (nếu có)
                bool imageChanged = viewModel.ImageFileGrammar != null && viewModel.ImageFileGrammar.Length > 0;
                bool imageFileChanged = false;
                if (imageChanged)
                {
                    // So sánh tên tệp mới với tên tệp hình ảnh cũ nếu cần
                    if (!string.IsNullOrEmpty(manguphap.ImageUrl))
                    {
                        string oldFileName = Path.GetFileName(manguphap.ImageUrl); // Lấy tên tệp từ URL cũ
                        string newFileName = viewModel.ImageFileGrammar.FileName; // Tên tệp mới từ file upload

                        imageFileChanged = oldFileName != newFileName; // So sánh tên tệp
                    }
                    else
                    {
                        // Nếu không có hình ảnh cũ, thì chỉ cần kiểm tra có file mới là đủ
                        imageFileChanged = true;
                    }
                }

                // Nếu không có thay đổi
                if (!tieuDeChanged && !noiDungChanged && !imageFileChanged)
                {
                    return Json(new { success = false, message = "Không có thay đổi nào được thực hiện." });
                }


                // Cập nhật tiêu đề nếu thay đổi
                if (tieuDeChanged)
                {
                    var newImageFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "grammar", "bai " + viewModel.Ten_bai, "imageGra");
                    Directory.CreateDirectory(newImageFolderPath);

                    // Di chuyển file ảnh nếu có thay đổi
                    var oldImageFilePath = Path.Combine(_environment.WebRootPath, manguphap.ImageUrl);
                    if (System.IO.File.Exists(oldImageFilePath))
                    {
                        var fileName = Path.GetFileName(oldImageFilePath);
                        var newImageFilePath = Path.Combine(_environment.WebRootPath, newImageFolderPath, fileName);
                        System.IO.File.Move(oldImageFilePath, newImageFilePath);

                        manguphap.ImageUrl = Path.Combine("adminn", "upload", "grammar", "bai " + viewModel.Ten_bai, "imageGra", fileName).Replace("\\", "/");

                        // Xóa thư mục cũ
                        var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "grammar", "bai " + manguphap.Ten_bai);
                        if (Directory.Exists(oldFolderBasePath))
                        {
                            Directory.Delete(oldFolderBasePath, true);
                        }

                    }

                    manguphap.Ten_bai = viewModel.Ten_bai;
                }

                // Cập nhật hình ảnh nếu thay đổi
                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                if (imageChanged)
                {
                    var folderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "grammar", "bai " + viewModel.Ten_bai, "imageGra");
                    Directory.CreateDirectory(folderBasePath);

                    // Xóa ảnh cũ nếu tồn tại
                    if (!string.IsNullOrEmpty(manguphap.ImageUrl))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, manguphap.ImageUrl);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Lưu ảnh mới
                    var fileExtension = Path.GetExtension(viewModel.ImageFileGrammar.FileName).ToLowerInvariant();
                    if (allowedImageExtensions.Contains(fileExtension))
                    {
                        var fileName = Path.GetFileNameWithoutExtension(viewModel.ImageFileGrammar.FileName);
                        var filePath = Path.Combine(folderBasePath, fileName + fileExtension);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.ImageFileGrammar.CopyToAsync(stream);
                        }

                        // Cập nhật đường dẫn ảnh
                        manguphap.ImageUrl = Path.Combine("adminn", "upload", "grammar", "bai " + manguphap.Ten_bai, "imageGra", fileName + fileExtension).Replace("\\", "/");
                    }
                }

                // Cập nhật nội dung
                if (noiDungChanged)
                {
                    manguphap.Noi_dung = viewModel.Noi_dung;
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                _db.Mabainguphaps.Update(manguphap);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật bài ngữ pháp thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during update: " + ex.Message);
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
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
                var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "grammar", "bai " + bainguphap.Ten_bai.ToString());
                if (Directory.Exists(oldFolderBasePath))
                {
                    Directory.Delete(oldFolderBasePath, true);
                }
                _db.Mabainguphaps.Remove(bainguphap);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }

        #endregion
    }
}
