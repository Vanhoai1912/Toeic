namespace ToeicWeb.Models.ViewModels
{
    public class TracNghiemViewModel
    {
        public Ma_bai_tap_doc BaiTap { get; set; }
        public List<Cau_hoi_bai_tap_doc> CauHoiList { get; set; }
        public string TotalTime { get; set; }
        public Dictionary<string, string> BaiDocs { get; set; } // Sử dụng Dictionary để ánh xạ bài đọc với nội dung
        

        public Ma_bai_tap_nge BaiTapNge { get; set; }
        public List<Cau_hoi_bai_tap_nge> CauHoiNgeList { get; set; }
    }
}
