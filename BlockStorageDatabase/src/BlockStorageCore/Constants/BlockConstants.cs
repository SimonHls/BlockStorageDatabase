namespace BlockStorageCore.Constants;

public static class BlockConstants {
    public const int HeaderFieldSize = ByteLengths.LongLen;
    public const int HeaderFieldCount = 6;
    public const int HeaderSize = HeaderFieldCount * HeaderFieldSize;
}
