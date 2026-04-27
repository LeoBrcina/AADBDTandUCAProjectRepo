namespace PicGramWebApp.Services.Packages
{
    public class PackageLimitResult
    {
        public bool IsAllowed { get; set; }
        public string? ErrorMessage { get; set; }

        public static PackageLimitResult Allowed() => new PackageLimitResult { IsAllowed = true };

        public static PackageLimitResult Denied(string message) =>
            new PackageLimitResult
            {
                IsAllowed = false,
                ErrorMessage = message
            };
    }
}