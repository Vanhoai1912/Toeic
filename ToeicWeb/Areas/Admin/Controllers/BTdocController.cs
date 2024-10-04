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

namespace Toeic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class BTdocController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public BTdocController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment)
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
            var viewModel = new CauhoiBTdocVM
            {
                Mabaitapdocs = _db.Mabaitapdocs.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> Create(CauhoiBTdocVM viewModel)
        {
            var uploadImageFolderPath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "image");
            Directory.CreateDirectory(uploadImageFolderPath);


            //Lưu đường dẫn file Image
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            var fileImageBDPaths = new Dictionary<string, string>();
            foreach (var formFile in viewModel.Image_bai_doc)
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
                    fileImageBDPaths.Add(relativeFilePath, fileName);
                }
            }

            if (viewModel.ExcelFile == null || viewModel.ExcelFile.Length == 0)
            {
                return Json(new { success = false, message = "File không được chọn hoặc không có dữ liệu" });
            }

            try
            {
                // Lưu file vào thư mục wwwroot/adminn/upload
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "excel");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string excelFilePath = Path.Combine(uploadsFolder, viewModel.ExcelFile.FileName);
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
                        var mabaidoc = _db.Mabaitapdocs.FirstOrDefault(c => c.Tieu_de == viewModel.Tieu_de);
                        if (mabaidoc == null)
                        {
                            mabaidoc = new Ma_bai_tap_doc
                            {
                                Tieu_de = viewModel.Tieu_de,
                                Part = viewModel.Part,
                                ExcelFilePath = excelFilePath,
                                ImageBDFolderPath = uploadImageFolderPath
                            };
                            _db.Mabaitapdocs.Add(mabaidoc);
                            await _db.SaveChangesAsync();
                        }

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var cauhoi = new Cau_hoi_bai_tap_doc
                            {
                                Thu_tu_cau = worksheet.Cells[row, 1].Value != null ? Convert.ToInt32(worksheet.Cells[row, 1].Value) : 0,
                                Cau_hoi = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty,
                                Dap_an_1 = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty,
                                Dap_an_2 = worksheet.Cells[row, 4].Value?.ToString() ?? string.Empty,
                                Dap_an_3 = worksheet.Cells[row, 5].Value?.ToString() ?? string.Empty,
                                Dap_an_4 = worksheet.Cells[row, 6].Value?.ToString() ?? string.Empty,
                                Dap_an_dung = worksheet.Cells[row, 7].Value?.ToString() ?? string.Empty,
                                Giai_thich = worksheet.Cells[row, 8].Value?.ToString() ?? string.Empty,
                                Giai_thich_bai_doc = worksheet.Cells[row, 10].Value?.ToString() ?? string.Empty,
                                Ma_bai_tap_docId = mabaidoc.Id
                            };
                            for (int i = 0; i < fileImageBDPaths.Count; i++)
                            {
                                if (worksheet.Cells[row, 9].Value?.ToString() == fileImageBDPaths.ElementAt(i).Value)
                                {
                                    cauhoi.Image_bai_doc = fileImageBDPaths.ElementAt(i).Key;
                                }
                            }

                            _db.Cauhoibaitapdocs.Add(cauhoi);
                        }

                        await _db.SaveChangesAsync();
                    }
                }

                return Json(new { success = true, message = "Thêm bài đọc mới thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi thêm bài đọc mới: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> Edit(int id)
        {
            // Tìm bài tập đọc theo ID
            var baitapdoc = await _db.Mabaitapdocs.FindAsync(id);
            if (baitapdoc == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài tập đọc" });
            }

            // Lấy đường dẫn file Excel từ cơ sở dữ liệu
            var filePath = baitapdoc.ExcelFilePath; // Giả sử bạn lưu đường dẫn file trong thuộc tính FilePath

            // Kiểm tra xem file có tồn tại hay không
            bool fileExists = !string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath);

            // Trả về thông tin của bài tập đọc và đường dẫn file Excel
            return Json(new
            {
                success = true,
                data = baitapdoc,
                filePath = fileExists ? filePath : null // Nếu file tồn tại, trả về đường dẫn, nếu không thì trả về null
            });
        }

        [HttpPost]
        public async Task<JsonResult> Update(CauhoiBTdocVM viewModel)
        {
            // Tìm bài tập đọc cần cập nhật
            var baitapdoc = await _db.Mabaitapdocs.FindAsync(viewModel.Id);
            if (baitapdoc == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài tập đọc" });
            }

            // Kiểm tra xem có thay đổi tiêu đề (Tieu_de) hoặc Part không
            bool tieuDeChanged = baitapdoc.Tieu_de != viewModel.Tieu_de;
            bool partChanged = baitapdoc.Part != viewModel.Part;

            var oldExcelFilePath = baitapdoc.ExcelFilePath;
            var oldImageBDFolderPath = baitapdoc.ImageBDFolderPath;
            var oldExcelFileName = Path.GetFileName(oldExcelFilePath);
            // Nếu Tieu_de hoặc Part thay đổi, cần tạo các thư mục mới
            if (tieuDeChanged || partChanged)
            {
                // Tạo đường dẫn mới
                var newFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString());
                Directory.CreateDirectory(newFolderBasePath);


                var newImageBDFolderPath = Path.Combine(newFolderBasePath, "image");
                var newExcelFolderPath = Path.Combine(newFolderBasePath, "excel");

                // Tạo thư mục mới nếu chưa có
                Directory.CreateDirectory(newImageBDFolderPath);
                Directory.CreateDirectory(newExcelFolderPath);

                // Chuyển các file hình ảnh
                if (Directory.Exists(oldImageBDFolderPath))
                {
                    foreach (var filePath in Directory.GetFiles(oldImageBDFolderPath))
                    {
                        var fileName = Path.GetFileName(filePath);
                        var newFilePath = Path.Combine(newImageBDFolderPath, fileName);
                        System.IO.File.Move(filePath, newFilePath);
                    }
                }

                if (!System.IO.File.Exists(baitapdoc.ExcelFilePath))
                {
                    return Json(new { success = false, message = "File Excel không tồn tại." });
                }

                // Chuyển file Excel
                if (System.IO.File.Exists(oldExcelFilePath))
                {
                    var newExcelFilePath = Path.Combine(newExcelFolderPath, oldExcelFileName);
                    System.IO.File.Move(oldExcelFilePath, newExcelFilePath);
                    baitapdoc.ExcelFilePath = newExcelFilePath;
                }

                // Xóa folder cũ
                var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + baitapdoc.Part.ToString(), baitapdoc.Tieu_de.ToString());
                if (Directory.Exists(oldFolderBasePath))
                {
                    Directory.Delete(oldFolderBasePath, true);
                }

                // Cập nhật các đường dẫn mới trong database
                baitapdoc.Tieu_de = viewModel.Tieu_de;
                baitapdoc.Part = viewModel.Part;

                // Cập nhật đường dẫn trong database
                baitapdoc.ImageBDFolderPath = newImageBDFolderPath;
            }

            // Xử lý Image (nếu có file mới)
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            Dictionary<string, string> fileImageBDPaths = new Dictionary<string, string>();
            if (viewModel.Image_bai_doc != null && viewModel.Image_bai_doc.Any())
            {
                // Xóa đường Image cũ trong database
                var cauhoiList = _db.Cauhoibaitapdocs.Where(c => c.Ma_bai_tap_docId == baitapdoc.Id).ToList();
                foreach (var cauhoi in cauhoiList)
                {
                    cauhoi.Image_bai_doc = null;
                }


                var FolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + baitapdoc.Part.ToString(), baitapdoc.Tieu_de.ToString(), "image");
                var ImageFiles = Directory.GetFiles(FolderBasePath);

                foreach (var img in ImageFiles)
                {
                    if (System.IO.File.Exists(img))
                    {
                        System.IO.File.Delete(img);
                    }
                }

                foreach (var formFile in viewModel.Image_bai_doc)
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
                        fileImageBDPaths.Add(relativeFilePath, fileName);
                    }
                }
            }

            // Xử lý Excel (nếu có file mới)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (viewModel.ExcelFile != null && viewModel.ExcelFile.Length > 0)
            {

                // Xóa file excel cũ
                if (!string.IsNullOrEmpty(baitapdoc.ExcelFilePath))
                {
                    if (System.IO.File.Exists(baitapdoc.ExcelFilePath))
                    {
                        System.IO.File.Delete(baitapdoc.ExcelFilePath);
                    }
                }

                // Lưu file Excel mới
                string uploadsExcelFolder = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + viewModel.Part.ToString(), viewModel.Tieu_de.ToString(), "excel");
                string newExcelFilePath = Path.Combine(uploadsExcelFolder, viewModel.ExcelFile.FileName);
                using (var fileStream = new FileStream(newExcelFilePath, FileMode.Create))
                {
                    await viewModel.ExcelFile.CopyToAsync(fileStream);
                }

                baitapdoc.ExcelFilePath = newExcelFilePath;
                var cauhoiList = _db.Cauhoibaitapdocs.Where(c => c.Ma_bai_tap_docId == baitapdoc.Id).ToList();
                _db.Cauhoibaitapdocs.RemoveRange(cauhoiList);


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
                            var cauhoi = new Cau_hoi_bai_tap_doc
                            {
                                Thu_tu_cau = worksheet.Cells[row, 1].Value != null ? Convert.ToInt32(worksheet.Cells[row, 1].Value) : 0,
                                Cau_hoi = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty,
                                Dap_an_1 = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty,
                                Dap_an_2 = worksheet.Cells[row, 4].Value?.ToString() ?? string.Empty,
                                Dap_an_3 = worksheet.Cells[row, 5].Value?.ToString() ?? string.Empty,
                                Dap_an_4 = worksheet.Cells[row, 6].Value?.ToString() ?? string.Empty,
                                Dap_an_dung = worksheet.Cells[row, 7].Value?.ToString() ?? string.Empty,
                                Giai_thich = worksheet.Cells[row, 8].Value?.ToString() ?? string.Empty,
                                Giai_thich_bai_doc = worksheet.Cells[row, 10].Value?.ToString() ?? string.Empty,
                                Ma_bai_tap_docId = baitapdoc.Id
                            };
                            //
                            if (fileImageBDPaths.Count > 0)
                            {
                                for (int i = 0; i < fileImageBDPaths.Count; i++)
                                {
                                    if (worksheet.Cells[row, 9].Value?.ToString() == fileImageBDPaths.ElementAt(i).Value)
                                    {
                                        cauhoi.Image_bai_doc = fileImageBDPaths.ElementAt(i).Key;
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
                                        if (worksheet.Cells[row, 9].Value?.ToString() == fileNameWithoutExtension)
                                        {
                                            cauhoi.Image_bai_doc = newFilePath;
                                        }
                                    }
                                }
                            }

                            row++;
                            _db.Cauhoibaitapdocs.Update(cauhoi);
                        }
                        await _db.SaveChangesAsync();
                    }
                }
            }
            else
            {
                using (var stream = new FileStream(baitapdoc.ExcelFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return Json(new { success = false, message = "Không tìm thấy sheet trong file Excel" });
                        }

                        var cauhoiList = _db.Cauhoibaitapdocs.Where(c => c.Ma_bai_tap_docId == baitapdoc.Id).ToList();


                        if (!fileImageBDPaths.IsNullOrEmpty())
                        {
                            int row = 2;
                            foreach (var cauhoi in cauhoiList)
                            {
                                if (row <= worksheet.Dimension.Rows)
                                {

                                    //
                                    if (fileImageBDPaths.Count > 0)
                                    {
                                        for (int i = 0; i < fileImageBDPaths.Count; i++)
                                        {
                                            if (worksheet.Cells[row, 9].Value?.ToString() == fileImageBDPaths.ElementAt(i).Value)
                                            {
                                                cauhoi.Image_bai_doc = fileImageBDPaths.ElementAt(i).Key;
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
                                                if (worksheet.Cells[row, 9].Value?.ToString() == fileNameWithoutExtension)
                                                {
                                                    cauhoi.Image_bai_doc = newFilePath;
                                                }
                                            }
                                        }
                                    }
                                    row++;
                                    _db.Cauhoibaitapdocs.Update(cauhoi);
                                }
                            }
                        }
                        await _db.SaveChangesAsync();
                    }
                }
            }
            _db.Mabaitapdocs.Update(baitapdoc);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật bài nghe thành công!" });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Ma_bai_tap_doc> objBtapdocList = _db.Mabaitapdocs.ToList();

            return Json(new { data = objBtapdocList });
        }

        [HttpDelete]
        public JsonResult Delete(int? id)
        {
            var baitapdoc = _db.Mabaitapdocs.Find(id);
            if(baitapdoc != null)
            {
                var oldFolderBasePath = Path.Combine(_environment.WebRootPath, "adminn", "upload", "part " + baitapdoc.Part.ToString(), baitapdoc.Tieu_de.ToString());
                if (Directory.Exists(oldFolderBasePath))
                {
                    Directory.Delete(oldFolderBasePath, true);
                }
                _db.Mabaitapdocs.Remove(baitapdoc);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }
        #endregion
    }
}
