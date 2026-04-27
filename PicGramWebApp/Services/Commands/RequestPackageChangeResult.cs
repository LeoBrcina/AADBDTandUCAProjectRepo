namespace PicGramWebApp.Services.Commands
{
    public class RequestPackageChangeResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static RequestPackageChangeResult Ok()
        {
            return new RequestPackageChangeResult
            {
                Success = true
            };
        }

        public static RequestPackageChangeResult Fail(string message)
        {
            return new RequestPackageChangeResult
            {
                Success = false,
                ErrorMessage = message
            };
        }
    }
}