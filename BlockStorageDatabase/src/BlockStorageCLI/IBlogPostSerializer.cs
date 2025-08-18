using BlockStorageCore.Entities;

namespace BlockStorageCLI {
    public interface IBlogPostSerializer {
        BlogPost Deserialize(byte[] bytes);
        byte[] Serialize(BlogPost post);
    }
}