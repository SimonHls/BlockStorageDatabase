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

        BlockStorage storage = new BlockStorage(stream, blockSize: 4096, blockHeaderSize: 48);

        // create a block we later find
        var createdBlock = storage.CreateNew(); // Id is  0, it's the first block

        // == Act ==
        var foundBlock = storage.Find(blockId: 0);

        // == Assert ==
        Assert.Equal(createdBlock, foundBlock);
    }

    [Fact]
    public void Find_ReturnsNullIfBlockIdDoesNotExist() {
        // == Arrange ==
        var stream = new MemoryStream();
        stream.SetLength(0);
        stream.Flush();
        stream.Position = 0;

        BlockStorage storage = new BlockStorage(stream, blockSize: 4096, 48);

        // == Act ==
        var block = storage.Find(blockId: 0); // There is no block in the stream


        // == Assert ==
        Assert.Null(block);
    }

}
