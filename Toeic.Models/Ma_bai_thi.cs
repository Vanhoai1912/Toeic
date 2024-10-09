using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toeic.Models
{
    public class Ma_bai_thi
    {
        [Key]
        public int Id { get; set; }
        public string Tieu_de { get; set; }
        public string ExamType { get; set; }
        public string ExcelFilePath { get; set; }
        public string? ImageFolderPath { get; set; }
        public string? AudioFolderPath { get; set; }
    }
}
