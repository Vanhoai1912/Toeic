using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toeic.Models
{
    public class Cau_hoi_bai_tap_doc
    {
        [Key]
        public int Id { get; set; }

        public string? Cau_hoi { get; set; }

        public string Dap_an_dung { get; set; }

        public string Dap_an_1 { get; set; }

        public string Dap_an_2 { get; set; }

        public string Dap_an_3 { get; set; }

        public string Dap_an_4 { get; set; }

        public string Giai_thich { get; set; }
        public string? Image_bai_doc { get; set; }
        public string? Giai_thich_bai_doc { get; set; }

        public int Thu_tu_cau { get; set; }

        public int Ma_bai_tap_docId { get; set; }
        [ForeignKey("Ma_bai_tap_docId")]
        public Ma_bai_tap_doc Ma_bai_tap_doc { get; set; }

        public string? UserAnswer { get; set; }
        public bool IsCorrect { get; set; } 
    }
}
