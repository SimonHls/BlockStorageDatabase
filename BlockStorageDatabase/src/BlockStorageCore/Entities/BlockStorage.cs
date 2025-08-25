using BlockStorageCore.Interfaces;

namespace BlockStorageCore.Entities;

public class BlockStorage : IBlockStorage {

    private readonly Stream _stream;
    private readonly Dictionary<uint, Block> blockCache = new Dictionary<uint, Block>();

    public int BlockContentSize { get; init; }

    public int BlockHeaderSize { get; init; }

    public int BlockHeaderFieldSize { get; init; } // long 

    public int BlockSize { get; init; }

    public BlockStorage(Stream stream, int blockSize, int blockHeaderSize) {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        if (blockSize < 0)
            throw new ArgumentOutOfRangeException(nameof(blockSize), "Block size must not be negative");
        if (blockHeaderSize > blockSize / 2)
            throw new ArgumentOutOfRangeException(nameof(blockHeaderSize), "Block header cannot be greater than 50% of total block size");
        // TODO: Remove magic number
        if (blockHeaderSize < 32)
            throw new ArgumentOutOfRangeException(nameof(blockHeaderSize), "Block header must be at least 32 bytes long.");

        _stream = stream;
        this.BlockSize = blockSize;
        this.BlockHeaderSize = blockHeaderSize;
        this.BlockContentSize = blockSize - blockHeaderSize;
        this.BlockHeaderFieldSize = 8; // 8 bytes = long. This should probably be configured somewhere else.
    }


    public IBlock CreateNew() {
        var streamLen = _stream.Length;

        if (streamLen % BlockSize != 0)
            throw new DataMisalignedException("The stream length must be a multiple of the block length! Stream length: " + streamLen);

        var newBlockId = (uint)Math.Ceiling((double)streamLen / BlockSize);

        _stream.SetLength(streamLen + BlockSize);
        _stream.Flush();

        var block = new Block(_stream, newBlockId, this);
        OnBlockInit(block);

        return block;
    }

    public IBlock? Find(uint blockId) {
        // Check the cache
        if (blockCache.ContainsKey(blockId))
            return blockCache[blockId];

        var blockPosition = blockId * BlockSize;
        if ((blockPosition + BlockSize) > _stream.Length) {
            return null;
        }
        // Get block from stream
        Block newBlock = new Block(_stream, blockId, this);
        OnBlockInit(newBlock);

        return newBlock;
    }

    private void OnBlockInit(Block block) {
        // Add block to our cache
        blockCache[block.Id] = block;
        // Subscribe to the DisposeEvent
        block.DisposedEvent += HandleBlockDispose;
    }

    private void HandleBlockDispose(object? sender, EventArgs e) {
        if (sender is Block block) {
            // Unsubscribe from the event
            block.DisposedEvent -= HandleBlockDispose;

            // Remove the block from the cache
            blockCache.Remove(block.Id);
        }
    }
}
