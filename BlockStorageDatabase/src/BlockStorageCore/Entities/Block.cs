using BlockStorageCore.Constants;
using BlockStorageCore.Helpers;
using BlockStorageCore.Interfaces;

namespace BlockStorageCore.Entities;
public class Block : IBlock {

    // TODO: Enhancement -> When initializing the block from the BlockStorage, we can instantly read the first few bytes from the block
    // and pass them as an argument to the Block instance. Any reads and writes can then first access this section.
    // Reading and writing from header would not require accessing the stream, and sometimes the section might even contain all the data we need.
    // We then write this section back into the stream when disposing of the block.

    public uint Id { get; set; }
    private Stream _stream;

    private bool _isDisposed = false;

    // event which notifies subscribers that the block was disposed
    public event EventHandler? DisposedEvent;

    // variable to track if the block was changed after the intial read
    private bool _pendingChanges = false;

    // We store header data in a cache, so we can get it faster on second reads
    private long?[] _headerCache;

    private readonly long _blockDataSectionStart;
    private readonly IBlockStorage _storage;

    public Block(Stream stream, uint blockId, IBlockStorage storage) {
        _stream = stream;
        Id = blockId;
        _storage = storage;
        _blockDataSectionStart = Id * _storage.BlockSize + _storage.BlockHeaderSize;
        _headerCache = new long?[_storage.BlockHeaderSize / 8]; // 8 bytes per header
    }

    public long GetHeader(uint field) {
        // Todo: Headers could also be an enum, this would be way more readable

        if (_isDisposed)
            throw new ObjectDisposedException(nameof(Block));

        // Check cache first
        long? valueFromCache = _headerCache[field];
        if (field < _headerCache.Length && valueFromCache != null)
            return (long)valueFromCache;

        // Not in cache -> read value from stream
        long positionInStream = _storage.BlockSize * Id + field * _storage.BlockHeaderFieldSize;
        if (positionInStream > _stream.Length || positionInStream < 0)
            throw new ArgumentOutOfRangeException(nameof(field), "Could not get header, calculated header position is outside the stream,");

        var buffer = new byte[_storage.BlockHeaderSize];
        _stream.Seek(positionInStream, (int)SeekOrigin.Begin);
        _stream.Read(buffer, 0, _storage.BlockHeaderSize);

        long valueFromStream = BufferHelper.ReadBufferInt64(buffer, 0);

        _headerCache[field] = valueFromStream;

        return valueFromStream;
    }

    public void SetHeader(uint field, long value) {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(Block));

        long positionInStream = _storage.BlockSize * Id + field * _storage.BlockHeaderFieldSize;
        if (positionInStream > _stream.Length || positionInStream < 0)
            throw new ArgumentOutOfRangeException(nameof(field), "Could not get header, calculated header position is outside the stream,");

        var valueBuffer = new byte[ByteLengths.LongLen];

        BufferHelper.WriteBuffer(value, valueBuffer, (int)SeekOrigin.Begin);

        _headerCache[field] = value;

        _stream.Seek(positionInStream, 0);
        _stream.Write(valueBuffer, 0, valueBuffer.Length);
        _pendingChanges = true;
    }

    public void Read(byte[] dst, int dstOffset, int srcOffset, int count) {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(Block));

        if (dst == null)
            throw new ArgumentNullException(nameof(dst));

        if (dstOffset < 0 || srcOffset < 0 || count < 0)
            throw new ArgumentOutOfRangeException("Offsets and count cannot be negative.");

        if (dstOffset + count > dst.Length)
            throw new ArgumentException("Read operation would overflow the destination buffer.");

        if (srcOffset + count > _storage.BlockContentSize)
            throw new ArgumentException("Read operation would exceed the block's data boundaries.");

        long absoluteReadPosition = _blockDataSectionStart + srcOffset;

        _stream.Seek(absoluteReadPosition, origin: 0);
        _stream.Read(buffer: dst, offset: dstOffset, count);
    }

    public void Write(byte[] src, int srcOffset, int dstOffset, int count) {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(Block));

        if (src == null)
            throw new ArgumentNullException(nameof(src));

        if (dstOffset < 0 || srcOffset < 0 || count < 0)
            throw new ArgumentOutOfRangeException("Offsets and count cannot be negative.");

        if (dstOffset + count > _storage.BlockContentSize)
            throw new ArgumentException("Write operation would overflow the blocks data section size.");

        if (srcOffset + count > src.Length)
            throw new ArgumentException("Write operation would exceed the source data boundaries");

        long absoluteWritePosition = _blockDataSectionStart + dstOffset;

        _stream.Seek(absoluteWritePosition, origin: 0);
        _stream.Write(src, srcOffset, count);

        _pendingChanges = true;
    }

    public void Dispose() {
        if (!_isDisposed) {
            if (_pendingChanges) {
                _stream.Flush();
            }
            _isDisposed = true;
            _pendingChanges = false;

            // Fire event to tell subscribers: block is disposed
            if (DisposedEvent != null) {
                DisposedEvent(this, EventArgs.Empty);
            }

            GC.SuppressFinalize(this);
        }
    }
}
