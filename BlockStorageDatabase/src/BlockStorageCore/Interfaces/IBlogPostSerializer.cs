using BlockStorageCore.Models;

namespace BlockStorageCore.Interfaces {
    public interface IBlogPostSerializer {
        BlogPost Deserialize(byte[] bytes);
        byte[] Serialize(BlogPost post);
    }
}