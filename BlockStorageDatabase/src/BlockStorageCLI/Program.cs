using BlockStorageCLI;
using BlockStorageCore.Entities;

var serializer = new BlogPostSerializer();


var testPost = new BlogPost(
    Id: Guid.NewGuid(),
    AuthorId: 1,
    PublishedUtc: DateTime.UtcNow,
    Title: "Title",
    Content: "content"
);

var bs = serializer.Serialize(testPost);

foreach (var item in bs) {
    Console.Write(item);
}

BlogPost deserializedPost = serializer.Deserialize(bs);
Console.WriteLine(deserializedPost.ToString());
Console.WriteLine();
Console.WriteLine(deserializedPost == testPost);