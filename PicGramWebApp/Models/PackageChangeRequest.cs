using System.ComponentModel.DataAnnotations;

namespace PicGramWebApp.Models
{
    public class PackageChangeRequest
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }

        public int CurrentPackagePlanId { get; set; }
        public PackagePlan? CurrentPackagePlan { get; set; }

        public int RequestedPackagePlanId { get; set; }
        public PackagePlan? RequestedPackagePlan { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.Now;
        public DateTime EffectiveFrom { get; set; }

        public bool IsApplied { get; set; } = false;
    }
}