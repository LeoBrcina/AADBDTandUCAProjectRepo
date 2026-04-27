namespace PicGramWebApp.Models.ViewModels
{
    public class UserStatisticsViewModel
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PackageName { get; set; } = "None";
        public int PhotoCount { get; set; }
        public int ActionCount { get; set; }
        public DateTime? LastActionAt { get; set; }
    }
}