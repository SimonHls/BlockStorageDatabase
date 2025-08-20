using BlockStorageCore.Entities;

namespace BlockStorageCoreTests.BlockTests;

/// <summary>
/// Tests for the block methods touching the block header,
/// ReadHeader() and WriteHeader()
/// </summary>
public class BlockHeaderTests : IDisposable {

    private readonly MemoryStream _stream;
    private readonly Block _block;
    private readonly long[] _initialHeaderData = { 1, 2, 3, 4, 5, 6 };

    public BlockHeaderTests() {
        _stream = new MemoryStream();
        using (var writer = new BinaryWriter(_stream, System.Text.Encoding.UTF8, leaveOpen: true)) {
            foreach (var value in _initialHeaderData) {
                writer.Write(value);
            }
        }
        _stream.Position = 0;

        _block = new Block(_stream, blockId: 0);
    }

    public void Dispose() {
        GC.SuppressFinalize(GetType());
        _stream.Dispose();
    }

    [Fact]
    public void GetHeader_ReturnsCorrectHeaderFromValidStream() {
        // == Act ==
        var headerVal = _block.GetHeader(1);

        // == Assert ==
        Assert.Equal(2, headerVal);
    }

    [Fact]
    public void GetHeader_ThrowsObjectDisposedException_WhenCalledOnDisposedObject() {
        // == Arrange ==
        _block.Dispose();

        // == Act ==
        Action act = () => _block.GetHeader(1); ;
        var ex = Record.Exception(act);

        // == Assert ==
        Assert.NotNull(ex);
        Assert.IsType<ObjectDisposedException>(ex);
    }

    [Fact]
    public void SetHeader_CanUpdateHeaderValueOnValidStream() {
        // == Act ==
        _block.SetHeader(1, 3); // change index 1 from 2 to 3
        var headerVal = _block.GetHeader(1);

        // == Assert ==
        Assert.Equal(3, headerVal);
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