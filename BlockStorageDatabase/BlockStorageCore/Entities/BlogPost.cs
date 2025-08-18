namespace BlockStorageCore.Entities {
    public record BlogPost(
        Guid Id, // 16 bytes
        int AuthorId, // 4 bytes
        DateTime PublishedUtc, // 8 bytes
        string Title, // variable bytes
        string Content // variable bytes
    );
}
