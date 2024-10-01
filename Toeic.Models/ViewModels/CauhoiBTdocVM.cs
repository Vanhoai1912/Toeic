using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Toeic.Models.ViewModels
{
    public class CauhoiBTdocVM
    {
        public IFormFile ExcelFile { get; set; }
        public string Tieu_de { get; set; }
        public int Id { get; set; }
        public int Part { get; set; }
        public List<Ma_bai_tap_doc> Mabaitapdocs { get; set; }
        public Ma_bai_tap_doc MaBaiTapDoc { get; set; }
    }
}
