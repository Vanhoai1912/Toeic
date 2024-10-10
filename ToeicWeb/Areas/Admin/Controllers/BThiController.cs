using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Linq;
using Toeic.DataAccess;
using Toeic.Models.ViewModels;
using Toeic.Models;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class BThiController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public BThiController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment)
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
            var viewModel = new CauhoiBThiVM
            {
                Mabaithis = _db.Mabaithis.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> Create(CauhoiBThiVM viewModel)
        {
            var uploadImageFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "image");
            Directory.CreateDirectory(uploadImageFolderPath);


            //Lưu đường dẫn file Image
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            var fileImagePaths = new Dictionary<string, string>();
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
                    var filePath = Path.Combine(uploadImageFolderPath, fileNameFul);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    var relativeFilePath = Path.Combine("adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "image", fileNameFul).Replace("\\", "/");
                    fileImagePaths.Add(relativeFilePath, fileName);
                }
            }


            var uploadAudioFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "audio");
            Directory.CreateDirectory(uploadAudioFolderPath);

            var allowedAudioExtensions = new[] { ".mp3" };

            // Dùng để kiểm tra loại MIME hợp lệ
            var allowedAudioMimeTypes = new[] { "audio/mpeg" };

            var fileAudioPaths = new Dictionary<string, string>();
            if (viewModel.AudioFile != null)
            {
                foreach (var formFile in viewModel.AudioFile)
                {
                    var fileExtension = Path.GetExtension(formFile.FileName).Trim().ToLowerInvariant();
                    var mimeType = formFile.ContentType.ToLowerInvariant(); // Lấy loại MIME của file

                    // Log chi tiết
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

                        var relativeFilePath = Path.Combine("adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "audio", fileNameFul).Replace("\\", "/");
                        fileAudioPaths.Add(relativeFilePath, fileName);
                    }
                }

            }


            //EXCEL
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (viewModel.ExcelFile == null || viewModel.ExcelFile.Length == 0)
            {
                return Json(new { success = false, message = "File không được chọn hoặc không có dữ liệu" });
            }

            try
            {
                // Lưu file vào thư mục wwwroot/adminn/upload
                string uploadsExcelFolder = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "excel");
                if (!Directory.Exists(uploadsExcelFolder))
                {
                    Directory.CreateDirectory(uploadsExcelFolder);
                }

                string excelFilePath = Path.Combine(uploadsExcelFolder, viewModel.ExcelFile.FileName);
                using (var fileStream = new FileStream(excelFilePath, FileMode.Create))
                {
                    await viewModel.ExcelFile.CopyToAsync(fileStream);
                }
                // Xử lý file Excel
                using (var stream = new MemoryStream())
                {
                    await viewModel.ExcelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.First();
                        // Kiểm tra xem mabainge đã tồn tại chưa
                        var mabaithi = _db.Mabaithis.FirstOrDefault(c => c.Tieu_de == viewModel.Tieu_de);
                        if (mabaithi == null)
                        {
                            mabaithi = new Ma_bai_thi
                            {
                                Tieu_de = viewModel.Tieu_de,
                                ExamType = viewModel.ExamType,
                                ExcelFilePath = excelFilePath,
                                ImageFolderPath = uploadImageFolderPath,
                                AudioFolderPath = uploadAudioFolderPath
                            };
                            _db.Mabaithis.Add(mabaithi);
                            await _db.SaveChangesAsync();
                        }
                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var cauhoi = new Cau_hoi_bai_thi
                            {
                                Thu_tu_cau = worksheet.Cells[row, 1].Value != null ? Convert.ToInt32(worksheet.Cells[row, 1].Value) : 0,
                                Cau_hoi = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty,
                                Dap_an_1 = worksheet.Cells[row, 5].Value?.ToString() ?? string.Empty,
                                Dap_an_2 = worksheet.Cells[row, 6].Value?.ToString() ?? string.Empty,
                                Dap_an_3 = worksheet.Cells[row, 7].Value?.ToString() ?? string.Empty,
                                Dap_an_4 = worksheet.Cells[row, 8].Value?.ToString() ?? string.Empty,
                                Dap_an_dung = worksheet.Cells[row, 9].Value?.ToString() ?? string.Empty,
                                Giai_thich_dap_an = worksheet.Cells[row, 10].Value?.ToString() ?? string.Empty,
                                QuestionType = viewModel.ExamType,
                                Ma_bai_thiId = mabaithi.Id
                            };

                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 11].Value?.ToString()))
                            {
                                cauhoi.Transcript = worksheet.Cells[row, 11].Value.ToString();
                            }
                            else
                            {
                                cauhoi.Transcript = string.Empty;
                            }



                            for (int i = 0; i < fileAudioPaths.Count; i++)
                            {
                                if (worksheet.Cells[row, 3].Value?.ToString() == fileAudioPaths.ElementAt(i).Value)
                                {
                                    cauhoi.Audio = fileAudioPaths.ElementAt(i).Key;
                                }
                            }

                            for (int i = 0; i < fileImagePaths.Count; i++)
                            {
                                if (worksheet.Cells[row, 4].Value?.ToString() == fileImagePaths.ElementAt(i).Value)
                                {
                                    cauhoi.Image = fileImagePaths.ElementAt(i).Key;
                                }
                            }



                            _db.Cauhoibaithis.Add(cauhoi);
                        }
                        await _db.SaveChangesAsync();
                    }
                }
                return Json(new { success = true, message = "Thêm bài thi mới thành công" });
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = $"Lỗi khi thêm bài thi mới: {innerException}" });
            }
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Ma_bai_thi> objBThiList = _db.Mabaithis.ToList();

            return Json(new { data = objBThiList });
        }

        [HttpDelete]
        public JsonResult Delete(int? id)
        {
            var mabaithi = _db.Mabaithis.Find(id);
            if (mabaithi != null)
            {
                var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + mabaithi.ExamType.ToString(), mabaithi.Tieu_de.ToString());
                if (Directory.Exists(oldFolderBasePath))
                {
                    Directory.Delete(oldFolderBasePath, true);
                }

                _db.Mabaithis.Remove(mabaithi);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }
        #endregion
    }
}
