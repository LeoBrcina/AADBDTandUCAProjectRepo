using System.ComponentModel.DataAnnotations;

namespace PicGramWebApp.Models
{
    public class ActionLog
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public string ActionType { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? Details { get; set; }
    }
}