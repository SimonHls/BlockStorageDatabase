namespace BlockStorageCore.Interfaces;

public interface IBlockStorage {
    /// <summary>
    /// Find a block by its id
    /// </summary>
    IBlock Find(uint blockId);

    /// <summary>
    /// Allocate new block, extend the length of underlying storage
    /// </summary>
    IBlock CreateNew();
}