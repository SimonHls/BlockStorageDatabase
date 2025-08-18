using BlockStorageCLI;
using BlockStorageCore.Entities;

var serializer = new BlogPostSerializer();


var testPost = new BlogPost(
    Id: Guid.NewGuid(),
    AuthorId: 1,
    PublishedUtc: DateTime.UtcNow,
    Title: "A cool blog post",
    Content: "The following example shows how to write to a file asynchronously. This code runs in a WPF app that has a TextBlock named UserInput and a button hooked up to a Click event handler that is named Button_Click. The file path needs to be changed to a file that exists on the computer."
);

var bs = serializer.Serialize(testPost);

var path = @"C:\Users\simon\source\repos\BlockStorageDatabase\BlockStorageDatabase\src\BlockStorageCLI\db\test.blockDb";

//using (FileStream fs = File.Create(path)) {
//    fs.Write(bs, 0, bs.Length);
//}

byte[] buffer = null;
using (FileStream stm = new System.IO.FileStream(path,
           FileMode.Open, FileAccess.Read, FileShare.None)) {
    buffer = new byte[stm.Length];
    stm.Read(buffer, 0, Convert.ToInt32(stm.Length));
}

var content = serializer.Deserialize(buffer);

Console.WriteLine(content);

//foreach (var item in bs) {
//    Console.Write(item);
//}

//BlogPost deserializedPost = serializer.Deserialize(bs);
//Console.WriteLine(deserializedPost.ToString());
//Console.WriteLine();
//Console.WriteLine(deserializedPost == testPost);