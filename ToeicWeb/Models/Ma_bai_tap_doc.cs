using System.ComponentModel.DataAnnotations;

namespace ToeicWeb.Models
{
    public class Ma_bai_tap_doc
    {
        [Key]
        public int Id { get; set; }

        public int Part {  get; set; }

        public string Tieu_de { get; set; }

        public string? FilePath { get; set; }

        // Mối quan hệ với bảng Cau_hoi_bai_tap_doc
        public ICollection<Cau_hoi_bai_tap_doc> CauHoiBaiTapDocs { get; set; }
    }
}
