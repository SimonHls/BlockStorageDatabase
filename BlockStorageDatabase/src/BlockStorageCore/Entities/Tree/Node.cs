namespace BlockStorageCore.Entities.Tree;

public abstract class Node {
    public uint BlockId { get; init; }
    public bool IsLeaf { get; init; }
    public uint keyCount { get; init; }
    public uint FreeStart { get; init; }
    public uint FreeEnd { get; init; }
    public uint UsedBytes { get; init; }

    public abstract void Insertkey();
    public abstract void Split();


}
