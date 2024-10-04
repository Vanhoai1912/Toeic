using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using Toeic.DataAccess;
using Toeic.Models;
using Toeic.Models.ViewModels;


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
            var uploadImageFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "image");
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

                    var relativeFilePath = Path.Combine("adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "image", fileNameFul).Replace("\\", "/");
                    fileImagePaths.Add(relativeFilePath, fileName);
                }
            }


            var uploadAudioFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "audio");
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

                    var relativeFilePath = Path.Combine("adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "audio", fileNameFul).Replace("\\", "/");
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
                string uploadsExcelFolder = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "excel");
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
                                ExcelFilePath = excelFilePath,
                                ImageFolderPath = uploadImageFolderPath,
                                AudioFolderPath = uploadAudioFolderPath
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

        public async Task<JsonResult> Edit(int id)
        {
            // Tìm bài tập đọc theo ID
            var baitapnge = await _db.Mabaitapnges.FindAsync(id);
            if (baitapnge == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài tập đọc" });
            }

            // Lấy đường dẫn file Excel từ cơ sở dữ liệu
            var filePath = baitapnge.ExcelFilePath; // Giả sử bạn lưu đường dẫn file trong thuộc tính FilePath

            // Kiểm tra xem file có tồn tại hay không
            bool fileExists = !string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath);

            // Trả về thông tin của bài tập đọc và đường dẫn file Excel
            return Json(new
            {
                success = true,
                data = baitapnge,
                filePath = fileExists ? filePath : null // Nếu file tồn tại, trả về đường dẫn, nếu không thì trả về null
            });
        }

        [HttpPost]
        public async Task<JsonResult> Update(CauhoiBTngeVM viewModel)
        {
            // Tìm đối tượng Ma_bai_tap_nge cần cập nhật
            var mabainge = await _db.Mabaitapnges.FindAsync(viewModel.Id);
            if (mabainge == null)
            {
                return Json(new { success = false, message = "Bài nghe không tồn tại." });
            }

            // Kiểm tra xem có thay đổi tiêu đề (Tieu_de) hoặc Part không
            bool tieuDeChanged = mabainge.Tieu_de != viewModel.Tieu_de;
            bool partChanged = mabainge.Part != viewModel.Part;

            // Đường dẫn cũ cho Image, Audio và Excel
            var oldImageFolderPath = mabainge.ImageFolderPath;
            var oldAudioFolderPath = mabainge.AudioFolderPath;
            var oldExcelFilePath = mabainge.ExcelFilePath;
            var oldExcelFileName = Path.GetFileName(oldExcelFilePath);
            // Nếu Tieu_de hoặc Part thay đổi, cần tạo các thư mục mới
            if (tieuDeChanged || partChanged)
            {
                // Tạo đường dẫn mới
                var newFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString());
                Directory.CreateDirectory(newFolderBasePath);


                var newImageFolderPath = Path.Combine(newFolderBasePath, "image");
                var newAudioFolderPath = Path.Combine(newFolderBasePath, "audio");
                var newExcelFolderPath = Path.Combine(newFolderBasePath, "excel");



                // Tạo thư mục mới nếu chưa có
                Directory.CreateDirectory(newImageFolderPath);
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

                if (!System.IO.File.Exists(mabainge.ExcelFilePath))
                {
                    return Json(new { success = false, message = "File Excel không tồn tại." });
                }


                // Chuyển file Excel
                if (System.IO.File.Exists(oldExcelFilePath))
                {
                    var newExcelFilePath = Path.Combine(newExcelFolderPath, oldExcelFileName);
                    System.IO.File.Move(oldExcelFilePath, newExcelFilePath);
                    mabainge.ExcelFilePath = newExcelFilePath;
                }

                // Xóa folder cũ
                var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + mabainge.Part.ToString(), mabainge.Tieu_de.ToString());
                if (Directory.Exists(oldFolderBasePath))
                {
                    Directory.Delete(oldFolderBasePath, true);
                }

                // Cập nhật các đường dẫn mới trong database
                mabainge.Tieu_de = viewModel.Tieu_de;
                mabainge.Part = viewModel.Part;

                // Cập nhật đường dẫn trong database
                mabainge.ImageFolderPath = newImageFolderPath;
                mabainge.AudioFolderPath = newAudioFolderPath;
            }

            // Xử lý Image (nếu có file mới)
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            Dictionary<string, string> fileImagePaths = new Dictionary<string, string>();
            if (viewModel.ImageFile != null && viewModel.ImageFile.Any())
            {
                // Xóa đường Image cũ trong database
                var cauhoiList = _db.Cauhoibaitapnges.Where(c => c.Ma_bai_tap_ngeId == mabainge.Id).ToList();
                foreach (var cauhoi in cauhoiList)
                {
                    cauhoi.Image = null;
                }

                var FolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + mabainge.Part.ToString(), mabainge.Tieu_de.ToString(), "image");
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
                        var filePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part, viewModel.Tieu_de, "image", fileNameFul);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        var relativeFilePath = Path.Combine("adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "image", fileNameFul).Replace("\\", "/");
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
                var cauhoiList = _db.Cauhoibaitapnges.Where(c => c.Ma_bai_tap_ngeId == mabainge.Id).ToList();
                foreach (var cauhoi in cauhoiList)
                {
                    cauhoi.Audio = null;
                }

                var FolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + mabainge.Part.ToString(), mabainge.Tieu_de.ToString(), "audio");
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
                        var filePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part, viewModel.Tieu_de, "audio", fileNameFul);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        var relativeFilePath = Path.Combine("adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "audio", fileNameFul).Replace("\\", "/");
                        fileAudioPaths.Add(relativeFilePath, fileName);
                    }
                }
            }

            // Xử lý Excel (nếu có file mới)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (viewModel.ExcelFile != null && viewModel.ExcelFile.Length > 0)
            {

                // Xóa file excel cũ
                if (!string.IsNullOrEmpty(mabainge.ExcelFilePath))
                {
                    if (System.IO.File.Exists(mabainge.ExcelFilePath))
                    {
                        System.IO.File.Delete(mabainge.ExcelFilePath);
                    }
                }

                // Lưu file Excel mới
                string uploadsExcelFolder = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "excel");
                string newExcelFilePath = Path.Combine(uploadsExcelFolder, viewModel.ExcelFile.FileName);
                using (var fileStream = new FileStream(newExcelFilePath, FileMode.Create))
                {
                    await viewModel.ExcelFile.CopyToAsync(fileStream);
                }

                mabainge.ExcelFilePath = newExcelFilePath;
                var cauhoiList = _db.Cauhoibaitapnges.Where(c => c.Ma_bai_tap_ngeId == mabainge.Id).ToList();
                _db.Cauhoibaitapnges.RemoveRange(cauhoiList);

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
                                var audioFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "audio");
                                if (Directory.Exists(audioFolderPath))
                                {
                                    foreach (var filePath in Directory.GetFiles(audioFolderPath))
                                    {
                                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                        var fileName = Path.GetFileName(filePath);
                                        var newFilePath = Path.Combine("adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "audio", fileName);
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
                                var imageFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "image");
                                if (Directory.Exists(imageFolderPath))
                                {
                                    foreach (var filePath in Directory.GetFiles(imageFolderPath))
                                    {
                                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                        var fileName = Path.GetFileName(filePath);
                                        var newFilePath = Path.Combine("adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "image", fileName);
                                        if (worksheet.Cells[row, 4].Value?.ToString() == fileNameWithoutExtension)
                                        {
                                            cauhoi.Image = newFilePath;
                                        }
                                    }
                                }
                            }

                            row++;
                            _db.Cauhoibaitapnges.Update(cauhoi);
                        }
                        await _db.SaveChangesAsync();
                    }
                }
            }
            else
            {
                using (var stream = new FileStream(mabainge.ExcelFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return Json(new { success = false, message = "Không tìm thấy sheet trong file Excel" });
                        }

                        var cauhoiList = _db.Cauhoibaitapnges.Where(c => c.Ma_bai_tap_ngeId == mabainge.Id).ToList();
                        if (!fileAudioPaths.IsNullOrEmpty() || !fileImagePaths.IsNullOrEmpty())
                        {
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
                                        var audioFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "audio");
                                        if (Directory.Exists(audioFolderPath))
                                        {
                                            foreach (var filePath in Directory.GetFiles(audioFolderPath))
                                            {
                                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                                var fileName = Path.GetFileName(filePath);
                                                var newFilePath = Path.Combine("adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "audio", fileName);
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
                                        var imageFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "image");
                                        if (Directory.Exists(imageFolderPath))
                                        {
                                            foreach (var filePath in Directory.GetFiles(imageFolderPath))
                                            {
                                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                                var fileName = Path.GetFileName(filePath);
                                                var newFilePath = Path.Combine("adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "image", fileName);
                                                if (worksheet.Cells[row, 4].Value?.ToString() == fileNameWithoutExtension)
                                                {
                                                    cauhoi.Image = newFilePath;
                                                }
                                            }
                                        }
                                    }


                                    row++;
                                }
                                _db.Cauhoibaitapnges.Update(cauhoi);
                            }
                        }
                        await _db.SaveChangesAsync();
                    }
                }
            }
            _db.Mabaitapnges.Update(mabainge);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật bài nghe thành công!" });
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
                var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + baitapnge.Part.ToString(), baitapnge.Tieu_de.ToString());
                if (Directory.Exists(oldFolderBasePath))
                {
                    Directory.Delete(oldFolderBasePath, true);
                }

                _db.Mabaitapnges.Remove(baitapnge);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }
        #endregion
    }
}

