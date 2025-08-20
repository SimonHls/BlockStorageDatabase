using BlockStorageCore.Constants;
using BlockStorageCore.Helpers;
using BlockStorageCore.Interfaces;

namespace BlockStorageCore.Entities;
public class Block : IBlock {

    public uint Id { get; set; }
    private Stream _stream;
    private readonly int _blockSize;

    // variable to track if the block was changed
    private bool pendingChanges = false;

    // The block data
    public byte[] Header { get; set; }
    public byte[] Data { get; set; }

    public Block(Stream stream, int blockSize, uint blockId) {
        _stream = stream;
        _blockSize = blockSize;
        Data = new byte[blockSize];
        Header = new byte[BlockConstants.HeaderSize];
        Id = blockId;
    }

    public void Dispose() {
        if (pendingChanges)
            // We dump the block into the stack
            throw new NotImplementedException();
        else
            // we dispose the block
            throw new NotImplementedException();
    }

    public long GetHeader(int field) {
        // Todo: Headers could also be an enum, this would be way more readable
        var buffer = new byte[BlockConstants.HeaderFieldSize];
        long positionInStream = _blockSize * Id + (long)field * BlockConstants.HeaderFieldSize;
        _stream.Seek(positionInStream, 0);
        _stream.Read(buffer, 0, BlockConstants.HeaderFieldSize);
        return BufferHelper.ReadBufferInt64(buffer, 0);
    }

    public void Read(byte[] dst, int dstOffset, int srcOffset, int count) {
        throw new NotImplementedException();
    }

    public void SetHeader(int field, long value) {
        throw new NotImplementedException();
    }

    public void Write(byte[] src, int srcOffset, int dstOffset, int count) {
        throw new NotImplementedException();
    }
}
