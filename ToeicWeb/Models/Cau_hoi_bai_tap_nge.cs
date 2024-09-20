using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ToeicWeb.Models
{
    public class Cau_hoi_bai_tap_nge
    {
        [Key]
        public int Id { get; set; }

        public int Thu_tu_cau { get; set; }

        public string? Audio { get; set; }

        public string? Image { get; set; }

        public string? Cau_hoi { get; set; }

        public string Dap_an_1 { get; set; }

        public string Dap_an_2 { get; set; }

        public string Dap_an_3 { get; set; }

        public string Dap_an_4 { get; set; }

        public string Dap_an_dung { get; set; }

        public string Giai_thich { get; set; }

        public int Ma_bai_tap_ngeId { get; set; }
        [ForeignKey("Ma_bai_tap_ngeId")]
        public Ma_bai_tap_nge Ma_bai_tap_nge { get; set; }

        // Fields for storing result-related information
        public string? UserAnswer { get; set; } // User's selected answer
        public bool IsCorrect { get; set; } // Whether the user's answer is correct
    }
}
