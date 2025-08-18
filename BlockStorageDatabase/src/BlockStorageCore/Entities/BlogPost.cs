namespace BlockStorageCore.Entities;
//public class BlogPost
//{
//    public Guid Id {get; set; } // 16 bytes
//    public int AuthorId { get; set;} // 4 bytes
//    public DateTime PublishedUtc { get; set; } // 8 bytes
//    public string Title { get; set; } // variable bytes
//    public string Content { get; set; } // variable bytes
//}

public record BlogPost(
    Guid Id,
    int AuthorId,
    DateTime PublishedUtc,
    string Title,
    string Content
    );
