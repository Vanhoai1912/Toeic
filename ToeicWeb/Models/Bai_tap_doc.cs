using System.ComponentModel.DataAnnotations;

namespace ToeicWeb.Models
{
    public class Bai_tap_doc
    {
        [Key]
        public int Id { get; set; }

        public string Part {  get; set; }
    }
}
