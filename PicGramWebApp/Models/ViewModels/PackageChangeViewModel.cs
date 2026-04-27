using Microsoft.AspNetCore.Mvc.Rendering;

namespace PicGramWebApp.Models.ViewModels
{
    public class PackageChangeViewModel
    {
        public string CurrentPackageName { get; set; } = null!;
        public int RequestedPackagePlanId { get; set; }
        public List<SelectListItem> AvailablePackages { get; set; } = new();
        public string? Message { get; set; }
    }
}