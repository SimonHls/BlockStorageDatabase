using BlockStorageCore.Constants;
using BlockStorageCore.Entities;

namespace BlockStorageCoreTests.BlockStorageTests;

public class BlockStorageCreateTests {
    [Fact]
    public void Create_ExtendsStreamAndReturnsNewBlock() {
        // == Arrange ==
        var stream = new MemoryStream();
        stream.SetLength(0);
        stream.Flush();
        stream.Position = 0;

        var exectedStreamLength = BlockConstants.TotalSize;

        var storage = new BlockStorage(stream);

        // == Act ==
        var newBlock = storage.CreateNew();

        // == Assert ==
        Assert.Equal(exectedStreamLength, stream.Length);
        Assert.IsType<Block>(newBlock);
    }
}
