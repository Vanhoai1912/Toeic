using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToeicWeb.Models
{
    public class Cau_hoi_bai_tap_doc
    {
        [Key]
        public int Id { get; set; }

        public string Cau_hoi { get; set; }

        public string Dap_an_dung {  get; set; }

        public string Dap_an_1 { get; set; }

        public string Dap_an_2 { get; set; }

        public string Dap_an_3 { get; set; }

        public string Dap_an_4 { get; set; }

        public string Giai_thich { get; set; }

        public string Photo_name { get; set; }

        public string So_thu_tu { get; set; }

        public int Bai_tap_docId { get; set; }
        [ForeignKey("Bai_tap_docId")]
        public Bai_tap_doc Bai_tap_doc { get; set; }

    }
}
