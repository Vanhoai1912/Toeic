using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using System.Text;
using ToeicWeb.Data;
using ToeicWeb.Models;
using ToeicWeb.Models.ViewModels;
using ToeicWeb.Utility;

namespace ToeicWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CauhoiBTdocController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly IWebHostEnvironment _environment;

        public CauhoiBTdocController(UserManager<IdentityUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;

        }
        public IActionResult Index()
        {
            var viewModel = new CauhoiBTdocVM
            {
                Mabaitapdocs = _db.Mabaitapdocs.ToList()
            };
            return View(viewModel);
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
        public async Task<IActionResult> Create(CauhoiBTdocVM viewModel)
        {
            if (viewModel.ExcelFile != null && viewModel.ExcelFile.Length > 0)
            {
                // Lưu file vào thư mục wwwroot/uploads
                string uploadsFolder = Path.Combine(_environment.WebRootPath,"wwwroot", "adminn",  "upload");
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
                        // Kiểm tra xem category đã tồn tại chưa
                        var category = _db.Mabaitapdocs.FirstOrDefault(c => c.Tieu_de == viewModel.Tieu_de);
                        if (category == null)
                        {
                            category = new Ma_bai_tap_doc
                            {
                                Tieu_de = viewModel.Tieu_de,
                                Part = viewModel.Part
                            };
                            _db.Mabaitapdocs.Add(category);
                            await _db.SaveChangesAsync();
                        }
                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var product = new Cau_hoi_bai_tap_doc
                            {
                                Thu_tu_cau = worksheet.Cells[row, 1].Value != null ? Convert.ToInt32(worksheet.Cells[row, 1].Value) : 0,
                                Cau_hoi = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty,
                                Dap_an_1 = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty,
                                Dap_an_2 = worksheet.Cells[row, 4].Value?.ToString() ?? string.Empty,
                                Dap_an_3 = worksheet.Cells[row, 5].Value?.ToString() ?? string.Empty,
                                Dap_an_4 = worksheet.Cells[row, 6].Value?.ToString() ?? string.Empty,
                                Dap_an_dung = worksheet.Cells[row, 7].Value?.ToString() ?? string.Empty,
                                Giai_thich = worksheet.Cells[row, 8].Value?.ToString() ?? string.Empty,
                                Bai_doc = worksheet.Cells[row, 9].Value?.ToString() ?? string.Empty,
                                Ma_bai_tap_docId = category.Id
                            };
                            _db.Cauhoibaitapdocs.Add(product);
                        }
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var objCauhoiList = _db.Cauhoibaitapdocs.ToList();

            return Json(new { data = objCauhoiList });
        }

        [HttpDelete]
        public JsonResult Delete(int? id)
        {
            var cauhoiBTdoc = _db.Cauhoibaitapdocs.Find(id);
            if (cauhoiBTdoc != null)
            {
                _db.Cauhoibaitapdocs.Remove(cauhoiBTdoc);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }
        #endregion

    }
}
