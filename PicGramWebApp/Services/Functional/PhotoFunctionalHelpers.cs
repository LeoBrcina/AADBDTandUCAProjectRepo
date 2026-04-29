namespace PicGramWebApp.Services.Functional;

public static class PhotoFunctionalHelpers
{
    public static string? NormalizeHashtag(string? hashtag)
    {
        if (string.IsNullOrWhiteSpace(hashtag))
        {
            return null;
        }

        var trimmed = hashtag.Trim();

        if (trimmed.StartsWith("#"))
        {
            trimmed = trimmed[1..];
        }

        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    public static string? NormalizeAuthor(string? author)
    {
        return string.IsNullOrWhiteSpace(author) ? null : author.Trim();
    }

    public static string NormalizeOutputFormat(string? format)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            return "jpg";
        }

        var normalized = format.Trim().ToLowerInvariant();

        return normalized switch
        {
            "jpg" or "jpeg" => "jpg",
            "png" => "png",
            "bmp" => "bmp",
            _ => "jpg"
        };
    }

    public static double CalculateStorageUsagePercentage(long usedBytes, long maxBytes)
    {
        if (maxBytes <= 0)
        {
            return 0;
        }

        return Math.Round((double)usedBytes / maxBytes * 100, 2);
    }

    public static long CalculateRemainingStorageBytes(long usedBytes, long maxBytes)
    {
        var remaining = maxBytes - usedBytes;
        return remaining < 0 ? 0 : remaining;
    }
}