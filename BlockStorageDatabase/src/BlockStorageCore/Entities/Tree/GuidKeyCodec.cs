using BlockStorageCore.Constants;
using BlockStorageCore.Interfaces.Tree;

namespace BlockStorageCore.Entities.Tree;

internal class GuidKeyCodec : IKeyCodec<Guid> {
    public int EncodedLength(Guid key) => ByteLengths.GuidLen;

    public int CompareEncoded(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b) {
        // Lexicographic compare of 16 bytes (unsigned)
        for (int i = 0; i < 16; i++) {
            byte ai = a[i], bi = b[i];
            if (ai != bi)
                return ai < bi ? -1 : 1;
        }
        return 0;

    }

    public Guid Decode(ReadOnlySpan<byte> src) {
        if (src.Length < 16) throw new ArgumentException("src too small");
        return new Guid(src);
    }

    public int Encode(Guid key, Span<byte> dst) {
        if (dst.Length < 16)
            throw new ArgumentException("dst too small");
        if (!key.TryWriteBytes(dst))
            throw new Exception("Could not convert Guid to byte array");
        return ByteLengths.GuidLen;
    }

}
