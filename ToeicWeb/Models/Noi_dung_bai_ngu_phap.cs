using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ToeicWeb.Models
{
    public class Noi_dung_bai_ngu_phap
    {
        [Key]
        public int Id { get; set; }

        public string Noi_dung { get; set; }

        public string? ImageUrl { get; set; }

        public int Ma_bai_ngu_phapId { get; set; }
        [ForeignKey("Ma_bai_ngu_phapId")]
        public Ma_bai_ngu_phap Ma_bai_ngu_phap { get; set; }
    }
}
