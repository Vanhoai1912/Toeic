using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toeic.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        public string Receiver { get; set; }

        [Required]
        public string MessageText { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool IsFromAI { get; set; }
    }

}
