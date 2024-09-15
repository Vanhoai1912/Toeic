using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToeicWeb.Models
{
    public class Noi_dung_bai_tu_vung
    {
        [Key]
        public int Id { get; set; } 

        public string Tu_vung { get; set; }

        public string Nghia { get; set; }

        public string Audio { get; set; }

        public string ImageUrl { get; set; }

        public string Phien_am { get; set; }

        public string Vi_du { get; set; }

        public string Tu_dong_nghia { get; set; }

        public string Tu_trai_nghia { get; set; }

        public int Ma_bai_tu_vungId { get; set; }
        [ForeignKey("Ma_bai_tu_vungId")]
        public Ma_bai_tu_vung Ma_bai_tu_vung { get; set; }

    }
}
