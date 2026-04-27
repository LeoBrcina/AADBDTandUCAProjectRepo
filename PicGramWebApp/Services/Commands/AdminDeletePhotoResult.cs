namespace PicGramWebApp.Services.Commands
{
    public class AdminDeletePhotoResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static AdminDeletePhotoResult Ok()
        {
            return new AdminDeletePhotoResult
            {
                Success = true
            };
        }

        public static AdminDeletePhotoResult Fail(string message)
        {
            return new AdminDeletePhotoResult
            {
                Success = false,
                ErrorMessage = message
            };
        }
    }
}