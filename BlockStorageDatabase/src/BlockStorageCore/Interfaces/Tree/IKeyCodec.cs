namespace BlockStorageCore.Interfaces.Tree;

public interface IKeyCodec<TKey> {
    // Returns the exact encoded byte length for this key instance.
    int EncodedLength(TKey key);

    /// <summary>
    /// Encode the key into dst; returns bytes written.
    /// </summary>
    /// <param name="key">the key to encode</param>
    /// <param name="dst">the destination buffer after encoding</param>
    /// <returns>the length in bytes of the written key</returns>
    int Encode(TKey key, Span<byte> dst);

    // Compare two already-encoded keys lexicographically (byte-wise).
    int CompareEncoded(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b);

    // Optional: convert encoded bytes back to the key object.
    TKey Decode(ReadOnlySpan<byte> src);
}
