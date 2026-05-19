using PicGramWebApp.Services.Functional;

namespace PicGramWebApp.Tests;

public class PhotoFunctionalHelpersTests
{
    [Fact]
    public void NormalizeHashtag_RemovesLeadingHashAndTrimsWhitespace()
    {
        var result = PhotoFunctionalHelpers.NormalizeHashtag("  #cars  ");

        Assert.Equal("cars", result);
    }

    [Fact]
    public void NormalizeHashtag_ConvertsValueToLowercase()
    {
        var result = PhotoFunctionalHelpers.NormalizeHashtag("  #FormulaOne  ");

        Assert.Equal("formulaone", result);
    }

    [Fact]
    public void NormalizeHashtag_WhenInputIsEmpty_ReturnsNull()
    {
        var result = PhotoFunctionalHelpers.NormalizeHashtag("   ");

        Assert.Null(result);
    }

    [Fact]
    public void NormalizeAuthor_TrimsWhitespace()
    {
        var result = PhotoFunctionalHelpers.NormalizeAuthor("  user@example.com  ");

        Assert.Equal("user@example.com", result);
    }

    [Fact]
    public void NormalizeOutputFormat_WhenUnknownFormat_ReturnsJpg()
    {
        var result = PhotoFunctionalHelpers.NormalizeOutputFormat("tiff");

        Assert.Equal("jpg", result);
    }

    [Fact]
    public void NormalizeOutputFormat_WhenJpegFormat_ReturnsJpg()
    {
        var result = PhotoFunctionalHelpers.NormalizeOutputFormat("jpeg");

        Assert.Equal("jpg", result);
    }

    [Fact]
    public void CalculateStorageUsagePercentage_ReturnsRoundedPercentage()
    {
        var result = PhotoFunctionalHelpers.CalculateStorageUsagePercentage(2500, 10000);

        Assert.Equal(25.00, result);
    }

    [Fact]
    public void CalculateRemainingStorageBytes_WhenUsedExceedsMax_ReturnsZero()
    {
        var result = PhotoFunctionalHelpers.CalculateRemainingStorageBytes(12000, 10000);

        Assert.Equal(0, result);
    }
}