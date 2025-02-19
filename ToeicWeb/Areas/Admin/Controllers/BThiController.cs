using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Linq;
using Toeic.DataAccess;
using Toeic.Models.ViewModels;
using Toeic.Models;
using Microsoft.AspNetCore.Authorization;
using Toeic.Utility;
using Microsoft.EntityFrameworkCore;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BThiController : Controller
    {

        private readonly ILogger<BThiController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public BThiController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment, ILogger<BThiController> logger)
        {
            _db = db;
            _environment = environment;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetUsersByTestResult(int maBaiThiId)
        {
            try
            {
                // Lấy danh sách TestResults và UserAnswers liên kết với MaBaiThiId
                var testResults = _db.TestResults
                    .Where(tr => tr.MabaithiId == maBaiThiId)
                    .Include(tr => tr.UserAnswers)
                    .Include(tr => tr.ApplicationUser) // Kết nối với bảng AspNetUsers
                    .ToList();

                // Tạo danh sách các người dùng và kết quả của họ
                var userResults = testResults.Select(tr => new {
                    UserName = tr.ApplicationUser.Name,
                    Email = tr.ApplicationUser.Email,
                    CorrectAnswers = tr.CorrectAnswers,
                    IncorrectAnswers = tr.IncorrectAnswers,
                    CompletionDate = tr.CompletionDate,
                    SkippedQuestions = tr.SkippedQuestions
                }).ToList();

                return Json(new { success = true, data = userResults });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
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
        public async Task<JsonResult> Edit(int id)
        {
            // Tìm bài tập đọc theo ID
            var mabaithi = await _db.Mabaithis.FindAsync(id);
            if (mabaithi == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài tập đọc" });
            }

            // Lấy số file ảnh trong folder
            var imageFiles = Directory.GetFiles(mabaithi.ImageFolderPath);
            int numberOfImages = imageFiles.Length;

            int? numberOfAudios = null;
            // Lấy số file nghe trong folder
            if (mabaithi.ExamType != "DOC")
            {
                var audioFiles = Directory.GetFiles(mabaithi.AudioFolderPath);
                numberOfAudios = audioFiles.Length;
            }


            // Lấy đường dẫn file Excel từ cơ sở dữ liệu
            var filePath = mabaithi.ExcelFilePath; // Giả sử bạn lưu đường dẫn file trong thuộc tính FilePath

            // Kiểm tra xem file có tồn tại hay không
            bool fileExists = !string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath);

            // Trả về thông tin của bài tập đọc và đường dẫn file Excel
            return Json(new
            {
                success = true,
                data = mabaithi,
                filePath = fileExists ? filePath : null, // Nếu file tồn tại, trả về đường dẫn, nếu không thì trả về null
                numberOfImages = numberOfImages,
                numberOfAudios = numberOfAudios
            });
        }

        [HttpPost]
        public async Task<JsonResult> Update(CauhoiBThiVM viewModel)
        {
            // Tìm đối tượng Ma_bai_tap_nge cần cập nhật
            var mabaithi = await _db.Mabaithis.FindAsync(viewModel.Id);
            if (mabaithi == null)
            {
                return Json(new { success = false, message = "Bài nghe không tồn tại." });
            }

            // Kiểm tra xem có thay đổi 
            bool tieuDeChanged = mabaithi.Tieu_de != viewModel.Tieu_de;
            bool examTypeChanged = mabaithi.ExamType != viewModel.ExamType;
            bool excelChanged = viewModel.ExcelFile != null && viewModel.ExcelFile.Length > 0;
            bool imageChanged = viewModel.ImageFile != null && viewModel.ImageFile.Any();
            bool audioChanged = viewModel.AudioFile != null && viewModel.AudioFile.Any();
            // Kiểm tra nếu không có thay đổi
            if (!tieuDeChanged && !examTypeChanged && !excelChanged && !imageChanged && !audioChanged)
            {
                return Json(new { success = false, message = "Không có thay đổi nào được thực hiện" });
            }

            // Đường dẫn cũ cho Image, Audio và Excel
            var oldImageFolderPath = mabaithi.ImageFolderPath;
            var oldAudioFolderPath = mabaithi.AudioFolderPath;
            var oldExcelFilePath = mabaithi.ExcelFilePath;
            var oldExcelFileName = Path.GetFileName(oldExcelFilePath);
            // Nếu Tieu_de hoặc Part thay đổi, cần tạo các thư mục mới
            if (tieuDeChanged || examTypeChanged)
            {

                // Tạo đường dẫn mới
                var newFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString());
                Directory.CreateDirectory(newFolderBasePath);


                if (examTypeChanged && viewModel.ExamType == "DOC")
                {
                    // Xóa các file âm thanh cũ
                    if (Directory.Exists(oldAudioFolderPath))
                    {
                        Directory.Delete(oldAudioFolderPath, true);
                    }

                    // Xóa dữ liệu âm thanh trong database
                    var cauhoiListt = _db.Cauhoibaithis.Where(c => c.Ma_bai_thiId == mabaithi.Id).ToList();
                    foreach (var ca in cauhoiListt)
                    {
                        ca.Audio = null;
                    }

                    await _db.SaveChangesAsync();
                }

                if (viewModel.ExamType != "DOC")
                {
                    var newAudioFolderPath = Path.Combine(newFolderBasePath, "audio");
                    Directory.CreateDirectory(newAudioFolderPath);

                    // Chuyển các file âm thanh
                    if (Directory.Exists(oldAudioFolderPath))
                    {
                        foreach (var filePath in Directory.GetFiles(oldAudioFolderPath))
                        {
                            var fileName = Path.GetFileName(filePath);
                            var newFilePath = Path.Combine(newAudioFolderPath, fileName);
                            System.IO.File.Move(filePath, newFilePath);
                        }
                    }

                    mabaithi.AudioFolderPath = newAudioFolderPath;
                }
                var newImageFolderPath = Path.Combine(newFolderBasePath, "image");
                var newExcelFolderPath = Path.Combine(newFolderBasePath, "excel");

                // Tạo thư mục mới nếu chưa có
                Directory.CreateDirectory(newImageFolderPath);
                Directory.CreateDirectory(newExcelFolderPath);


                // Chuyển các file hình ảnh

                if (Directory.Exists(oldImageFolderPath))
                {
                    foreach (var filePath in Directory.GetFiles(oldImageFolderPath))
                    {
                        var fileName = Path.GetFileName(filePath);
                        var newFilePath = Path.Combine(newImageFolderPath, fileName);
                        System.IO.File.Move(filePath, newFilePath);
                    }
                }

                if (!System.IO.File.Exists(mabaithi.ExcelFilePath))
                {
                    return Json(new { success = false, message = "File Excel không tồn tại." });
                }


                try
                {
                    // Di chuyển file Excel
                    var newExcelFilePath = Path.Combine(newExcelFolderPath, oldExcelFileName);
                    Console.WriteLine($"Old Excel File Path: {oldExcelFilePath}");
                    Console.WriteLine($"New Excel File Path: {newExcelFilePath}");

                    System.IO.File.Move(oldExcelFilePath, newExcelFilePath);
                    mabaithi.ExcelFilePath = newExcelFilePath;
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi
                    _logger.LogError($"Error moving Excel file: {ex.Message}");
                    return Json(new { success = false, message = "Lỗi khi di chuyển file Excel." });
                }


                // Xóa folder cũ
                var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + mabaithi.ExamType.ToString(), mabaithi.Tieu_de.ToString());
                if (Directory.Exists(oldFolderBasePath))
                {
                    Directory.Delete(oldFolderBasePath, true);
                }

                // Cập nhật các đường dẫn mới trong database
                mabaithi.Tieu_de = viewModel.Tieu_de;
                mabaithi.ExamType = viewModel.ExamType;

                // Cập nhật đường dẫn trong database
                mabaithi.ImageFolderPath = newImageFolderPath;

                _db.Mabaithis.Update(mabaithi);
                await _db.SaveChangesAsync();
            }


            // Xử lý Image (nếu có file mới)
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            Dictionary<string, string> fileImagePaths = new Dictionary<string, string>();
            if (viewModel.ImageFile != null && viewModel.ImageFile.Any())
            {
                // Xóa đường Image cũ trong database
                var cauhoiList = _db.Cauhoibaithis.Where(c => c.Ma_bai_thiId == mabaithi.Id).ToList();
                foreach (var cauhoi in cauhoiList)
                {
                    cauhoi.Image = null;
                }

                var FolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + mabaithi.ExamType.ToString(), mabaithi.Tieu_de.ToString(), "image");
                var ImageFiles = Directory.GetFiles(FolderBasePath);
                foreach (var img in ImageFiles)
                {
                    if (System.IO.File.Exists(img))
                    {
                        System.IO.File.Delete(img);
                    }
                }

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
                        var filePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + viewModel.ExamType, viewModel.Tieu_de, "image", fileNameFul);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        var relativeFilePath = Path.Combine("adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "image", fileNameFul).Replace("\\", "/");
                        fileImagePaths.Add(relativeFilePath, fileName);
                    }
                }
            }

            // Xử lý Audio (nếu có file mới)
            var allowedAudioExtensions = new[] { ".mp3" };
            var allowedAudioMimeTypes = new[] { "audio/mpeg" };
            Dictionary<string, string> fileAudioPaths = new Dictionary<string, string>();
            if (viewModel.AudioFile != null && viewModel.AudioFile.Any())
            {
                // Xóa đường dẫn Audio cũ trong database
                var cauhoiList = _db.Cauhoibaithis.Where(c => c.Ma_bai_thiId == mabaithi.Id).ToList();
                foreach (var cauhoi in cauhoiList)
                {
                    cauhoi.Audio = null;
                }

                var FolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + mabaithi.ExamType.ToString(), mabaithi.Tieu_de.ToString(), "audio");
                var AudioFiles = Directory.GetFiles(FolderBasePath);
                foreach (var audio in AudioFiles)
                {
                    if (System.IO.File.Exists(audio))
                    {
                        System.IO.File.Delete(audio);
                    }
                }


                foreach (var formFile in viewModel.AudioFile)
                {
                    var fileExtension = Path.GetExtension(formFile.FileName).Trim().ToLowerInvariant();
                    var mimeType = formFile.ContentType.ToLowerInvariant();

                    if (string.IsNullOrEmpty(fileExtension) || !allowedAudioExtensions.Contains(fileExtension))
                    {
                        return Json(new { success = false, message = "Invalid file extension for audio. Allowed extensions are: " + string.Join(", ", allowedAudioExtensions) });
                    }

                    if (!allowedAudioMimeTypes.Contains(mimeType))
                    {
                        return Json(new { success = false, message = "Invalid MIME type for audio. Allowed MIME types are: " + string.Join(", ", allowedAudioMimeTypes) });
                    }

                    if (formFile.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(formFile.FileName).Trim();
                        var fileNameFul = fileName + fileExtension;
                        var filePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + viewModel.ExamType, viewModel.Tieu_de, "audio", fileNameFul);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        var relativeFilePath = Path.Combine("adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "audio", fileNameFul).Replace("\\", "/");
                        fileAudioPaths.Add(relativeFilePath, fileName);
                    }
                }
            }

            // Xử lý Excel (nếu có file mới)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (viewModel.ExcelFile != null && viewModel.ExcelFile.Length > 0)
            {

                // Xóa file excel cũ
                if (!string.IsNullOrEmpty(mabaithi.ExcelFilePath))
                {
                    if (System.IO.File.Exists(mabaithi.ExcelFilePath))
                    {
                        System.IO.File.Delete(mabaithi.ExcelFilePath);
                    }
                }

                // Lưu file Excel mới
                string uploadsExcelFolder = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "excel");
                string newExcelFilePath = Path.Combine(uploadsExcelFolder, viewModel.ExcelFile.FileName);
                using (var fileStream = new FileStream(newExcelFilePath, FileMode.Create))
                {
                    await viewModel.ExcelFile.CopyToAsync(fileStream);
                }

                // Xóa dữ liệu cũ trong database
                mabaithi.ExcelFilePath = newExcelFilePath;
                var cauhoiList = _db.Cauhoibaithis.Where(c => c.Ma_bai_thiId == mabaithi.Id).ToList();
                _db.Cauhoibaithis.RemoveRange(cauhoiList);
                try
                {
                    // Xử lý file Excel
                    using (var stream = new MemoryStream())
                    {
                        await viewModel.ExcelFile.CopyToAsync(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets.First();
                            int row = 2;
                            while (row <= worksheet.Dimension.Rows)
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


                                if (viewModel.ExamType != "DOC")
                                {
                                    if (fileAudioPaths.Count > 0)
                                    {
                                        for (int i = 0; i < fileAudioPaths.Count; i++)
                                        {
                                            if (worksheet.Cells[row, 3].Value?.ToString() == fileAudioPaths.ElementAt(i).Value)
                                            {
                                                cauhoi.Audio = fileAudioPaths.ElementAt(i).Key;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var audioFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "audio");
                                        if (Directory.Exists(audioFolderPath))
                                        {
                                            foreach (var filePath in Directory.GetFiles(audioFolderPath))
                                            {
                                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                                var fileName = Path.GetFileName(filePath);
                                                var newFilePath = Path.Combine("adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "audio", fileName);
                                                if (worksheet.Cells[row, 3].Value?.ToString() == fileNameWithoutExtension)
                                                {
                                                    cauhoi.Audio = newFilePath;
                                                }
                                            }
                                        }
                                    }
                                }

                                //
                                if (fileImagePaths.Count > 0)
                                {
                                    for (int i = 0; i < fileImagePaths.Count; i++)
                                    {
                                        if (worksheet.Cells[row, 4].Value?.ToString() == fileImagePaths.ElementAt(i).Value)
                                        {
                                            cauhoi.Image = fileImagePaths.ElementAt(i).Key;
                                        }
                                    }
                                }
                                else
                                {
                                    var imageFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "image");
                                    if (Directory.Exists(imageFolderPath))
                                    {
                                        foreach (var filePath in Directory.GetFiles(imageFolderPath))
                                        {
                                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                            var fileName = Path.GetFileName(filePath);
                                            var newFilePath = Path.Combine("adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "image", fileName);
                                            if (worksheet.Cells[row, 4].Value?.ToString() == fileNameWithoutExtension)
                                            {
                                                cauhoi.Image = newFilePath;
                                            }
                                        }
                                    }
                                }

                                row++;
                                _db.Cauhoibaithis.Update(cauhoi);
                            }
                            await _db.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during updating process.");
                    return Json(new { success = false, message = "Lỗi khi lưu dữ liệu vào cơ sở dữ liệu: " + ex.Message });
                }

            }
            else
            {
                using (var stream = new FileStream(mabaithi.ExcelFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return Json(new { success = false, message = "Không tìm thấy sheet trong file Excel" });
                        }

                        var cauhoiList = _db.Cauhoibaithis.Where(c => c.Ma_bai_thiId == mabaithi.Id).ToList();

                        int row = 2;
                        foreach (var cauhoi in cauhoiList)
                        {
                            if (row <= worksheet.Dimension.Rows)
                            {


                                //
                                if (fileAudioPaths.Count > 0)
                                {
                                    for (int i = 0; i < fileAudioPaths.Count; i++)
                                    {
                                        if (worksheet.Cells[row, 3].Value?.ToString() == fileAudioPaths.ElementAt(i).Value)
                                        {
                                            cauhoi.Audio = fileAudioPaths.ElementAt(i).Key;
                                        }
                                    }
                                }
                                else
                                {

                                    var audioFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "audio");
                                    if (Directory.Exists(audioFolderPath))
                                    {
                                        foreach (var filePath in Directory.GetFiles(audioFolderPath))
                                        {
                                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                            var fileName = Path.GetFileName(filePath);
                                            var newFilePath = Path.Combine("adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "audio", fileName).Replace("\\", "/");
                                            if (worksheet.Cells[row, 3].Value?.ToString() == fileNameWithoutExtension)
                                            {
                                                cauhoi.Audio = newFilePath;
                                            }
                                        }
                                    }
                                }


                                //
                                if (fileImagePaths.Count > 0)
                                {
                                    for (int i = 0; i < fileImagePaths.Count; i++)
                                    {
                                        if (worksheet.Cells[row, 4].Value?.ToString() == fileImagePaths.ElementAt(i).Value)
                                        {
                                            cauhoi.Image = fileImagePaths.ElementAt(i).Key;
                                        }
                                    }
                                }
                                else
                                {
                                    var imageFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "image");
                                    if (Directory.Exists(imageFolderPath))
                                    {
                                        foreach (var filePath in Directory.GetFiles(imageFolderPath))
                                        {
                                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                            var fileName = Path.GetFileName(filePath);
                                            var newFilePath = Path.Combine("adminn", "upload", "bài thi " + viewModel.ExamType.ToString(), viewModel.Tieu_de.ToString(), "image", fileName).Replace("\\", "/");
                                            if (worksheet.Cells[row, 4].Value?.ToString() == fileNameWithoutExtension)
                                            {
                                                cauhoi.Image = newFilePath;
                                            }
                                        }
                                    }
                                }

                                row++;
                            }
                            _db.Cauhoibaithis.Update(cauhoi);
                        }

                    }
                    await _db.SaveChangesAsync();
                }
            }

            return Json(new { success = true, message = "Cập nhật bài thi thành công!" });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Ma_bai_thi> objBThiList = _db.Mabaithis.ToList();

            return Json(new { data = objBThiList });
        }

        public JsonResult Delete(int? id)
        {
            var mabaithi = _db.Mabaithis.Find(id);
            if (mabaithi != null)
            {
                // Xóa thư mục liên quan (nếu có)
                var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "bài thi " + mabaithi.ExamType.ToString(), mabaithi.Tieu_de.ToString());
                if (Directory.Exists(oldFolderBasePath))
                {
                    Directory.Delete(oldFolderBasePath, true);
                }

                // Lấy tất cả các bản ghi trong TestResults liên quan tới mabaithi
                var testResults = _db.TestResults.Where(t => t.MabaithiId == id).ToList();
                if (testResults.Any())
                {
                    foreach (var testResult in testResults)
                    {
                        // Lấy các bản ghi UserAnswers liên quan tới testResult
                        var userAnswers = _db.UserAnswers.Where(u => u.TestResultId == testResult.Id).ToList();

                        // Xóa các bản ghi trong bảng UserAnswers
                        _db.UserAnswers.RemoveRange(userAnswers);

                        // Xóa bản ghi trong bảng TestResults
                        _db.TestResults.Remove(testResult);
                    }
                }

                // Lấy các câu hỏi trong Cauhoibaithis liên quan tới mabaithi
                var questions = _db.Cauhoibaithis.Where(c => c.Ma_bai_thiId == id).ToList();

                // Xóa các bản ghi trong bảng Cauhoibaithis
                _db.Cauhoibaithis.RemoveRange(questions);

                // Xóa bản ghi trong Mabaithis
                _db.Mabaithis.Remove(mabaithi);

                // Lưu thay đổi
                _db.SaveChanges();

                return Json(new { success = true, message = "Xóa thành công" });
            }

            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }
        #endregion
    }

}
