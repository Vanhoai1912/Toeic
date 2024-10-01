using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toeic.Models.ViewModels
{
    public class BainguphapVM
    {
        public string Ten_bai { get; set; }
        public int Id { get; set; }
        public string Noi_dung { get; set; }
        public List<Ma_bai_ngu_phap> Mabainguphaps { get; set; }
        public Ma_bai_ngu_phap Mabainguphap { get; set; }
        public List<Noi_dung_bai_ngu_phap> Noidungbainguphaps { get; set; }
        public IFormFile ImageFileGrammar { get; set; }
    }
}
