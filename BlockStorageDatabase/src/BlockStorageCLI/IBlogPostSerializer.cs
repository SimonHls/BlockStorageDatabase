using BlockStorageCLI.Models;

namespace BlockStorageCLI.Interfaces {
    public interface IBlogPostSerializer {
        BlogPost Deserialize(byte[] bytes);
        byte[] Serialize(BlogPost post);
    }
}