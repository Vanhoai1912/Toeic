using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ToeicWeb.Models.ViewModels
{
    public class CauhoiBTdocVM
    {
        public IFormFile ExcelFile { get; set; }
        public string Tieu_de { get; set; }
        public int Part { get; set; }
        public List<Ma_bai_tap_doc> Mabaitapdocs { get; set; }
    }
}
