using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ToeicWeb.Models.ViewModels
{
    public class CauhoiBTdocVM
    {
        public Cau_hoi_bai_tap_doc Cauhoibtdoc { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MabtdoctapList { get; set; }
    }
}
