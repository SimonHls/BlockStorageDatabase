using BlockStorageCore.Entities;

namespace BlockStorageCoreTests.BlockTests;

public class BlockReadWriteTests {

    [Fact]
    public void Read_CanReadFromValidStream() {
        // == Arrange ==

        long[] blockData = { 1, 2, 3, 4, 5, 6, 7, 8, 9 }; // First 6 values are headers so our data is 7, 8, 9

        long expectedValue1 = 7;
        long expectedValue2 = 8;

        var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true)) {
            foreach (var value in blockData) {
                writer.Write(value);
            }
        }
        stream.Position = 0;

        Block block = new Block(stream, 0);

        // == Act ==
        var resultBuffer = new byte[8]; // 8 bytes for a long
        // srcOffset = 0 should read first 8 bytes in data section, so 7
        block.Read(resultBuffer, 0, srcOffset: 0, count: 8);
        long result1 = BitConverter.ToInt64(resultBuffer);

        // srcOffset = 8 should read from 8 bytes into data section, so 8
        block.Read(resultBuffer, 0, srcOffset: 8, count: 8);
        long result2 = BitConverter.ToInt64(resultBuffer);

        // == Assert ==
        Assert.Equal(expectedValue1, result1);
        Assert.Equal(expectedValue2, result2);
    }

    [Fact]
    public void Write_WritesDataToValidStream() {
        // == Arrange =

        long[] blockData = { 1, 2, 3, 4, 5, 6 }; // Stream only has header data

        var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true)) {
            foreach (var value in blockData) {
                writer.Write(value);
            }
        }
        stream.Position = 0;

        Block block = new Block(stream, 0);

        long expectedValue = 10;

        // 8 byte test data, enough for a long
        var testData = new byte[8];
        testData = BitConverter.GetBytes(expectedValue);

        // == Act ==

        // Write the test data to the beginning of the blocks data section
        block.Write(testData, srcOffset: 0, dstOffset: 0, count: 8);

        // Read it back from the block
        var readBuffer = new byte[8];
        block.Read(readBuffer, dstOffset: 0, srcOffset: 0, count: 8);
        var readValue = BitConverter.ToInt64(readBuffer);

        // == Assert ==

        Assert.Equal(expectedValue, readValue);
    }
}
