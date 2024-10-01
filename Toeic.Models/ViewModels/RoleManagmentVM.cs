using Microsoft.AspNetCore.Mvc.Rendering;
using Toeic.Models;

namespace Toeic.Models.ViewModels
{
    public class RoleManagmentVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
