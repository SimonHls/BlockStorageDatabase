using BlockStorageCore.Entities;
using BlockStorageCoreTests.Helpers;

namespace BlockStorageCoreTests.BlockTests;

/// <summary>
/// Tests for the block methods touching the block header,
/// ReadHeader() and WriteHeader()
/// </summary>
public class BlockHeaderTests : IDisposable {

    private readonly MemoryStream _stream;
    private readonly Block _block;
    private readonly long[] _initialHeaderData = { 1L, 2L, 3L, 4L, 5L, 6L };

    public BlockHeaderTests() {
        _stream = new MemoryStream();
        using (var writer = new BinaryWriter(_stream, System.Text.Encoding.UTF8, leaveOpen: true)) {
            foreach (var value in _initialHeaderData) {
                writer.Write(value);
            }
        }
        _stream.Position = 0;

        var mockStorage = BlockStorageMocks.GetMockStorage();

        _block = new Block(_stream, blockId: 0, mockStorage.Object);
    }

    public void Dispose() {
        GC.SuppressFinalize(GetType());
        _stream.Dispose();
    }

    [Fact]
    public void GetHeader_ReturnsCorrectHeaderFromValidStream() {
        // == Act ==
        var headerVal0 = _block.GetHeader(0);
        var headerVal1 = _block.GetHeader(1);
        var headerVal2 = _block.GetHeader(2);
        var headerVal3 = _block.GetHeader(3);
        var headerVal4 = _block.GetHeader(4);

        // == Assert ==
        Assert.Equal(1, headerVal0);
        Assert.Equal(2, headerVal1);
        Assert.Equal(3, headerVal2);
        Assert.Equal(4, headerVal3);
        Assert.Equal(5, headerVal4);
    }

    [Fact]
    public void GetHeader_ThrowsObjectDisposedException_WhenCalledOnDisposedObject() {
        // == Arrange ==
        _block.Dispose();

        // == Act ==
        Action act = () => _block.GetHeader(1);
        var ex = Record.Exception(act);

        // == Assert ==
        Assert.NotNull(ex);
        Assert.IsType<ObjectDisposedException>(ex);
    }

    [Fact]
    public void SetHeader_CanUpdateHeaderValueOnValidStream() {
        // == Act ==
        uint targetHeader1 = 1;
        uint targetHeader2 = 4;
        _block.SetHeader(targetHeader1, 3); // change index 1 from 2 to 3
        _block.SetHeader(targetHeader2, 8); // change index 4 from 5 to 8

        var headerVal1 = _block.GetHeader(targetHeader1);
        var headerVal2 = _block.GetHeader(targetHeader2);

        // == Assert ==
        Assert.Equal(3, headerVal1);
        Assert.Equal(8, headerVal2);
    }

    [Fact]
    public void SetHeader_ThrowsObjectDisposedException_WhenCalledOnDisposedObject() {
        // == Arrange ==
        _block.Dispose();

        // == Act ==
        Action act = () => _block.SetHeader(1, 3);
        var ex = Record.Exception(act);

        // == Assert ==
        Assert.NotNull(ex);
        Assert.IsType<ObjectDisposedException>(ex);
    }
}