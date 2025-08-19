using BlockStorageCore.Constants;
using BlockStorageCore.Helpers;
using BlockStorageCore.Interfaces;
using BlockStorageCore.Models;
using System.Text;

namespace BlockStorageCLI {
    public class BlogPostSerializer : IBlogPostSerializer {
        /// <summary>
        /// Serializes a BlogPost into a byte array byte[].
        /// </summary>
        /// <param name="post">The post to serialize</param>
        /// <returns>A byte[] with the serialized post</returns>
        public byte[] Serialize(BlogPost post) {
            // current offset from byte[] origin
            var offset = 0;

            var titleLength = BufferHelper.GetByteLength(post.Title);
            var contentLength = BufferHelper.GetByteLength(post.Content);

            // Add hard-coded length for static length properties
            var postByteArray = new byte[
                BlogPostConstants.GuidLength +
                BlogPostConstants.AuthorIdLength +
                BlogPostConstants.PublishedUtcLength +
                BlogPostConstants.DynamicLengthIndicatorLength +
                titleLength +
                BlogPostConstants.DynamicLengthIndicatorLength +
                contentLength
            ];

            // Id
            Buffer.BlockCopy(
                src: post.Id.ToByteArray(), // Guid has a ToByteArray method 👍
                srcOffset: 0,
                dst: postByteArray,
                dstOffset: offset,
                count: BlogPostConstants.GuidLength
            );
            offset += BlogPostConstants.GuidLength;

            // AuthorId
            Buffer.BlockCopy(
                src:
                LeByteConverter.GetBytes(post.AuthorId),
                srcOffset: 0,
                dst: postByteArray,
                dstOffset: offset,
                count: BlogPostConstants.AuthorIdLength
            );
            offset += BlogPostConstants.AuthorIdLength;

            // PublishedUtc
            Buffer.BlockCopy(
                src: LeByteConverter.GetBytes(post.PublishedUtc.ToBinary()),
                srcOffset: 0,
                dst: postByteArray,
                dstOffset: offset,
                count: BlogPostConstants.PublishedUtcLength
            );
            offset += BlogPostConstants.PublishedUtcLength;

            // Title length indicator
            Buffer.BlockCopy(
                src: LeByteConverter.GetBytes(titleLength),
                srcOffset: 0,
                dst: postByteArray,
                dstOffset: offset,
                count: BlogPostConstants.DynamicLengthIndicatorLength
            );
            offset += BlogPostConstants.DynamicLengthIndicatorLength;

            // Title
            Buffer.BlockCopy(
                src: Encoding.UTF8.GetBytes(post.Title),
                srcOffset: 0,
                dst: postByteArray,
                dstOffset: offset,
                count: titleLength
            );
            offset += BufferHelper.GetByteLength(post.Title);

            // Content length indicator
            Buffer.BlockCopy(
                src: LeByteConverter.GetBytes(contentLength),
                srcOffset: 0,
                dst: postByteArray,
                dstOffset: offset,
                count: BlogPostConstants.DynamicLengthIndicatorLength
            );
            offset += BlogPostConstants.DynamicLengthIndicatorLength;

            // Title
            Buffer.BlockCopy(
                src: Encoding.UTF8.GetBytes(post.Content),
                srcOffset: 0,
                dst: postByteArray,
                dstOffset: offset,
                count: contentLength
            );

            return postByteArray;
        }

        public BlogPost Deserialize(byte[] bytes) {

            var offset = 0;

            // Read Id
            var postId = BufferHelper.ReadBufferGuid(bytes, offset);
            offset += BlogPostConstants.GuidLength;

            // Read AuthorId
            var postAuthorId = BufferHelper.ReadBufferInt32(bytes, offset);
            offset += BlogPostConstants.AuthorIdLength;

            // Read PublishedUtd
            var postPublishedUtc = DateTime.FromBinary(BufferHelper.ReadBufferInt64(bytes, offset));
            offset += BlogPostConstants.PublishedUtcLength;

            // Read Title
            var titleLength = BufferHelper.ReadBufferInt32(bytes, offset);
            if (titleLength is < 0 or > BlogPostConstants.MaxTitleLength) {
                throw new Exception("Invalid string length: " + titleLength);
            }
            offset += BlogPostConstants.DynamicLengthIndicatorLength;
            var postTitle = Encoding.UTF8.GetString(bytes, offset, titleLength);
            offset += titleLength;

            // Read Content
            var contentLength = BufferHelper.ReadBufferInt32(bytes, offset);
            if (contentLength is < 0 or > BlogPostConstants.MaxContentLength) {
                throw new Exception("Invalid string length: " + contentLength);
            }
            offset += BlogPostConstants.DynamicLengthIndicatorLength;
            var postContent = Encoding.UTF8.GetString(bytes, offset, contentLength);

            // Return constructed model
            return new BlogPost(
                postId,
                postAuthorId,
                postPublishedUtc,
                postTitle,
                postContent
            );
        }
    }
}
