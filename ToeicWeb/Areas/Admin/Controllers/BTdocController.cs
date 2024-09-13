﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ToeicWeb.Data;
using ToeicWeb.Models;
using ToeicWeb.Models.ViewModels;
using ToeicWeb.Utility;

namespace ToeicWeb.Areas.Admin.Controllers
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


        //[HttpPost]
        //public JsonResult Insert(Ma_bai_tap_doc model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Mabaitapdocs.Add(model);
        //        _db.SaveChanges();
        //        return Json(new { success = true, message = "Thêm mã thành công" });
        //    }
        //    return Json(new { success = false, message = "Không thể lưu mã bài tập đọc mới" });
        //}

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
                        // Kiểm tra xem mabaidoc đã tồn tại chưa
                        var mabaidoc = _db.Mabaitapdocs.FirstOrDefault(c => c.Tieu_de == viewModel.Tieu_de);
                        if (mabaidoc == null)
                        {
                            mabaidoc = new Ma_bai_tap_doc
                            {
                                Tieu_de = viewModel.Tieu_de,
                                Part = viewModel.Part
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
                                Bai_doc = worksheet.Cells[row, 9].Value?.ToString() ?? string.Empty,
                                Ma_bai_tap_docId = mabaidoc.Id
                            };
                            _db.Cauhoibaitapdocs.Add(cauhoi);
                        }
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public JsonResult Edit(int id)
        {
            var baitapdoc = _db.Mabaitapdocs.Find(id);
            return Json(baitapdoc);
        }

        [HttpPost]
        public JsonResult Update(Ma_bai_tap_doc model)
        {
            if (ModelState.IsValid)
            {
                _db.Mabaitapdocs.Update(model);
                _db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            return Json(new { success = true, message = "Model validation failed" });
        }


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
                _db.Mabaitapdocs.Remove(baitapdoc);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Có lỗi khi xóa" });
        }
        #endregion
    }
}
