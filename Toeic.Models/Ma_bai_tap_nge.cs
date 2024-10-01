using System.ComponentModel.DataAnnotations;

namespace Toeic.Models
{
    public class Ma_bai_tap_nge
    {
        [Key]
        public int Id { get; set; }

        public int Part { get; set; }

        public string Tieu_de { get; set; }

        public string ExcelFilePath { get; set; }
        public string? ImageFolderPath { get; set; }
        public string AudioFolderPath { get; set; }

        public ICollection<Cau_hoi_bai_tap_nge> CauHoiBaiTapNges { get; set; }
    }
}
