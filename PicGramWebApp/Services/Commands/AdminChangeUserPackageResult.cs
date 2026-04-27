namespace PicGramWebApp.Services.Commands
{
    public class AdminChangeUserPackageResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static AdminChangeUserPackageResult Ok()
        {
            return new AdminChangeUserPackageResult
            {
                Success = true
            };
        }

        public static AdminChangeUserPackageResult Fail(string message)
        {
            return new AdminChangeUserPackageResult
            {
                Success = false,
                ErrorMessage = message
            };
        }
    }
}