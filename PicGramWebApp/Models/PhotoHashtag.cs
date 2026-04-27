namespace PicGramWebApp.Models
{
    public class PhotoHashtag
    {
        public int PhotoId { get; set; }
        public Photo? Photo { get; set; }

        public int HashtagId { get; set; }
        public Hashtag? Hashtag { get; set; }
    }
}