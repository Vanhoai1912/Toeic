using System.ComponentModel.DataAnnotations;

namespace ToeicWeb.Models
{
    public class Ma_bai_ngu_phap
    {
        [Key]
        public int Id { get; set; }

        public string Ten_bai { get; set; }

        public string? ImageUrl { get; set; }
    }
}
