using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toeic.Models.ViewModels
{
    public class ArticleVM
    {
        public string Ten_bai { get; set; }
        public int Id { get; set; }
        public string Noi_dung { get; set; }
        public string Description { get; set; }

        public List<Article> Articles { get; set; }
        public Article Article { get; set; }

        // Thêm trường để hiển thị đường dẫn ảnh
        public string ImageUrl { get; set; }

        // Chỉ dùng khi upload ảnh mới
        public IFormFile ImageFileArticle { get; set; }
    }

}
