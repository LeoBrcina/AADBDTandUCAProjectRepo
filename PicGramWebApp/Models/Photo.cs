using System.ComponentModel.DataAnnotations;

namespace PicGramWebApp.Models
{
    public class Photo
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; } = null!;

        [Required]
        public string FilePath { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public DateTime UploadedAt { get; set; } = DateTime.Now;
        public long FileSize { get; set; }
        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }
        public ICollection<PhotoHashtag>? PhotoHashtags { get; set; }
    }
}