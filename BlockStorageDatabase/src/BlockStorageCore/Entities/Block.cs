using BlockStorageCore.Enums;
using BlockStorageCore.Interfaces;
using BlockStorageCore.structs;

namespace BlockStorageCore.Entities;

public class Block : IBlock {


    public ulong Id { get; init; }
    public bool IsDirty { get; set; }

    private BlockHeader _header;
    private byte[] _data;
    private readonly int _blockSize;

    public Block(ulong Id, int blockSize) {
        _header = new BlockHeader();
        _header.Initialize(Id);
        _blockSize = blockSize;
        _data = new byte[blockSize - BlockHeader.SizeInBytes];
        IsDirty = false;
        this.Id = Id;
    }

    public ulong GetHeader(HeaderField field) {
        return _header.GetHeader(field);
    }

    public void Read(byte[] dst, int dstOffset, int srcOffset, int count) {
        if (dst == null) {
            throw new ArgumentNullException(nameof(dst));
        }
        if (srcOffset < 0 || count < 0 || srcOffset + count > _data.Length) {
            throw new ArgumentOutOfRangeException(nameof(srcOffset), "Read would go beyond the bounds of the block's data.");
        }
        if (dstOffset < 0 || dstOffset + count > dst.Length) {
            throw new ArgumentOutOfRangeException(nameof(dstOffset), "Read would go beyond the bounds of the destination buffer.");
        }
        Buffer.BlockCopy(
            src: _data,
            srcOffset,
            dst,
            dstOffset,
            count
        );
    }

    public void SetHeader(HeaderField field, ulong value) {
        _header.SetHeader(field, value);
        IsDirty = true;
    }

    public void Write(byte[] src, int srcOffset, int dstOffset, int count) {
        if (src == null) {
            throw new ArgumentNullException(nameof(src));
        }
        if (dstOffset < 0 || count < 0 || dstOffset + count > _data.Length) {
            throw new ArgumentOutOfRangeException(nameof(srcOffset), "Write would go beyond the bounds of the block's data.");
        }
        if (srcOffset < 0 || srcOffset + count > src.Length) {
            throw new ArgumentOutOfRangeException(nameof(dstOffset), "Write would go beyond the bounds of the source buffer.");
        }

        Buffer.BlockCopy(
            src,
            srcOffset,
            _data,
            dstOffset,
            count
        );

        IsDirty = true;
    }

}
