namespace BlockStorageCore.Interfaces.Tree;

public interface INodeLayout<TKey> {
    bool IsFixedCellSize { get; }
    int FixedCellSize { get; } // if fixed

    int MeasureLeafCellSize(TKey key, int ptrCount);
    int MeasureInternalCellSize(TKey key);

    // Write a single cell into dst (starting at its beginning)
    int WriteLeafCell(Span dst, TKey key, ReadOnlySpan recordIds);
    int WriteInternalCell(Span dst, TKey key, uint rightChildId);

    // Read views (no allocations)
    ReadOnlySpan ReadKey(ReadOnlySpan cell);
    ReadOnlySpan ReadLeafPtrs(ReadOnlySpan cell);
    uint ReadRightChild(ReadOnlySpan cell);
}