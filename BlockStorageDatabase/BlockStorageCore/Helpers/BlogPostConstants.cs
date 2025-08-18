namespace BlockStorageCore.Helpers;

/// <summary>
/// Stores the length for all static length properties in the Post class,
/// in bytes.
/// </summary>
public static class BlogPostConstants
{
    public const int GuidLength = 16;
    public const int AuthorIdLength = 4;
    public const int PublishedUtcLength = 8;
    public const int DynamicLengthIndicatorLength = 4;
    
    public const int MaxTitleLength = 8*1024; // That's probably too long
    public const int MaxContentLength = 512 * 1024; // 512kB seems enough

    public static int GetStringByteLength(string s)
    {
        return System.Text.Encoding.UTF8.GetByteCount(s);
    }
}