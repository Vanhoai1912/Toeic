using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toeic.Models
{
    public class UserAnswer
    {
        [Key]
        public int Id { get; set; }

        public int TestResultId { get; set; }  // Liên kết với kết quả bài thi (TestResult)
        [ForeignKey("TestResultId")]
        public TestResult TestResult { get; set; } // Điều hướng đến đối tượng TestResult

        public int CauHoiId { get; set; }  // Liên kết với câu hỏi trong bảng Cau_hoi_bai_thi
        [ForeignKey("CauHoiId")]
        public Cau_hoi_bai_thi CauHoi { get; set; }

        public string Answer { get; set; }  // Câu trả lời của người dùng
        public bool IsCorrect { get; set; } // Câu trả lời có đúng không
    }

}
