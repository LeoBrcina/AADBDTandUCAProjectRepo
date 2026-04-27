namespace PicGramWebApp.Services.Search
{
    public class PhotoSearchCriteria
    {
        public string? Hashtag { get; set; }
        public string? Author { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? MinSize { get; set; }
        public long? MaxSize { get; set; }
    }
}