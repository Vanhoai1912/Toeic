using Microsoft.AspNetCore.Http;

namespace Toeic.Models.ViewModels
{
    public class CauhoiBThiVM
    {
        public IFormFile ExcelFile { get; set; }
        public string Tieu_de { get; set; }
        public int Id { get; set; }
        public string ExamType { get; set; }
        public List<Ma_bai_thi> Mabaithis { get; set; }
        public List<IFormFile> ImageFile { get; set; }
        public List<IFormFile> AudioFile { get; set; }
    }
}
