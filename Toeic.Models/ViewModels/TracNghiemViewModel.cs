namespace Toeic.Models.ViewModels
{
    public class TracNghiemViewModel
    {
        public Ma_bai_tap_doc BaiTap { get; set; }
        public List<Cau_hoi_bai_tap_doc> CauHoiList { get; set; }
        public string TotalTime { get; set; }
        public Dictionary<string, string> BaiDocs { get; set; } 
        

        public Ma_bai_tap_nge BaiTapNge { get; set; }
        public List<Cau_hoi_bai_tap_nge> CauHoiNgeList { get; set; }

        public Ma_bai_thi Baithi { get; set; }
        public List<Cau_hoi_bai_thi> CauHoiBaiThiList { get; set; }
    }
}
