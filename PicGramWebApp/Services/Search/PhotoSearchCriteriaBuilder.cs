namespace PicGramWebApp.Services.Search
{
    // Builder pattern: constructs a flexible search criteria object step by step,
    // avoiding large constructors and making optional filters easier to combine.
    public class PhotoSearchCriteriaBuilder
    {
        private readonly PhotoSearchCriteria _criteria = new();

        public PhotoSearchCriteriaBuilder WithHashtag(string? hashtag)
        {
            if (!string.IsNullOrWhiteSpace(hashtag))
                _criteria.Hashtag = hashtag.Trim();

            return this;
        }

        public PhotoSearchCriteriaBuilder WithAuthor(string? author)
        {
            if (!string.IsNullOrWhiteSpace(author))
                _criteria.Author = author.Trim();

            return this;
        }

        public PhotoSearchCriteriaBuilder WithFromDate(DateTime? fromDate)
        {
            _criteria.FromDate = fromDate;
            return this;
        }

        public PhotoSearchCriteriaBuilder WithToDate(DateTime? toDate)
        {
            _criteria.ToDate = toDate;
            return this;
        }

        public PhotoSearchCriteriaBuilder WithMinSize(long? minSize)
        {
            _criteria.MinSize = minSize;
            return this;
        }

        public PhotoSearchCriteriaBuilder WithMaxSize(long? maxSize)
        {
            _criteria.MaxSize = maxSize;
            return this;
        }

        public PhotoSearchCriteria Build()
        {
            return _criteria;
        }
    }
}