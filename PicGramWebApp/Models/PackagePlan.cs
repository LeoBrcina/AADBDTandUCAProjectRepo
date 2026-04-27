namespace PicGramWebApp.Models
{
    public class PackagePlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }

        public int MaxUploadsPerMonth { get; set; }
        public long MaxStorageBytes { get; set; }
        public int MaxDownloadsPerMonth { get; set; }

        public ICollection<ApplicationUser>? Users { get; set; }
    }
}