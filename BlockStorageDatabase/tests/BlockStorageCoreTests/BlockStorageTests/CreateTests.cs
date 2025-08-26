using BlockStorageCore.Entities;
using BlockStorageCore.Enums;

namespace BlockStorageCoreTests.BlockStorageTests;

public class CreateTests {
    [Fact]
    public void Create_ExtendsStreamAndReturnsNewBlock() {
        // == Arrange ==
        var stream = new MemoryStream();
        stream.SetLength(0);
        stream.Flush();
        stream.Position = 0;
        var initialStreamLength = stream.Length;

        var storage = new BlockStorage(stream, blockSize: 4096, Enum.GetNames(typeof(DataBlockHeader)).Length * 8);
        var exectedStreamLength = 4096;

        // == Act ==
        var newBlock = storage.CreateNew();

        // == Assert ==

        // sanity check: initial stream is length 0
        Assert.Equal(0, initialStreamLength);
        Assert.Equal(exectedStreamLength, stream.Length);
        Assert.IsType<Block>(newBlock);
    }
}
