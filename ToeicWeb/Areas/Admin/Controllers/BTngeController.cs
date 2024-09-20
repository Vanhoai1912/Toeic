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
                    var uploadFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "image");
                    Directory.CreateDirectory(uploadFolderPath);

                    var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                    var fileNameFul = fileName + fileExtension;
                    var filePath = Path.Combine(uploadFolderPath, fileNameFul);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    var relativeFilePath = Path.Combine("adminn", "upload", "image", fileNameFul).Replace("\\", "/");
                    fileImagePaths.Add(relativeFilePath, fileName);
                }
            }

            //Lưu đường dẫn file Audio
            var allowedAudioExtensions = ".mp3";

            var fileAudioPaths = new Dictionary<string, string>();
            foreach (var formFile in viewModel.AudioFile)
            {
                var fileExtension = Path.GetExtension(formFile.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(fileExtension) || !allowedAudioExtensions.Contains(fileExtension))
                {
                    return Json(new { success = false, message = "Invalid file extension for audio. Allowed extension is: " + allowedAudioExtensions });
                }

                if (formFile.Length > 0)
                {
                    var uploadFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "audio");
                    Directory.CreateDirectory(uploadFolderPath);

                    var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                    var fileNameFul = fileName + fileExtension;
                    var filePath = Path.Combine(uploadFolderPath, fileNameFul);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    var relativeFilePath = Path.Combine("adminn", "upload", "audio", fileNameFul).Replace("\\", "/");
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
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "adminn", "upload");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string filePath = Path.Combine(uploadsFolder, viewModel.ExcelFile.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
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
                                //FilePath = Path.Combine("adminn", "upload", viewModel.ExcelFile.FileName) // Lưu đường dẫn file
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
        //[HttpGet]
        //public async Task<JsonResult> Edit(int id)
        //{
        //    // Tìm bài tập đọc theo ID
        //    var baitapdoc = await _db.Mabaitapdocs.FindAsync(id);
        //    if (baitapdoc == null)
        //    {
        //        return Json(new { success = false, message = "Không tìm thấy bài tập đọc" });
        //    }

        //    // Lấy đường dẫn file Excel từ cơ sở dữ liệu
        //    var filePath = baitapdoc.FilePath; // Giả sử bạn lưu đường dẫn file trong thuộc tính FilePath

        //    // Kiểm tra xem file có tồn tại hay không
        //    bool fileExists = !string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath);

        //    // Trả về thông tin của bài tập đọc và đường dẫn file Excel
        //    return Json(new
        //    {
        //        success = true,
        //        data = baitapdoc,
        //        filePath = fileExists ? filePath : null // Nếu file tồn tại, trả về đường dẫn, nếu không thì trả về null
        //    });
        //}


        //[HttpPost]
        //public async Task<JsonResult> Update(int id, string tieu_de, int part, IFormFile FileExcel)
        //{
        //    // Tìm bài tập đọc cần cập nhật
        //    var baitapdoc = await _db.Mabaitapdocs.FindAsync(id);
        //    if (baitapdoc == null)
        //    {
        //        return Json(new { success = false, message = "Không tìm thấy bài tập đọc" });
        //    }

        //    // Cập nhật thông tin tiêu đề và part
        //    baitapdoc.Tieu_de = tieu_de;
        //    baitapdoc.Part = part;

        //    // Kiểm tra file Excel
        //    if (FileExcel != null && FileExcel.Length > 0)
        //    {
        //        // Xóa file Excel cũ nếu tồn tại
        //        if (!string.IsNullOrEmpty(baitapdoc.FilePath))
        //        {
        //            string oldFilePath = Path.Combine(_environment.WebRootPath, baitapdoc.FilePath);
        //            if (System.IO.File.Exists(oldFilePath))
        //            {
        //                System.IO.File.Delete(oldFilePath);
        //            }
        //        }

        //        // Lưu file mới vào thư mục wwwroot/adminn/upload
        //        string uploadsFolder = Path.Combine(_environment.WebRootPath, "adminn", "upload");
        //        if (!Directory.Exists(uploadsFolder))
        //        {
        //            Directory.CreateDirectory(uploadsFolder);
        //        }

        //        string filePath = Path.Combine(uploadsFolder, FileExcel.FileName);
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await FileExcel.CopyToAsync(fileStream);
        //        }

        //        // Cập nhật đường dẫn file trong cơ sở dữ liệu
        //        baitapdoc.FilePath = Path.Combine("adminn", "upload", FileExcel.FileName);

        //        // Xóa các câu hỏi cũ trong bảng Câu hỏi bài tập đọc
        //        var oldQuestions = _db.Cauhoibaitapdocs.Where(q => q.Ma_bai_tap_docId == baitapdoc.Id);
        //        _db.Cauhoibaitapdocs.RemoveRange(oldQuestions);
        //        await _db.SaveChangesAsync();

        //        // Đọc dữ liệu từ file Excel và thêm câu hỏi mới
        //        using (var stream = new MemoryStream())
        //        {
        //            await FileExcel.CopyToAsync(stream);
        //            using (var package = new ExcelPackage(stream))
        //            {
        //                var worksheet = package.Workbook.Worksheets[0]; // Sheet đầu tiên
        //                int rowCount = worksheet.Dimension.Rows;

        //                for (int row = 2; row <= rowCount; row++) // Bắt đầu từ dòng 2 để bỏ qua tiêu đề
        //                {
        //                    var question = new Cau_hoi_bai_tap_doc
        //                    {
        //                        Thu_tu_cau = worksheet.Cells[row, 1].Value != null ? Convert.ToInt32(worksheet.Cells[row, 1].Value) : 0,
        //                        Cau_hoi = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty,
        //                        Dap_an_1 = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty,
        //                        Dap_an_2 = worksheet.Cells[row, 4].Value?.ToString() ?? string.Empty,
        //                        Dap_an_3 = worksheet.Cells[row, 5].Value?.ToString() ?? string.Empty,
        //                        Dap_an_4 = worksheet.Cells[row, 6].Value?.ToString() ?? string.Empty,
        //                        Dap_an_dung = worksheet.Cells[row, 7].Value?.ToString() ?? string.Empty,
        //                        Giai_thich = worksheet.Cells[row, 8].Value?.ToString() ?? string.Empty,
        //                        Bai_doc = worksheet.Cells[row, 9].Value?.ToString() ?? string.Empty,
        //                        Ma_bai_tap_docId = baitapdoc.Id
        //                    };
        //                    _db.Cauhoibaitapdocs.Add(question);
        //                }
        //            }
        //        }
        //    }

        //    _db.Mabaitapdocs.Update(baitapdoc);
        //    await _db.SaveChangesAsync();

        //    return Json(new { success = true, message = "Sửa bài đọc thành công" });
        //}


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

