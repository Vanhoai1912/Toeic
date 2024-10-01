using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Linq;
using ToeicWeb.Data;
using ToeicWeb.Models;
using ToeicWeb.Models.ViewModels;
using ToeicWeb.Utility;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class VocaController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public VocaController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment)
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
            var viewModel = new BaituvungVM
            {
                Mabaituvungs = _db.Mabaituvungs.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> Create(BaituvungVM viewModel)
        {
            try
            {
                // Folder for multiple images
                var uploadImageFolderPath = Path.Combine(_environment.WebRootPath, "customer", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "image");
                Directory.CreateDirectory(uploadImageFolderPath); 

                // Folder for mã bài từ vựng image
                var uploadImageMavocaFolderPath = Path.Combine(_environment.WebRootPath, "customer", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "imageMaVoca");
                Directory.CreateDirectory(uploadImageMavocaFolderPath); 

                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                // Save multiple images to 'image' folder
                var fileImagePaths = new Dictionary<string, string>();
                if (viewModel.ImageFile != null)
                {
                    foreach (var formFile in viewModel.ImageFile)
                    {
                        var fileExtension = Path.GetExtension(formFile.FileName).ToLowerInvariant();
                        if (string.IsNullOrEmpty(fileExtension) || !allowedImageExtensions.Contains(fileExtension))
                        {
                            return Json(new { success = false, message = "Invalid file extension for images. Allowed extensions are: " + string.Join(", ", allowedImageExtensions) });
                        }

                        if (formFile.Length > 0)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                            var fileNameFul = fileName + fileExtension;
                            var filePath = Path.Combine(uploadImageFolderPath, fileNameFul); // Save to 'image' folder
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }

                            var relativeFilePath = Path.Combine("customer", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "image", fileNameFul).Replace("\\", "/");
                            fileImagePaths.Add(relativeFilePath, fileName);
                        }
                    }
                }

                // File ảnh mã bài từ vựng
                var ImageMavocaFilePath = ""; 
                if (viewModel.ImageFileMavoca != null)
                {
                    var fileExtension = Path.GetExtension(viewModel.ImageFileMavoca.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(fileExtension) || !allowedImageExtensions.Contains(fileExtension))
                    {
                        return Json(new { success = false, message = "Invalid file extension for mã bài từ vựng image. Allowed extensions are: " + string.Join(", ", allowedImageExtensions) });
                    }

                    if (viewModel.ImageFileMavoca.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(viewModel.ImageFileMavoca.FileName);
                        var fileNameFul = fileName + fileExtension;
                        var filePath = Path.Combine(uploadImageMavocaFolderPath, fileNameFul); 
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.ImageFileMavoca.CopyToAsync(stream);
                        }

                        var relativeFilePathMavoca = Path.Combine("customer", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "imageMaVoca", fileNameFul).Replace("\\", "/");
                        ImageMavocaFilePath = relativeFilePathMavoca;
                    }
                }

                var uploadAudioFolderPath = Path.Combine(_environment.WebRootPath, "customer", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "audio");
                Directory.CreateDirectory(uploadAudioFolderPath);

                var allowedAudioExtensions = new[] { ".mp3" };
                var allowedAudioMimeTypes = new[] { "audio/mpeg" };

                var fileAudioPaths = new Dictionary<string, string>();
                if (viewModel.AudioFile != null)
                {
                    foreach (var formFile in viewModel.AudioFile)
                    {
                        var fileExtension = Path.GetExtension(formFile.FileName).Trim().ToLowerInvariant();
                        var mimeType = formFile.ContentType.ToLowerInvariant();

                        Console.WriteLine($"File name: {formFile.FileName}, Extension: {fileExtension}, MIME: {mimeType}");

                        if (string.IsNullOrEmpty(fileExtension) || !allowedAudioExtensions.Contains(fileExtension))
                        {
                            Console.WriteLine($"Invalid extension detected: {fileExtension}. Expected: {string.Join(", ", allowedAudioExtensions)}");
                            return Json(new { success = false, message = "Invalid file extension for audio. Allowed extensions are: " + string.Join(", ", allowedAudioExtensions) });
                        }

                        if (!allowedAudioMimeTypes.Contains(mimeType))
                        {
                            Console.WriteLine($"Invalid MIME type detected: {mimeType}. Expected: {string.Join(", ", allowedAudioMimeTypes)}");
                            return Json(new { success = false, message = "Invalid MIME type for audio. Allowed MIME types are: " + string.Join(", ", allowedAudioMimeTypes) });
                        }

                        if (formFile.Length > 0)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName).Trim();
                            var fileNameFul = fileName + fileExtension;
                            var filePath = Path.Combine(uploadAudioFolderPath, fileNameFul);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }

                            var relativeFilePath = Path.Combine("customer", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "audio", fileNameFul).Replace("\\", "/");
                            fileAudioPaths.Add(relativeFilePath, fileName);
                        }
                    }
                }

                // EXCEL
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                if (viewModel.ExcelFile == null || viewModel.ExcelFile.Length == 0)
                {
                    return Json(new { success = false, message = "File không được chọn hoặc không có dữ liệu" });
                }

                string uploadsExcelFolder = Path.Combine(_environment.WebRootPath, "customer", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "excel");
                if (!Directory.Exists(uploadsExcelFolder))
                {
                    Directory.CreateDirectory(uploadsExcelFolder);
                }

                string excelFilePath = Path.Combine(uploadsExcelFolder, viewModel.ExcelFile.FileName);
                using (var fileStream = new FileStream(excelFilePath, FileMode.Create))
                {
                    await viewModel.ExcelFile.CopyToAsync(fileStream);
                }

                using (var stream = new MemoryStream())
                {
                    await viewModel.ExcelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.First();
                        var mabaituvung = _db.Mabaituvungs.FirstOrDefault(c => c.Ten_bai == viewModel.Ten_bai);

                        if (mabaituvung == null)
                        {
                            mabaituvung = new Ma_bai_tu_vung
                            {
                                Ten_bai = viewModel.Ten_bai,
                                FilePath = excelFilePath,
                                ImageUrl = ImageMavocaFilePath
                            };
                            _db.Mabaituvungs.Add(mabaituvung);
                            await _db.SaveChangesAsync();
                        }

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var noidung = new Noi_dung_bai_tu_vung
                            {
                                So_thu_tu = worksheet.Cells[row, 1].Value != null ? Convert.ToInt32(worksheet.Cells[row, 1].Value) : 0,
                                Tu_vung = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty,
                                Nghia = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty,
                                Phien_am = worksheet.Cells[row, 6].Value?.ToString() ?? string.Empty,
                                Vi_du = worksheet.Cells[row, 7].Value?.ToString() ?? string.Empty,
                                Tu_dong_nghia = worksheet.Cells[row, 8].Value?.ToString() ?? string.Empty,
                                Tu_trai_nghia = worksheet.Cells[row, 9].Value?.ToString() ?? string.Empty,
                                Ma_bai_tu_vungId = mabaituvung.Id
                            };

                            for (int i = 0; i < fileAudioPaths.Count; i++)
                            {
                                if (worksheet.Cells[row, 4].Value?.ToString() == fileAudioPaths.ElementAt(i).Value)
                                {
                                    noidung.Audio = fileAudioPaths.ElementAt(i).Key;
                                }
                            }

                            for (int i = 0; i < fileImagePaths.Count; i++)
                            {
                                if (worksheet.Cells[row, 5].Value?.ToString() == fileImagePaths.ElementAt(i).Value)
                                {
                                    noidung.ImageUrl = fileImagePaths.ElementAt(i).Key;
                                }
                            }

                            _db.Noidungbaituvungs.Add(noidung);
                        }
                        await _db.SaveChangesAsync();
                    }
                }

                return Json(new { success = true, message = "Thêm bài từ vựng mới thành công" });
            }
            catch (Exception ex)
            {
                // Log the exception to help diagnose the issue
                Console.WriteLine($"Error in Create: {ex}");
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = $"Lỗi khi thêm bài từ vựng mới: {innerException}" });
            }
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Ma_bai_tu_vung> objBtuvungList = _db.Mabaituvungs.ToList();

            return Json(new { data = objBtuvungList });
        }

        [HttpDelete]
        public JsonResult Delete(int? id)
        {
            var baituvung = _db.Mabaituvungs.Find(id);
            if (baituvung != null)
            {
                _db.Mabaituvungs.Remove(baituvung);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }   
      
        #endregion
    }
}
