namespace PicGramWebApp.Services.Observers
{
    public class PhotoActionEvent
    {
        public string? UserId { get; set; }
        public string ActionType { get; set; } = null!;
        public string? Details { get; set; }
    }
}