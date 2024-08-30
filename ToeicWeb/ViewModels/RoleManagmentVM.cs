using Microsoft.AspNetCore.Mvc.Rendering;
using ToeicWeb.Models;

namespace ToeicWeb.ViewModels
{
    public class RoleManagmentVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
