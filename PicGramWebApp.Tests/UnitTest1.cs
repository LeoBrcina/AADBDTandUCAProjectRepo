using PicGramWebApp.Services.Search;

namespace PicGramWebApp.Tests;

public class PhotoSearchCriteriaBuilderTests
{
    [Fact]
    public void Build_WithAllFilters_ReturnsCriteriaWithExpectedValues()
    {
        // Arrange
        var fromDate = new DateTime(2026, 3, 1);
        var toDate = new DateTime(2026, 3, 31);

        // Act
        var criteria = new PhotoSearchCriteriaBuilder()
            .WithHashtag("cars")
            .WithAuthor("test1@gmail.com")
            .WithFromDate(fromDate)
            .WithToDate(toDate)
            .WithMinSize(1000)
            .WithMaxSize(5000)
            .Build();

        // Assert
        Assert.Equal("cars", criteria.Hashtag);
        Assert.Equal("test1@gmail.com", criteria.Author);
        Assert.Equal(fromDate, criteria.FromDate);
        Assert.Equal(toDate, criteria.ToDate);
        Assert.Equal(1000, criteria.MinSize);
        Assert.Equal(5000, criteria.MaxSize);
    }

    [Fact]
    public void Build_WithWhitespaceTextFilters_TrimsHashtagAndAuthor()
    {
        // Act
        var criteria = new PhotoSearchCriteriaBuilder()
            .WithHashtag("  travel  ")
            .WithAuthor("  author@example.com  ")
            .Build();

        // Assert
        Assert.Equal("travel", criteria.Hashtag);
        Assert.Equal("author@example.com", criteria.Author);
    }

    [Fact]
    public void Build_WithEmptyTextFilters_LeavesHashtagAndAuthorNull()
    {
        // Act
        var criteria = new PhotoSearchCriteriaBuilder()
            .WithHashtag("   ")
            .WithAuthor("")
            .Build();

        // Assert
        Assert.Null(criteria.Hashtag);
        Assert.Null(criteria.Author);
    }
}