using BlockStorageCLI;
using BlockStorageCore.Models;

namespace BlogStorageCLITests;

public class BlogPostSerializerTests {
    [Fact]
    public void BlogPostSerializer_SerializedAndDeserializedObjectsAreTheSame() {
        // Arrange
        var serializer = new BlogPostSerializer();

        var testPost = new BlogPost(
            Id: Guid.NewGuid(),
            AuthorId: 1,
            PublishedUtc: DateTime.UtcNow,
            Title: "Title",
            Content: "content"
        );

        // Act
        var bs = serializer.Serialize(testPost);
        BlogPost deserializedPost = serializer.Deserialize(bs);

        // Assert
        Assert.Equal(deserializedPost, testPost);
    }
}