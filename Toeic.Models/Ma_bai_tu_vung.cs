using System.ComponentModel.DataAnnotations;

namespace Toeic.Models
{
    public class Ma_bai_tu_vung
    {
        [Key]
        public int Id { get; set; }

        public string Ten_bai {  get; set; }

        public string? ImageUrl { get; set; }

        public string? FilePath { get; set; }

        public ICollection<Noi_dung_bai_tu_vung> Noidungbaituvungs { get; set; }
    }
}
