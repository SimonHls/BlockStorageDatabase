using BlockStorageCore.Entities;

namespace BlockStorageCoreTests;

public class BlockTests {
    [Fact]
    public void GetHeader_ReturnsCorrectHeaderFromValidStream() {

        // == Arrange ==

        long[] headerData = { 1, 2, 3, 4, 5, 6 };
        var stream = new MemoryStream();

        using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true)) {
            foreach (var value in headerData) {
                writer.Write(value);
            }
        }

        stream.Position = 0;

        var block = new Block(stream, blockSize: 4096, blockId: 0);

        // == Act ==
        var headerVal = block.GetHeader(1);

        // == Assert ==
        Assert.Equal(2, headerVal);

        stream.Dispose();

    }
}