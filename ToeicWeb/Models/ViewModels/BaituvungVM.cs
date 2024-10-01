namespace ToeicWeb.Models.ViewModels
{
    public class BaituvungVM
    {
        public IFormFile ExcelFile { get; set; }
        public string Ten_bai { get; set; }
        public int Id { get; set; }
        public List<Ma_bai_tu_vung> Mabaituvungs { get; set; }
        public Ma_bai_tu_vung Mabaituvung { get; set; }
        public List<IFormFile> ImageFile { get; set; } = new List<IFormFile>();  // Khởi tạo để tránh null
        public List<IFormFile> AudioFile { get; set; } = new List<IFormFile>();  // Khởi tạo để tránh null
        public List<Noi_dung_bai_tu_vung> Noidungbaituvungs { get; set; }
        public IFormFile ImageFileMavoca { get; set; }
    }
}
