using BlockStorageCore.Entities;

namespace BlockStorageCoreTests.BlockStorageTests;

public class BlockStorageFindTests {
    [Fact]
    public void Find_ReturnsBlockIfItAlreadyExists() {

        // == Arrange ==
        var stream = new MemoryStream();
        stream.SetLength(0);
        stream.Flush();
        stream.Position = 0;

        BlockStorage storage = new BlockStorage(stream);

        // create a block we later find
        var createdBlock = storage.CreateNew(); // Id is  0, it's the first block

        // == Act ==
        var foundBlock = storage.Find(blockId: 0);

        // == Assert ==
        Assert.Equal(createdBlock, foundBlock);
    }

    [Fact]
    public void Find_ThrowsErrorIfBlockIdDoesNotExist() {
        // == Arrange ==
        var stream = new MemoryStream();
        stream.SetLength(0);
        stream.Flush();
        stream.Position = 0;

        BlockStorage storage = new BlockStorage(stream);

        // == Act ==
        Action act = () => storage.Find(blockId: 0); // There is no block in the stream
        Exception ex = Record.Exception(act);

        // == Assert ==
        Assert.NotNull(ex);
        Assert.IsType<ArgumentOutOfRangeException>(ex);
    }

}
