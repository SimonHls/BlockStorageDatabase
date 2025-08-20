using BlockStorageCore.Constants;
using BlockStorageCore.Helpers;
using BlockStorageCore.Interfaces;

namespace BlockStorageCore.Entities;
public class Block : IBlock {

    public uint Id { get; set; }
    private Stream _stream;
    private readonly int _blockSize;

    private bool isDisposed = false;

    // variable to track if the block was changed after the intial read
    private bool pendingChanges = false;

    // We store header data in a cache, so we can get it faster on second reads
    public long?[] headerCache = new long?[BlockConstants.HeaderFieldCount];

    public Block(Stream stream, int blockSize, uint blockId) {
        _stream = stream;
        _blockSize = blockSize;
        Id = blockId;
    }

    public long GetHeader(int field) {
        // Todo: Headers could also be an enum, this would be way more readable

        if (isDisposed)
            throw new ObjectDisposedException(nameof(Block));

        if (field < 0 || field > BlockConstants.HeaderFieldCount)
            throw new ArgumentOutOfRangeException(nameof(field), "Could not get header, index out of range.");

        long positionInStream = _blockSize * Id + (long)field * BlockConstants.HeaderFieldSize;
        if (positionInStream > _stream.Length || positionInStream < 0)
            throw new ArgumentOutOfRangeException(nameof(field), "Could not get header, calculated header position is outside the stream,");

        var buffer = new byte[BlockConstants.HeaderFieldSize];
        _stream.Seek(positionInStream, (int)SeekOrigin.Begin);
        _stream.Read(buffer, 0, BlockConstants.HeaderFieldSize);
        return BufferHelper.ReadBufferInt64(buffer, 0);
    }

    public void SetHeader(int field, long value) {
        if (isDisposed)
            throw new ObjectDisposedException(nameof(Block));

        if (field < 0 || field > BlockConstants.HeaderFieldCount)
            throw new ArgumentOutOfRangeException(nameof(field), "Could not get header, index out of range.");

        long positionInStream = _blockSize * Id + (long)field * BlockConstants.HeaderFieldSize;
        if (positionInStream > _stream.Length || positionInStream < 0)
            throw new ArgumentOutOfRangeException(nameof(field), "Could not get header, calculated header position is outside the stream,");

        var valueBuffer = new byte[ByteLengths.LongLen];

        BufferHelper.WriteBuffer(value, valueBuffer, (int)SeekOrigin.Begin);

        _stream.Seek(positionInStream, 0);
        _stream.Write(valueBuffer, 0, valueBuffer.Length);
    }

    public void Read(byte[] dst, int dstOffset, int srcOffset, int count) {
        throw new NotImplementedException();
    }

    public void Write(byte[] src, int srcOffset, int dstOffset, int count) {
        throw new NotImplementedException();
    }

    public void Dispose() {
        if (pendingChanges) {
            _stream.Flush();
        }
        if (!isDisposed) {
            _stream.Dispose();
            isDisposed = true;
        }
    }
}
