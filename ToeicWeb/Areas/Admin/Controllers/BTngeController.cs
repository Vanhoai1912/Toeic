using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using ToeicWeb.Data;
using ToeicWeb.Models;
using ToeicWeb.Models.ViewModels;


namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BTngeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;
        public BTngeController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment)
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
            var viewModel = new CauhoiBTngeVM
            {
                Mabaitapnges = _db.Mabaitapnges.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> Create(CauhoiBTngeVM viewModel)
        {
            var uploadImageFolderPath = Path.Combine(_environment.WebRootPath, "customer", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "image");
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

                    var relativeFilePath = Path.Combine("customer", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "image", fileNameFul).Replace("\\", "/");
                    fileImagePaths.Add(relativeFilePath, fileName);
                }
            }


            var uploadAudioFolderPath = Path.Combine(_environment.WebRootPath, "customer", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "audio");
            Directory.CreateDirectory(uploadAudioFolderPath);

            var allowedAudioExtensions = new[] { ".mp3" };

            // Dùng để kiểm tra loại MIME hợp lệ
            var allowedAudioMimeTypes = new[] { "audio/mpeg" };

            var fileAudioPaths = new Dictionary<string, string>();
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

                    var relativeFilePath = Path.Combine("customer", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "audio", fileNameFul).Replace("\\", "/");
                    fileAudioPaths.Add(relativeFilePath, fileName);
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
                // Lưu file vào thư mục wwwroot/customer/upload
                string uploadsExcelFolder = Path.Combine(_environment.WebRootPath, "customer", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "excel");
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
                        var mabainge = _db.Mabaitapnges.FirstOrDefault(c => c.Tieu_de == viewModel.Tieu_de);
                        if (mabainge == null)
                        {
                            mabainge = new Ma_bai_tap_nge
                            {
                                Tieu_de = viewModel.Tieu_de,
                                Part = viewModel.Part,
                                FilePath = excelFilePath
                            };
                            _db.Mabaitapnges.Add(mabainge);
                            await _db.SaveChangesAsync();
                        }
                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var cauhoi = new Cau_hoi_bai_tap_nge
                            {

                                Thu_tu_cau = worksheet.Cells[row, 1].Value != null ? Convert.ToInt32(worksheet.Cells[row, 1].Value) : 0,
                                Cau_hoi = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty,
                                Dap_an_1 = worksheet.Cells[row, 5].Value?.ToString() ?? string.Empty,
                                Dap_an_2 = worksheet.Cells[row, 6].Value?.ToString() ?? string.Empty,
                                Dap_an_3 = worksheet.Cells[row, 7].Value?.ToString() ?? string.Empty,
                                Dap_an_4 = worksheet.Cells[row, 8].Value?.ToString() ?? string.Empty,
                                Dap_an_dung = worksheet.Cells[row, 9].Value?.ToString() ?? string.Empty,
                                Giai_thich = worksheet.Cells[row, 10].Value?.ToString() ?? string.Empty,
                                Ma_bai_tap_ngeId = mabainge.Id
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

                            _db.Cauhoibaitapnges.Add(cauhoi);
                        }
                        await _db.SaveChangesAsync();
                    }
                }

                return Json(new { success = true, message = "Thêm bài nghe mới thành công" });
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = $"Lỗi khi thêm bài nghe mới: {innerException}" });
            }
        }

        #region API CALLS





        [HttpGet]
        public IActionResult GetAll()
        {
            List<Ma_bai_tap_nge> objBtapngeList = _db.Mabaitapnges.ToList();

            return Json(new { data = objBtapngeList });
        }

        [HttpDelete]
        public JsonResult Delete(int? id)
        {
            var baitapnge = _db.Mabaitapnges.Find(id);
            if (baitapnge != null)
            {
                _db.Mabaitapnges.Remove(baitapnge);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }
        #endregion
    }
}

