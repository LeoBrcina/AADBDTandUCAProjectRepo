using System.ComponentModel.DataAnnotations;

namespace PicGramWebApp.Models
{
    public class Hashtag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public ICollection<PhotoHashtag>? PhotoHashtags { get; set; }
    }
}