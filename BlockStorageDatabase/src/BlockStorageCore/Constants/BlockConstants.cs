namespace BlockStorageCore.Constants;

public static class BlockConstants {
    public const int TotalSize = 4096;

    public const int HeaderFieldSize = ByteLengths.LongLen;
    public const int HeaderFieldCount = 6;
    public const int HeaderSize = HeaderFieldCount * HeaderFieldSize;

    public const int DataSectionSize = TotalSize - HeaderSize;
}
