namespace PicGramWebApp.Models.ViewModels
{
    public class PackageUsageViewModel
    {
        public string Email { get; set; } = null!;
        public string PackageName { get; set; } = "None";
        public decimal PackagePrice { get; set; }

        public int UploadedPhotos { get; set; }
        public long UsedStorageBytes { get; set; }
        public int DownloadCount { get; set; }

        public int MaxUploadsPerMonth { get; set; }
        public long MaxStorageBytes { get; set; }
        public int MaxDownloadsPerMonth { get; set; }
    }
}