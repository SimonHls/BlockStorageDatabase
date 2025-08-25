using BlockStorageCore.Constants;
using BlockStorageCore.Helpers;
using BlockStorageCore.Interfaces;

namespace BlockStorageCore.Entities;

public class RecordStorage : IRecordStorage {

    // Important: The Id of a record is the same as the Id of the first block.
    // Therefore: Record Ids are not sequential, and are 1 indexed, since the 0 record is reserved to store deleted records.

    const int MaxRecordSize = 4194304; // 4MB

    // Field positions in the header
    const int kNextBlockId = 0;
    const int kRecordLength = 1;
    const int kBlockContentLength = 2;
    const int kPreviousBlockId = 3;
    const int kIsDeleted = 4;


    private readonly IBlockStorage _blockStorage;

    public RecordStorage(IBlockStorage blockStorage) {
        _blockStorage = blockStorage;
    }

    public virtual uint Create() {
        using (var firstBlock = AllocateBlock()) {
            return firstBlock.Id;
        }
    }

    public uint Create(byte[] data) {
        if (data == null) {
            throw new ArgumentException();
        }
        // pass the data to the method taking a data generator, id doesn't matter.
        return Create(recordId => data);
    }

    public uint Create(Func<uint, byte[]> dataGenerator) {
        // The dataGenerator function we pass allows us to do something with the allocated record id before writing the data.
        // For example, we can build up our index for the new record.
        if (dataGenerator == null) {
            throw new ArgumentException();
        }

        var firstBlock = AllocateBlock();
        using (firstBlock) {
            var newRecordId = firstBlock.Id;

            // We give the generator our records id, and get data in return
            var data = dataGenerator(newRecordId);

            var dataWritten = 0;
            var totalDataToBeWritten = data.Length;
            firstBlock.SetHeader(kRecordLength, totalDataToBeWritten);

            if (totalDataToBeWritten == 0) {
                return newRecordId;
            }

            // Now we write the data, getting new blocks until all data is written.
            IBlock currentBlock = firstBlock;
            while (dataWritten < totalDataToBeWritten) {
                IBlock? nextBlock = null;

                using (currentBlock) {
                    var writeBatchLength = Math.Min(totalDataToBeWritten - dataWritten, _blockStorage.BlockContentSize);
                    currentBlock.Write(data, dataWritten, dstOffset: 0, writeBatchLength);
                    currentBlock.SetHeader(kBlockContentLength, writeBatchLength);
                    dataWritten += writeBatchLength;

                    // Still data left to write? get new block and continue
                    if (dataWritten < totalDataToBeWritten) {
                        bool success = false;
                        try {
                            nextBlock = AllocateBlock();
                            currentBlock.SetHeader(kNextBlockId, nextBlock.Id);
                            nextBlock.SetHeader(kPreviousBlockId, currentBlock.Id);
                            success = true;
                        }
                        finally {
                            if (!success && nextBlock != null) {
                                nextBlock.Dispose();
                                nextBlock = null;
                            }
                        }
                    } else {
                        // All data was written
                        break;
                    }
                }
                if (nextBlock != null)
                    currentBlock = nextBlock;
            }

            return newRecordId;
        }
    }

    public void Delete(uint recordId) {
        using (var firstBlock = _blockStorage.Find(recordId)) {
            IBlock? currentBlock = firstBlock;
            if (currentBlock == null) return;

            while (true) {
                IBlock? nextBlock = null;

                using (currentBlock) {

                    MarkAsFree(currentBlock);

                    var nextBlockId = (uint)currentBlock.GetHeader(kNextBlockId);
                    if (nextBlockId == 0) break;
                    else {
                        nextBlock = _blockStorage.Find(nextBlockId);
                        if (nextBlock == null) {
                            throw new InvalidDataException("Block not found by id: " + nextBlockId);
                        }
                    }
                }

                currentBlock = nextBlock;
            }
        }
    }

    /// <summary>
    /// Adds a block to the free list and sets the deleted flag on the block.
    /// This does not reset any other header data, since the reset is done on allocation anyways.
    /// </summary>
    /// <param name="block">The block to mark as free</param>
    /// <exception cref="Exception">Is thrown when ne free block record was found</exception>
    private void MarkAsFree(IBlock block) {
        IBlock? lastBlock = null;
        IBlock? secondLastBlock = null;

        // We don't need secondLastBlock, but the method returns it.
        GetLastTwoBlockOfFreeBlocksRecord(out lastBlock, out secondLastBlock);

        using (lastBlock)
        using (secondLastBlock) {

            if (lastBlock == null)
                throw new Exception("Could not add record to free list, free list not found.");

            // Add block id to free list
            var lastBlockContentLength = lastBlock.GetHeader(kBlockContentLength);
            var spaceLeft = _blockStorage.BlockContentSize - lastBlockContentLength;
            if (spaceLeft >= ByteLengths.Int32Len) {
                AddUint32ToBlockData(lastBlock, block.Id);
                lastBlock.SetHeader(kBlockContentLength, lastBlockContentLength + ByteLengths.UInt32Len);
            } else {

                using (var newBlock = _blockStorage.CreateNew()) {
                    newBlock.SetHeader(kPreviousBlockId, lastBlock.Id);
                    lastBlock.SetHeader(kNextBlockId, newBlock.Id);

                    AddUint32ToBlockData(newBlock, block.Id);
                    newBlock.SetHeader(kBlockContentLength, ByteLengths.UInt32Len);
                }
            }
            block.SetHeader(kIsDeleted, 1L);

        }
    }

    public byte[]? Find(uint recordId) {

        // Get first block
        using (var firstBlock = _blockStorage.Find(recordId)) {
            if (firstBlock == null) return null;

            if (firstBlock.GetHeader(kIsDeleted) == 1L) return null;

            // Block is not the first block
            if (firstBlock.GetHeader(kPreviousBlockId) != 0L) return null;

            var totalRecordSize = firstBlock.GetHeader(kRecordLength);
            if (totalRecordSize > MaxRecordSize) {
                throw new NotSupportedException("Unexpected record length: " + totalRecordSize);
            }

            var recordData = new byte[totalRecordSize];
            var bytesRead = 0;

            IBlock? currentBlock = firstBlock;

            while (true) {
                uint nextBlockId;

                using (currentBlock) {
                    var currentBlockContentLength = (int)currentBlock.GetHeader(kBlockContentLength);
                    currentBlock.Read(recordData, bytesRead, 0, currentBlockContentLength);
                    bytesRead += currentBlockContentLength;

                    nextBlockId = (uint)currentBlock.GetHeader(kNextBlockId);
                    if (nextBlockId == 0)
                        return recordData;
                }

                currentBlock = _blockStorage.Find(nextBlockId);
                if (currentBlock == null) {
                    throw new InvalidDataException("Could not find block: " + nextBlockId);
                }
            }
        }
    }

    public void Update(uint recordId, byte[] data) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Allocates a block, by either finding a free (deleted) one, or creating a new one
    /// </summary>
    /// <returns>A free block</returns>
    private IBlock AllocateBlock() {
        uint freeBlockId;
        IBlock? newBlock;

        // See if we have a free block
        if (TryFindFreeBlock(out freeBlockId)) {
            newBlock = _blockStorage.Find(freeBlockId);
            if (newBlock == null) {
                throw new InvalidDataException("Block not found by id: " + freeBlockId);
            }
            // Clear out headers
            newBlock.SetHeader(kBlockContentLength, 0L);
            newBlock.SetHeader(kNextBlockId, 0L);
            newBlock.SetHeader(kPreviousBlockId, 0L);
            newBlock.SetHeader(kRecordLength, 0L);
            newBlock.SetHeader(kIsDeleted, 0L);
        } else {
            newBlock = _blockStorage.CreateNew();
            if (newBlock == null) {
                throw new InvalidDataException("Failed to create a new block.");
            }
        }
        return newBlock;
    }

    private bool TryFindFreeBlock(out uint blockId) {
        blockId = 0;

        IBlock? lastBlock;
        IBlock? secondLastBlock;
        GetLastTwoBlockOfFreeBlocksRecord(out lastBlock, out secondLastBlock); // innocent

        // using to make sure we dispose the blocks
        using (lastBlock)
        using (secondLastBlock) {
            if (lastBlock == null)
                throw new Exception("Could not find #0 record, last block in record is null.");

            // check if we have data in the last block
            var lastBlockContentLen = lastBlock.GetHeader(kBlockContentLength);
            if (lastBlockContentLen == 0) {

                // We have an empty last block, and it is the only block, so we have no free blocks
                if (secondLastBlock == null) {
                    return false;
                }

                // We get our free block id from the second last block
                blockId = ReadLastUint32FromBlockData(secondLastBlock);

                // Replace the free block with the id of our empty last block.
                // This effectively deletes it.
                // We don't need to change the content length header, since we removed and then added items of the same length
                AddUint32ToBlockData(secondLastBlock, lastBlock.Id, replace: true);
                secondLastBlock.SetHeader(kNextBlockId, 0L);
                lastBlock.SetHeader(kIsDeleted, 1L);
                lastBlock.SetHeader(kPreviousBlockId, 0L);

                return true;
            }
            // Last block is not empty, so we just get the free block from there
            else {
                blockId = ReadLastUint32FromBlockData(lastBlock);
                lastBlock.SetHeader(kBlockContentLength, lastBlock.GetHeader(kBlockContentLength) - ByteLengths.UInt32Len);
                return true;
            }
        }
    }

    private uint ReadLastUint32FromBlockData(IBlock block) {
        var resultBuffer = new byte[ByteLengths.UInt32Len];
        var contentLength = block.GetHeader(kBlockContentLength);
        if ((contentLength % ByteLengths.UInt32Len) != 0) {
            throw new DataMisalignedException("Block content length not %4: " + contentLength);
        }

        if (contentLength == 0) {
            throw new InvalidDataException("Trying to dequeue UInt32 from an empty block");
        }

        block.Read(dst: resultBuffer, dstOffset: 0, srcOffset: (int)contentLength - ByteLengths.UInt32Len, count: ByteLengths.UInt32Len);
        return LeByteConverter.GetUInt32(resultBuffer);
    }

    /// <summary>
    /// Appends or replaces a UInt32 to a blocks data section. 
    /// Appending extends the content length, replacing changes the last four bytes of the current content to the new value.
    /// </summary>
    /// <param name="block">The target block</param>
    /// <param name="value">The value to append or replace to</param>
    /// <param name="replace">If true, we change the last four bytes of the current content. If false, we extend the content (default)</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="DataMisalignedException"></exception>
    private void AddUint32ToBlockData(IBlock block, uint value, bool replace = false) {
        var contentLength = block.GetHeader(kBlockContentLength);
        if (replace) contentLength = contentLength - ByteLengths.UInt32Len;

        if (contentLength + ByteLengths.UInt32Len > _blockStorage.BlockContentSize) {
            throw new ArgumentOutOfRangeException(nameof(value), "Cannot append uint32 to block content, block is full");
        }

        if ((contentLength % ByteLengths.UInt32Len) != 0) {
            throw new DataMisalignedException("Block content length not %4: " + contentLength);
        }

        block.Write(src: LeByteConverter.GetBytes(value), srcOffset: 0, dstOffset: (int)contentLength, count: ByteLengths.UInt32Len);
    }

    private void GetLastTwoBlockOfFreeBlocksRecord(out IBlock? lastBlock, out IBlock? secondLastBlock) {
        lastBlock = null;
        secondLastBlock = null;

        var recordBlocks = FindBlocksForRecord(0); // Innocent

        try {
            if (recordBlocks == null) {
                throw new Exception("No blocks found for record 0");
            }

            lastBlock = recordBlocks[recordBlocks.Count - 1];
            if (recordBlocks.Count > 1)
                secondLastBlock = recordBlocks[recordBlocks.Count - 2];

        }
        finally {
            if (recordBlocks != null)
                // Dispose all unused blocks
                foreach (var b in recordBlocks) {
                    if ((lastBlock == null || b != lastBlock)
                             && (secondLastBlock == null || b != secondLastBlock)) {
                        b.Dispose();
                    }
                }
        }
    }

    /// <summary>
    /// Finds all blocks for a specific record
    /// </summary>
    /// <param name="recordId">The id of the record (and the first block)</param>
    /// <returns>A list of IBlock that make up a record</returns>
    private List<IBlock> FindBlocksForRecord(uint recordId) {

        var blocks = new List<IBlock>();
        var success = false;

        try {
            var currentBlockId = recordId;

            do {
                // Grab next block
                var block = _blockStorage.Find(currentBlockId);
                if (block == null) {
                    // Special case: if block #0 never created, then attempt to create it
                    if (currentBlockId == 0) {
                        block = _blockStorage.CreateNew();
                    } else {
                        throw new Exception("Block not found by id: " + currentBlockId);
                    }
                }
                blocks.Add(block);

                if (1L == block.GetHeader(kIsDeleted)) {
                    throw new InvalidDataException("Block not found: " + currentBlockId);
                }

                // Move next
                currentBlockId = (uint)block.GetHeader(kNextBlockId);
            } while (currentBlockId != 0);

            success = true;
            return blocks;
        }
        finally {
            // Something bad happened, dispose everything
            if (!success) {
                foreach (Block b in blocks)
                    b.Dispose();
            }
        }
    }
}
