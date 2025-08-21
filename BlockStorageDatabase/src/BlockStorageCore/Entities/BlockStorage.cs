using BlockStorageCore.Constants;
using BlockStorageCore.Interfaces;

namespace BlockStorageCore.Entities;

public class BlockStorage : IBlockStorage {

    private readonly Stream _stream;
    private readonly Dictionary<uint, Block> blockCache = new Dictionary<uint, Block>();


    public BlockStorage(Stream stream) {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        _stream = stream;
    }


    public IBlock CreateNew() {
        var streamLen = _stream.Length;

        if (streamLen % BlockConstants.TotalSize != 0)
            throw new DataMisalignedException("The stream length must be a multiple of the block length! Stream length: " + streamLen);

        var newBlockId = (uint)Math.Ceiling((double)streamLen / BlockConstants.TotalSize);

        _stream.SetLength(streamLen + BlockConstants.TotalSize);
        _stream.Flush();

        var block = new Block(_stream, newBlockId);
        OnBlockInit(block);

        return block;
    }

    public IBlock Find(uint blockId) {
        // Check the cache
        if (blockCache.ContainsKey(blockId))
            return blockCache[blockId];

        var blockPosition = blockId * BlockConstants.TotalSize;
        if (blockId < 0 || blockPosition + BlockConstants.TotalSize > _stream.Length)
            throw new ArgumentOutOfRangeException(nameof(blockId), "The provided block id is outside the stream range.");

        // Get block from stream
        Block newBlock = new Block(_stream, blockId);
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
