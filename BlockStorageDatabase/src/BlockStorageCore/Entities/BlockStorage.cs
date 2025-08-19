using BlockStorageCore.Enums;
using BlockStorageCore.Interfaces;
using BlockStorageCore.structs;

namespace BlockStorageCore.Entities;
public class BlockStorage : IBlockStorage, IDisposable {

    public const int HeaderSizeInBytes = BlockHeader.SizeInBytes;
    public const int TotalBlockSize = 4096;

    public int BlockSize => TotalBlockSize;
    public int BlockHeaderSize => HeaderSizeInBytes;
    public int BlockContentSize => TotalBlockSize - HeaderSizeInBytes;

    private readonly FileStream _dbFileStream;
    private ulong _nextAvailableBlockId;

    private readonly Dictionary<ulong, Block> _blockCache;

    public BlockStorage(string filePath) {
        // Open the file for both reading and writing. Create it if it doesn't exist.
        _dbFileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        // Determine the next available block ID based on the current file size.
        _nextAvailableBlockId = (ulong)_dbFileStream.Length / TotalBlockSize;

        _blockCache = new Dictionary<ulong, Block>();
    }

    public IBlock CreateNew() {
        ulong newBlockId = _nextAvailableBlockId;
        _nextAvailableBlockId++;
        var newBlock = new Block(newBlockId, TotalBlockSize);
        newBlock.IsDirty = true;
        _blockCache[newBlockId] = newBlock;

        return newBlock;
    }

    public IBlock Find(uint blockId) {
        ulong id = blockId;

        // Check if the block is already in the cache.
        if (_blockCache.TryGetValue(id, out Block cachedBlock)) {
            return cachedBlock;
        }

        // If not cached, we read it from the disk.
        if (id >= _nextAvailableBlockId) {
            throw new ArgumentOutOfRangeException(nameof(blockId), "Block does not exist in the file.");
        }

        // Create a new block object and populate it from the stream.
        var block = new Block(id, TotalBlockSize);
        ReadFromStream(block, id);

        // Add the read block to the cache before returning it.
        _blockCache[id] = block;
        return block;
    }

    private void ReadFromStream(Block block, ulong blockId) {
        long position = (long)blockId * TotalBlockSize;
        _dbFileStream.Seek(position, SeekOrigin.Begin);

        using (var reader = new BinaryReader(_dbFileStream, System.Text.Encoding.UTF8, true)) {
            // Read all the header fields directly into the block's header struct.
            for (int i = 0; i < BlockHeader.FieldCount; i++) {
                block.SetHeader((HeaderField)i, reader.ReadUInt64());
            }

            // Read the data payload into the block's data array.
            reader.Read(block.Data, 0, block.Data.Length);
        }
        block.IsDirty = false;
    }

    private void WriteToStream(Block block) {
        long position = (long)block.Id * TotalBlockSize;
        _dbFileStream.Seek(position, SeekOrigin.Begin);

        using (var writer = new BinaryWriter(_dbFileStream, System.Text.Encoding.UTF8, true)) {
            // Write all header fields sequentially.
            for (int i = 0; i < BlockHeader.FieldCount; i++) {
                writer.Write(block.GetHeader((HeaderField)i));
            }
            // Write the data payload.
            writer.Write(block.Data);
        }
    }

    public void Flush() {
        foreach (var block in _blockCache.Values) {
            if (block.IsDirty) {
                // If a block is dirty, write its contents to the correct
                // position in the file stream.
                WriteToStream(block);
                block.IsDirty = false;
            }
        }
        _dbFileStream.Flush(true);
    }

    public void Dispose() {
        Flush();
        _dbFileStream.Close();
        _dbFileStream.Dispose();
    }
}
