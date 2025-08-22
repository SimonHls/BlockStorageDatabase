using BlockStorageCore.Entities;
using BlockStorageCoreTests.Helpers;
using Moq;

namespace BlockStorageCoreTests.BlockTests;

public class BlockCachingTests {

    [Fact]
    public void GetHeader_CachesValue_AfterFirstRead() {
        // == Arrange ==

        var mockStream = new Mock<Stream>();
        long expectedValue = 99L;


        mockStream
            .Setup(s => s.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
            .Callback((byte[] buffer, int offset, int count) => {
                // When Read is called, we write fake data
                var bytes = BitConverter.GetBytes(expectedValue);
                bytes.CopyTo(buffer, offset);
            });
        mockStream.Setup(s => s.Length).Returns(4096);
        mockStream.Setup(s => s.CanSeek).Returns(true);

        var mockStorage = BlockStorageMocks.GetMockStorage();

        var block = new Block(mockStream.Object, blockId: 0, mockStorage.Object);

        // == Act ==

        // Call 1 -> This should go to the stream to get the value.
        var value1 = block.GetHeader(1);

        // Call 2 -> This should hit the cache
        var value2 = block.GetHeader(1);

        // == Assert ==

        // Ensure we got the correct value on both calls.
        Assert.Equal(expectedValue, value1);
        Assert.Equal(expectedValue, value2);

        // Check that we read from the stream only once, so the other read hit the cache
        mockStream.Verify(s => s.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
    }

    [Fact]
    public void SetHeader_PopulatesCache_AndAvoidsStreamReadOnGet() {
        // == Arrange ==
        var mockStream = new Mock<Stream>();
        mockStream.Setup(s => s.Length).Returns(4096);
        mockStream.Setup(s => s.CanSeek).Returns(true);
        mockStream.Setup(s => s.CanWrite).Returns(true);

        var mockStorage = BlockStorageMocks.GetMockStorage();

        var block = new Block(mockStream.Object, blockId: 0, mockStorage.Object);

        long newValue = 123L;

        // == Act ==
        // SetHeader should write to the stream and update the cache.
        block.SetHeader(1, newValue);

        // GetHeader should find the value in the cache
        var result = block.GetHeader(1);

        // == Assert ==
        Assert.Equal(newValue, result);

        // Verify that SetHeader wrote to the stream
        mockStream.Verify(s => s.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());

        // Verify that GetHeader didn't need to read from the stream
        mockStream.Verify(s => s.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
    }
}
