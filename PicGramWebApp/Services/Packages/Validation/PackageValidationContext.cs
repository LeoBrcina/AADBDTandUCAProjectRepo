using PicGramWebApp.Models;

namespace PicGramWebApp.Services.Packages.Validation
{
    public class PackageValidationContext
    {
        public ApplicationUser User { get; set; } = null!;
        public long NewFileSize { get; set; }
        public PackagePlan PackagePlan { get; set; } = null!;
    }
}