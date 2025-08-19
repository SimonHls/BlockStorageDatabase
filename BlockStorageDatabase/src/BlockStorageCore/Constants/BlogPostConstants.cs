namespace BlockStorageCore.Constants;

/// <summary>
/// Stores the length for all static length properties in the Post class,
/// in bytes.
/// </summary>
public static class BlogPostConstants {
    public const int GuidLength = ByteLengths.GuidLen;
    public const int AuthorIdLength = ByteLengths.Int32Len;
    public const int PublishedUtcLength = ByteLengths.DateTimeLen;
    public const int DynamicLengthIndicatorLength = ByteLengths.Int32Len;

    public const int MaxTitleLength = 8 * 1024; // That's probably too long
    public const int MaxContentLength = 512 * 1024; // 512kB seems enough
}