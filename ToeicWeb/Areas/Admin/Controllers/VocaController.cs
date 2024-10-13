using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Linq;
using Toeic.Utility;
using Toeic.Models;
using Toeic.Models.ViewModels;
using Toeic.DataAccess;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
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
            // Folder for mã bài từ vựng image
            var uploadImageMavocaFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "imageMaVoca");
            Directory.CreateDirectory(uploadImageMavocaFolderPath);

            var uploadImageFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "image");
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

                    var relativeFilePath = Path.Combine("adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "image", fileNameFul).Replace("\\", "/");
                    fileImagePaths.Add(relativeFilePath, fileName);
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

                    var relativeFilePathMavoca = Path.Combine("adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "imageMaVoca", fileNameFul).Replace("\\", "/");
                    ImageMavocaFilePath = relativeFilePathMavoca;
                }
            }

            var uploadAudioFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "audio");
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

                    var relativeFilePath = Path.Combine("adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "audio", fileNameFul).Replace("\\", "/");
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
                // Lưu file vào thư mục wwwroot/adminn/upload
                string uploadsExcelFolder = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "excel");
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
                        var matuvung = _db.Mabaituvungs.FirstOrDefault(c => c.Ten_bai == viewModel.Ten_bai);
                        if (matuvung == null)
                        {
                            matuvung = new Ma_bai_tu_vung
                            {
                                Ten_bai = viewModel.Ten_bai,
                                ExcelFilePath = excelFilePath,
                                ImageFolderPath = uploadImageFolderPath,
                                AudioFolderPath = uploadAudioFolderPath,
                                ImageUrl = ImageMavocaFilePath
                            };
                            _db.Mabaituvungs.Add(matuvung);
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
                                Ma_bai_tu_vungId = matuvung.Id
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

                return Json(new { success = true, message = "Thêm bài nghe mới thành công" });
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = $"Lỗi khi thêm bài nghe mới: {innerException}" });
            }
        }


        public async Task<JsonResult> Edit(int id)
        {
            var baituvung = await _db.Mabaituvungs.FindAsync(id);
            if (baituvung == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài tập đọc" });
            }

            var imageFiles = Directory.GetFiles(baituvung.ImageFolderPath);
            int numberOfImages = imageFiles.Length;

            var audioFiles = Directory.GetFiles(baituvung.AudioFolderPath);
            int numberOfAudios = audioFiles.Length;

            var filePath = baituvung.ExcelFilePath;

            bool fileExists = !string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath);

            var filePathImageVoca = baituvung.ImageUrl;

            // Bỏ qua kiểm tra File.Exists nếu đây là URL web
            bool fileExist = !string.IsNullOrEmpty(filePathImageVoca);

            return Json(new
            {
                success = true,
                data = baituvung,
                filePath = fileExists ? filePath : null,
                numberOfImages = numberOfImages,
                numberOfAudios = numberOfAudios,
                filePathImageVoca = fileExist ? filePathImageVoca : null
            });
        }

        [HttpPost]
        public async Task<JsonResult> Update(BaituvungVM viewModel)
        {
            // Tìm đối tượng Ma_bai_tap_nge cần cập nhật
            var matuvung = await _db.Mabaituvungs.FindAsync(viewModel.Id);
            if (matuvung == null)
            {
                return Json(new { success = false, message = "Bài từ vựng không tồn tại." });
            }

            // Kiểm tra xem có thay đổi 
            bool tieuDeChanged = matuvung.Ten_bai != viewModel.Ten_bai;
            bool excelChanged = viewModel.ExcelFile != null && viewModel.ExcelFile.Length > 0;
            bool imageChanged = viewModel.ImageFile != null && viewModel.ImageFile.Any();
            bool imageVocaChanged = viewModel.ImageFileMavoca != null && viewModel.ImageFileMavoca.Length > 0;
            bool audioChanged = viewModel.AudioFile != null && viewModel.AudioFile.Any();
            // Kiểm tra nếu không có thay đổi
            if (!tieuDeChanged && !imageVocaChanged && !excelChanged && !imageChanged && !audioChanged)
            {
                return Json(new { success = false, message = "Không có thay đổi nào được thực hiện" });
            }

            // Đường dẫn cũ cho Image, Audio và Excel
            var oldImageFolderPath = matuvung.ImageFolderPath;
            var oldImageVocaFolderPath = matuvung.ImageUrl;
            var oldAudioFolderPath = matuvung.AudioFolderPath;
            var oldExcelFilePath = matuvung.ExcelFilePath;
            var oldExcelFileName = Path.GetFileName(oldExcelFilePath);


            // Nếu Tieu_de hoặc Part thay đổi, cần tạo các thư mục mới
            if (tieuDeChanged)
            {
                // Tạo đường dẫn mới
                var newFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString());
                Directory.CreateDirectory(newFolderBasePath);

                var newImageFolderPath = Path.Combine(newFolderBasePath, "image");
                var newImageVocaFolderPath = Path.Combine(newFolderBasePath, "imageMaVoca");
                var newAudioFolderPath = Path.Combine(newFolderBasePath, "audio");
                var newExcelFolderPath = Path.Combine(newFolderBasePath, "excel");



                // Tạo thư mục mới nếu chưa có
                Directory.CreateDirectory(newImageFolderPath);
                Directory.CreateDirectory(newImageVocaFolderPath);
                Directory.CreateDirectory(newAudioFolderPath);
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

                // Chuyển các file hình ảnh ma voca

                var oldImageFilePath = Path.Combine(_environment.WebRootPath, matuvung.ImageUrl);
                if (System.IO.File.Exists(oldImageFilePath))
                {
                    var fileName = Path.GetFileName(oldImageFilePath);
                    var newImageFilePath = Path.Combine(_environment.WebRootPath, newImageVocaFolderPath, fileName);
                    System.IO.File.Move(oldImageFilePath, newImageFilePath);

                    matuvung.ImageUrl = Path.Combine("adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "imageMaVoca", fileName).Replace("\\", "/");
                }

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

                if (!System.IO.File.Exists(matuvung.ExcelFilePath))
                {
                    return Json(new { success = false, message = "File Excel không tồn tại." });
                }


                // Chuyển file Excel
                if (System.IO.File.Exists(oldExcelFilePath))
                {
                    var newExcelFilePath = Path.Combine(newExcelFolderPath, oldExcelFileName);
                    System.IO.File.Move(oldExcelFilePath, newExcelFilePath);
                    matuvung.ExcelFilePath = newExcelFilePath;
                }

                // Xóa folder cũ
                var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + matuvung.Ten_bai.ToString());
                if (Directory.Exists(oldFolderBasePath))
                {
                    Directory.Delete(oldFolderBasePath, true);
                }

                // Cập nhật các đường dẫn mới trong database
                matuvung.Ten_bai = viewModel.Ten_bai;

                // Cập nhật đường dẫn trong database
                matuvung.ImageFolderPath = newImageFolderPath;
                matuvung.AudioFolderPath = newAudioFolderPath;

                _db.Mabaituvungs.Update(matuvung);
                await _db.SaveChangesAsync();
            }

            // Xử lý Image (nếu có file mới)
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            Dictionary<string, string> fileImagePaths = new Dictionary<string, string>();
            if (viewModel.ImageFile != null && viewModel.ImageFile.Any())
            {
                // Xóa đường Image cũ trong database
                var cauhoiList = _db.Noidungbaituvungs.Where(c => c.Ma_bai_tu_vungId == matuvung.Id).ToList();
                foreach (var cauhoi in cauhoiList)
                {
                    cauhoi.ImageUrl = null;
                }

                var FolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + matuvung.Ten_bai.ToString(), "image");
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
                        var filePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai, "image", fileNameFul);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        var relativeFilePath = Path.Combine("adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "image", fileNameFul).Replace("\\", "/");
                        fileImagePaths.Add(relativeFilePath, fileName);
                    }
                }
            }

            // Cập nhật ảnh voca
            if (viewModel.ImageFileMavoca != null)
            {
                var folderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai, "imageMaVoca");
                Directory.CreateDirectory(folderBasePath);

                // Xóa ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(matuvung.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, matuvung.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Lưu ảnh mới
                var fileExtension = Path.GetExtension(viewModel.ImageFileMavoca.FileName).ToLowerInvariant();
                if (allowedImageExtensions.Contains(fileExtension))
                {
                    var fileName = Path.GetFileNameWithoutExtension(viewModel.ImageFileMavoca.FileName);
                    var filePath = Path.Combine(folderBasePath, fileName + fileExtension);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImageFileMavoca.CopyToAsync(stream);
                    }

                    // Cập nhật đường dẫn ảnh
                    matuvung.ImageUrl = Path.Combine("adminn", "upload", "voca", "bai " + matuvung.Ten_bai, "imageMaVoca", fileName + fileExtension).Replace("\\", "/");
                }
            }


            // Xử lý Audio (nếu có file mới)
            var allowedAudioExtensions = new[] { ".mp3" };
            var allowedAudioMimeTypes = new[] { "audio/mpeg" };
            Dictionary<string, string> fileAudioPaths = new Dictionary<string, string>();
            if (viewModel.AudioFile != null && viewModel.AudioFile.Any())
            {
                // Xóa đường dẫn Audio cũ trong database
                var cauhoiList = _db.Noidungbaituvungs.Where(c => c.Ma_bai_tu_vungId == matuvung.Id).ToList();
                foreach (var cauhoi in cauhoiList)
                {
                    cauhoi.Audio = null;
                }

                var FolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + matuvung.Ten_bai.ToString(), "audio");
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
                        var filePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai, "audio", fileNameFul);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        var relativeFilePath = Path.Combine("adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "audio", fileNameFul).Replace("\\", "/");
                        fileAudioPaths.Add(relativeFilePath, fileName);
                    }
                }
            }

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    // Xử lý Excel (nếu có file mới)
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    if (viewModel.ExcelFile != null && viewModel.ExcelFile.Length > 0)
                    {

                        // Xóa file excel cũ
                        if (!string.IsNullOrEmpty(matuvung.ExcelFilePath))
                        {
                            if (System.IO.File.Exists(matuvung.ExcelFilePath))
                            {
                                System.IO.File.Delete(matuvung.ExcelFilePath);
                            }
                        }

                        // Lưu file Excel mới
                        string uploadsExcelFolder = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "excel");
                        string newExcelFilePath = Path.Combine(uploadsExcelFolder, viewModel.ExcelFile.FileName);
                        using (var fileStream = new FileStream(newExcelFilePath, FileMode.Create))
                        {
                            await viewModel.ExcelFile.CopyToAsync(fileStream);
                        }

                        matuvung.ExcelFilePath = newExcelFilePath;
                        var cauhoiList = _db.Noidungbaituvungs.Where(c => c.Ma_bai_tu_vungId == matuvung.Id).ToList();
                        _db.Noidungbaituvungs.RemoveRange(cauhoiList);

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
                                    var cauhoi = new Noi_dung_bai_tu_vung
                                    {
                                        So_thu_tu = worksheet.Cells[row, 1].Value != null ? Convert.ToInt32(worksheet.Cells[row, 1].Value) : 0,
                                        Tu_vung = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty,
                                        Nghia = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty,
                                        Phien_am = worksheet.Cells[row, 6].Value?.ToString() ?? string.Empty,
                                        Vi_du = worksheet.Cells[row, 7].Value?.ToString() ?? string.Empty,
                                        Tu_dong_nghia = worksheet.Cells[row, 8].Value?.ToString() ?? string.Empty,
                                        Tu_trai_nghia = worksheet.Cells[row, 9].Value?.ToString() ?? string.Empty,
                                        Ma_bai_tu_vungId = matuvung.Id
                                    };

                                    if (fileAudioPaths.Count > 0)
                                    {
                                        for (int i = 0; i < fileAudioPaths.Count; i++)
                                        {
                                            if (worksheet.Cells[row, 4].Value?.ToString() == fileAudioPaths.ElementAt(i).Value)
                                            {
                                                cauhoi.Audio = fileAudioPaths.ElementAt(i).Key;
                                                
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var audioFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "audio");
                                        if (Directory.Exists(audioFolderPath))
                                        {
                                            foreach (var filePath in Directory.GetFiles(audioFolderPath))
                                            {
                                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                                var fileName = Path.GetFileName(filePath);
                                                var newFilePath = Path.Combine("adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "audio", fileName);
                                                if (worksheet.Cells[row, 4].Value?.ToString() == fileNameWithoutExtension)
                                                {
                                                    cauhoi.Audio = newFilePath;
                                                }
                                            }
                                        }
                                    }
                                    if (string.IsNullOrEmpty(cauhoi.Audio))
                                    {
                                        throw new Exception("File audio bị sai hoặc thiếu.");
                                    }


                                    //
                                    if (fileImagePaths.Count > 0)
                                    {
                                        for (int i = 0; i < fileImagePaths.Count; i++)
                                        {
                                            if (worksheet.Cells[row, 5].Value?.ToString() == fileImagePaths.ElementAt(i).Value)
                                            {
                                                cauhoi.ImageUrl = fileImagePaths.ElementAt(i).Key;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var imageFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "image");
                                        if (Directory.Exists(imageFolderPath))
                                        {
                                            foreach (var filePath in Directory.GetFiles(imageFolderPath))
                                            {
                                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                                var fileName = Path.GetFileName(filePath);
                                                var newFilePath = Path.Combine("adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "image", fileName);
                                                if (worksheet.Cells[row, 5].Value?.ToString() == fileNameWithoutExtension)
                                                {
                                                    cauhoi.ImageUrl = newFilePath;
                                                }
                                            }
                                        }
                                    }
                                    if (string.IsNullOrEmpty(cauhoi.ImageUrl))
                                    {
                                        throw new Exception("File image bị sai hoặc thiếu.");
                                    }

                                    row++;
                                    _db.Noidungbaituvungs.Update(cauhoi);
                                }
                            }
                        }
                    }
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Trả về kết quả thành công dưới dạng JSON
                    return Json(new { success = true, message = "Cập nhật thành công!" });
                }
                catch (Exception ex)
                {
                    // Hoàn tác các thay đổi nếu có lỗi
                    await transaction.RollbackAsync();

                    // Trả về kết quả lỗi dưới dạng JSON
                    return Json(new { success = false, message = $"Lỗi cập nhật: {ex.Message}" });
                }
            }
        }
            /*else
            {
                using (var stream = new FileStream(matuvung.ExcelFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return Json(new { success = false, message = "Không tìm thấy sheet trong file Excel" });
                        }

                        var cauhoiList = _db.Noidungbaituvungs.Where(c => c.Ma_bai_tu_vungId == matuvung.Id).ToList();

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
                                        if (worksheet.Cells[row, 4].Value?.ToString() == fileAudioPaths.ElementAt(i).Value)
                                        {
                                            cauhoi.Audio = fileAudioPaths.ElementAt(i).Key;
                                        }
                                    }
                                }
                                else
                                {
                                    var audioFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "audio");
                                    if (Directory.Exists(audioFolderPath))
                                    {
                                        foreach (var filePath in Directory.GetFiles(audioFolderPath))
                                        {
                                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                            var fileName = Path.GetFileName(filePath);
                                            var newFilePath = Path.Combine("adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "audio", fileName);
                                            if (worksheet.Cells[row, 4].Value?.ToString() == fileNameWithoutExtension)
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
                                        if (worksheet.Cells[row, 5].Value?.ToString() == fileImagePaths.ElementAt(i).Value)
                                        {
                                            cauhoi.ImageUrl = fileImagePaths.ElementAt(i).Key;
                                        }
                                    }
                                }
                                else
                                {
                                    var imageFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "image");
                                    if (Directory.Exists(imageFolderPath))
                                    {
                                        foreach (var filePath in Directory.GetFiles(imageFolderPath))
                                        {
                                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                            var fileName = Path.GetFileName(filePath);
                                            var newFilePath = Path.Combine("adminn", "upload", "voca", "bai " + viewModel.Ten_bai.ToString(), "image", fileName);
                                            if (worksheet.Cells[row, 5].Value?.ToString() == fileNameWithoutExtension)
                                            {
                                                cauhoi.ImageUrl = newFilePath;
                                            }
                                        }
                                    }
                                }


                                row++;
                            }
                            _db.Noidungbaituvungs.Update(cauhoi);
                        }
                        await _db.SaveChangesAsync();
                    }
                }
            }
            _db.Mabaituvungs.Update(matuvung);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật bài từ vựng thành công!" });
        }*/

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
                var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "voca", "bai " + baituvung.Ten_bai.ToString());
                if (Directory.Exists(oldFolderBasePath))
                {
                    Directory.Delete(oldFolderBasePath, true);
                }
                _db.Mabaituvungs.Remove(baituvung);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }
        #endregion
    }
}
