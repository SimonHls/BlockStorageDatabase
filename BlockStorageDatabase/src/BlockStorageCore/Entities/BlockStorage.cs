using BlockStorageCore.Interfaces;

namespace BlockStorageCore.Entities;
public class BlockStorage : IBlockStorage {
    public int BlockContentSize => throw new NotImplementedException();

    public int BlockHeaderSize => throw new NotImplementedException();

    public int BlockSize => throw new NotImplementedException();

    public IBlock CreateNew() {
        throw new NotImplementedException();
    }

    public IBlock Find(uint blockId) {
        throw new NotImplementedException();
    }
}
