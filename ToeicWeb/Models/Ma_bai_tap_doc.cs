using System.ComponentModel.DataAnnotations;

namespace ToeicWeb.Models
{
    public class Ma_bai_tap_doc
    {
        [Key]
        public int Id { get; set; }

        public string Part {  get; set; }
    }
}
