using BlockStorageCore.Entities;

namespace BlockStorageCLI {
    public class BlogPostSerializer {
        /// <summary>
        /// Serializes a BlogPost into a byte array byte[].
        /// </summary>
        /// <param name="post">The post to serialize</param>
        /// <returns>A byte[] with the serialized post</returns>
        public byte[] Serialize(BlogPost post) {

            // Calculate all dynamic sized props
            var titleBytes = System.Text.Encoding.UTF8.GetByteCount(post.Title);
            var contentBytes = System.Text.Encoding.UTF8.GetByteCount(post.Content);

            // Add hard-coded length for static length properties
            var postByteArray = new byte[
                16 +            // Guid
                4 +             // AuthorId
                8 +             // PublishedUtc
                4 +             // Bytes to store the title length
                titleBytes +    // Bytes for Title
                4 +             // Bytes to store content length
                contentBytes    // Bytes for Content
            ];

            // Buffer.BlockCopy and put data into specific parts of an array of primitive types

            // Id
            Buffer.BlockCopy(
                src: post.Id.ToByteArray(), // Guid has a ToByteArray method 👍
                srcOffset: 0,
                dst: postByteArray,
                dstOffset: 0,
                count: 16 // Guid is 16 bytes
            );

            // AuthorId
            Buffer.BlockCopy(
                src: LittleEnd,
                srcOffset: 0,
                dst: postByteArray,
                dstOffset: 0,
                count: 16 // Guid is 16 bytes
            );
        }
    }
}
