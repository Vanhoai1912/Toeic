using Microsoft.AspNetCore.Http;

namespace Toeic.Models.ViewModels
{
    public class CauhoiBTngeVM
    {
            public IFormFile ExcelFile { get; set; }
            public string Tieu_de { get; set; }
            public int Id { get; set; }
            public int Part { get; set; }
            public List<Ma_bai_tap_nge> Mabaitapnges { get; set; }
            public Ma_bai_tap_nge Mabaitapnge { get; set; }
            public List<IFormFile> ImageFile { get; set; } = new List<IFormFile>();  // Khởi tạo để tránh null
            public List<IFormFile> AudioFile { get; set; } = new List<IFormFile>();  // Khởi tạo để tránh null

    }
}
