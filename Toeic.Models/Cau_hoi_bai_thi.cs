using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toeic.Models
{
    public class Cau_hoi_bai_thi
    {
        [Key]
        public int Id { get; set; }
        public string QuestionType { get; set; }

        public int Thu_tu_cau { get; set; }

        public string? Audio { get; set; }

        public string? Image { get; set; }

        public string? Cau_hoi { get; set; }

        public string Dap_an_1 { get; set; }

        public string Dap_an_2 { get; set; }

        public string Dap_an_3 { get; set; }

        public string Dap_an_4 { get; set; }

        public string Dap_an_dung { get; set; }

        public string? Transcript { get; set; }

        public string Giai_thich { get; set; }
        public string? Giai_thich_bai_doc { get; set; }

        public int Ma_bai_thiId { get; set; }
        [ForeignKey("Ma_bai_thiId")]
        public Ma_bai_thi Ma_bai_thi { get; set; }
    }
}
