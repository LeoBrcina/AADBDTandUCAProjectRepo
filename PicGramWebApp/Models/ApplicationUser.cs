using Microsoft.AspNetCore.Identity;

namespace PicGramWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? PackagePlanId { get; set; }
        public PackagePlan? PackagePlan { get; set; }
    }
}