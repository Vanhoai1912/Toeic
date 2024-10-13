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
       
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public int SkippedQuestions { get; set; }
        public DateTime CompletionDate { get; set; }


        public int MabaithiId { get; set; } // ID của bài thi
        [ForeignKey("MabaithiId")]
        public Ma_bai_thi Mabaithi { get; set; } // Điều hướng đến đối tượng Mabaithi

        // Khóa ngoại liên kết với ApplicationUser
        public string ApplicationUserId { get; set; }  // ID của người dùng

        // Điều hướng đến đối tượng ApplicationUser
        public ApplicationUser ApplicationUser { get; set; }

        public TimeSpan Duration { get; set; } // Thêm trường này để lưu thời gian làm bài

                                               // Khai báo danh sách câu trả lời của người dùng
        public ICollection<UserAnswer> UserAnswers { get; set; }  // Liên kết với các câu trả lời

    }
}
