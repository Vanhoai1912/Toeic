using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Toeic.DataAccess;
using Toeic.Models.ViewModels;
using Toeic.Models;
using Microsoft.AspNetCore.Authorization;
using Toeic.Utility;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public ArticleController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment)
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
            var viewModel = new ArticleVM
            {
                Articles = _db.Articles.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> Create(ArticleVM viewModel)
        {
            try
            {
                // Tạo đường dẫn thư mục lưu hình ảnh bài ngữ pháp
                var uploadImageGraFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "article", "bai " + viewModel.Ten_bai.ToString(), "imageArticle");
                Directory.CreateDirectory(uploadImageGraFolderPath);

                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileFathImage = "";
                // Kiểm tra và xử lý tệp hình ảnh
                if (viewModel.ImageFileArticle != null)
                {
                    var fileExtension = Path.GetExtension(viewModel.ImageFileArticle.FileName).ToLowerInvariant();
                    if (!string.IsNullOrEmpty(fileExtension) && allowedImageExtensions.Contains(fileExtension))
                    {
                        // Lưu tệp hình ảnh lên máy chủ
                        var fileName = Path.GetFileNameWithoutExtension(viewModel.ImageFileArticle.FileName);
                        var fileNameFul = fileName + fileExtension;
                        var filePath = Path.Combine(uploadImageGraFolderPath, fileNameFul);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.ImageFileArticle.CopyToAsync(stream);
                        }
                        // Lưu đường dẫn tương đối để sử dụng trong ứng dụng web
                        string relativeFilePathGra = Path.Combine("adminn", "upload", "article", "bai " + viewModel.Ten_bai.ToString(), "imageArticle", fileNameFul).Replace("\\", "/");
                        fileFathImage = relativeFilePathGra;
                    }
                }

                // Kiểm tra bài ngữ pháp trong cơ sở dữ liệu
                var article = _db.Articles.FirstOrDefault(c => c.Ten_bai == viewModel.Ten_bai);

                if (article == null)
                {
                    // Tạo mới bản ghi article
                    article = new Article
                    {
                        Ten_bai = viewModel.Ten_bai,
                        ImageUrl = fileFathImage, // Lưu đường dẫn thư mục ảnh của article
                        Noi_dung = viewModel.Noi_dung, // Lưu nội dung article
                        Description = viewModel.Description // Lưu nội dung article

                    };
                    _db.Articles.Add(article);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // Cập nhật nội dung article nếu đã tồn tại
                    article.ImageUrl = fileFathImage;
                    article.Noi_dung = viewModel.Noi_dung; // Cập nhật nội dung article
                    article.Description = viewModel.Description;
                    await _db.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Thêm bài báo mới thành công" });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để dễ chẩn đoán
                Console.WriteLine($"Error in Create: {ex}");
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = $"Lỗi khi thêm bài báo mới: {innerException}" });
            }
        }


        [HttpGet]
        public async Task<JsonResult> Edit(int id)
        {
            var mabai = await _db.Articles.FindAsync(id);
            if (mabai == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài báo." });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    id = mabai.Id,
                    ten_bai = mabai.Ten_bai,
                    description = mabai.Description,
                    noi_dung = mabai.Noi_dung,
                    filePathImageArticle = mabai.ImageUrl // Trả về đường dẫn ảnh
                }
            });
        }

        [HttpPost]
        public async Task<JsonResult> Update(ArticleVM viewModel)
        {
            try
            {
                // Tìm bài báo cần cập nhật
                var article = await _db.Articles.FindAsync(viewModel.Id);
                if (article == null)
                {
                    return Json(new { success = false, message = "Bài ngữ pháp không tồn tại." });
                }

                // Kiểm tra thay đổi tiêu đề, description, hình ảnh và nội dung
                bool tieuDeChanged = article.Ten_bai != viewModel.Ten_bai;
                bool noiDungChanged = article.Noi_dung != viewModel.Noi_dung;
                bool descriptionChanged = article.Description != viewModel.Description;


                // Kiểm tra nếu có tệp hình ảnh mới và so sánh với hình ảnh cũ (nếu có)
                bool imageChanged = viewModel.ImageFileArticle != null && viewModel.ImageFileArticle.Length > 0;
                bool imageFileChanged = false;
                if (imageChanged)
                {
                    // So sánh tên tệp mới với tên tệp hình ảnh cũ nếu cần
                    if (!string.IsNullOrEmpty(article.ImageUrl))
                    {
                        string oldFileName = Path.GetFileName(article.ImageUrl); // Lấy tên tệp từ URL cũ
                        string newFileName = viewModel.ImageFileArticle.FileName; // Tên tệp mới từ file upload

                        imageFileChanged = oldFileName != newFileName; // So sánh tên tệp
                    }
                    else
                    {
                        // Nếu không có hình ảnh cũ, thì chỉ cần kiểm tra có file mới là đủ
                        imageFileChanged = true;
                    }
                }

                // Nếu không có thay đổi
                if (!tieuDeChanged && !noiDungChanged && !descriptionChanged && !imageFileChanged)
                {
                    return Json(new { success = false, message = "Không có thay đổi nào được thực hiện." });
                }


                // Cập nhật tiêu đề nếu thay đổi
                if (tieuDeChanged)
                {
                    var newImageFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "article", "bai " + viewModel.Ten_bai, "imageArticle");
                    Directory.CreateDirectory(newImageFolderPath);

                    // Di chuyển file ảnh nếu có thay đổi
                    var oldImageFilePath = Path.Combine(_environment.WebRootPath, article.ImageUrl);
                    if (System.IO.File.Exists(oldImageFilePath))
                    {
                        var fileName = Path.GetFileName(oldImageFilePath);
                        var newImageFilePath = Path.Combine(_environment.WebRootPath, newImageFolderPath, fileName);
                        System.IO.File.Move(oldImageFilePath, newImageFilePath);

                        article.ImageUrl = Path.Combine("adminn", "upload", "article", "bai " + viewModel.Ten_bai, "imageArticle", fileName).Replace("\\", "/");

                        // Xóa thư mục cũ
                        var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "article", "bai " + article.Ten_bai);
                        if (Directory.Exists(oldFolderBasePath))
                        {
                            Directory.Delete(oldFolderBasePath, true);
                        }

                    }

                    article.Ten_bai = viewModel.Ten_bai;
                }

                // Cập nhật description nếu thay đổi
                if (descriptionChanged)
                {
                    article.Description = viewModel.Description;
                }

                // Cập nhật hình ảnh nếu thay đổi
                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                if (imageChanged)
                {
                    var folderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "article", "bai " + viewModel.Ten_bai, "imageArticle");
                    Directory.CreateDirectory(folderBasePath);

                    // Xóa ảnh cũ nếu tồn tại
                    if (!string.IsNullOrEmpty(article.ImageUrl))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, article.ImageUrl);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Lưu ảnh mới
                    var fileExtension = Path.GetExtension(viewModel.ImageFileArticle.FileName).ToLowerInvariant();
                    if (allowedImageExtensions.Contains(fileExtension))
                    {
                        var fileName = Path.GetFileNameWithoutExtension(viewModel.ImageFileArticle.FileName);
                        var filePath = Path.Combine(folderBasePath, fileName + fileExtension);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.ImageFileArticle.CopyToAsync(stream);
                        }

                        // Cập nhật đường dẫn ảnh
                        article.ImageUrl = Path.Combine("adminn", "upload", "article", "bai " + article.Ten_bai, "imageArticle", fileName + fileExtension).Replace("\\", "/");
                    }
                }

                // Cập nhật nội dung
                if (noiDungChanged)
                {
                    article.Noi_dung = viewModel.Noi_dung;
                }


                article.UpdatedAt = DateTime.UtcNow.AddHours(7);

                // Lưu thay đổi vào cơ sở dữ liệu
                _db.Articles.Update(article);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật bài báo thành công!" });
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
            List<Article> objArticleList = _db.Articles.ToList();

            return Json(new { data = objArticleList });
        }

        [HttpDelete]
        public JsonResult Delete(int? id)
        {
            var article = _db.Articles.Find(id);
            if (article != null)
            {
                var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "article", "bai " + article.Ten_bai.ToString());
                if (Directory.Exists(oldFolderBasePath))
                {
                    Directory.Delete(oldFolderBasePath, true);
                }
                _db.Articles.Remove(article);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }

        #endregion
    }
}