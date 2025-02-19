using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toeic.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        public string Ten_bai { get; set; }

        public string Noi_dung { get; set; }

        public string? ImageUrl { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7); // Giờ Việt Nam (UTC+7)
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7); // Giờ Việt Nam (UTC+7)

    }
}
