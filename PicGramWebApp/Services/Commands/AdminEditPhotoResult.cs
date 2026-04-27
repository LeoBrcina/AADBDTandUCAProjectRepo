namespace PicGramWebApp.Services.Commands
{
    public class AdminEditPhotoResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static AdminEditPhotoResult Ok()
        {
            return new AdminEditPhotoResult
            {
                Success = true
            };
        }

        public static AdminEditPhotoResult Fail(string message)
        {
            return new AdminEditPhotoResult
            {
                Success = false,
                ErrorMessage = message
            };
        }
    }
}