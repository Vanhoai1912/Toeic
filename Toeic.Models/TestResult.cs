using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toeic.Models
{
    public class TestResult
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public int SkippedQuestions { get; set; }
        public DateTime CompletionDate { get; set; }

        // Khóa ngoại liên kết với ApplicationUser
        public string ApplicationUserId { get; set; }  // ID của người dùng

        // Điều hướng đến đối tượng ApplicationUser
        public ApplicationUser ApplicationUser { get; set; }
    }


}
