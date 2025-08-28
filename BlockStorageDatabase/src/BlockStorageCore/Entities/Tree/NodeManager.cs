using BlockStorageCore.Interfaces.Storage;

namespace BlockStorageCore.Entities.Tree;

// Allocates new nodes
// Loads nodes by block id
// Save nodes
// Manages Meta Block (Root Block Id, height)
public class NodeManager {

    private readonly IBlockStorage _blockStorage;

    // Indices into BlockStorage header (each is one 8-byte long)
    const int kNodeType = 0; // 0 = Internal, 1 = Leaf
    const int kParentBlockId = 1; // 0 if root (or unknown)
    const int kKeyCount = 2; // number of slots/keys currently in the node

    const int kFreeStart = 3; // offset (bytes) to start of free space (== end of slot dir)
    const int kFreeEnd = 4; // offset (bytes) to end of free space (== start of cells region)
    const int kUsedBytes = 5; // sum of cell byte lengths currently in use (for compaction decision)

    // Node-type specific
    const int kSpecialA = 6; // Leaf: RightSiblingBlockId  | Internal: LeftmostChildBlockId
    const int kSpecialB = 7; // Leaf: PrevSiblingBlockId (optional, can be 0) | Internal: reserved (0)

    // (Optional but nice to have — you can keep them at 0 initially)
    const int kReserved8 = 8; // reserved for future use
    const int kReserved9 = 9; // reserved for future use



    public NodeManager(IBlockStorage blockStorage) {
        _blockStorage = blockStorage;
    }


}
