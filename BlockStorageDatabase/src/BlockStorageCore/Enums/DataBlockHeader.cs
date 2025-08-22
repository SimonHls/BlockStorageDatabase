namespace BlockStorageCore.Enums;

public enum DataBlockHeader {
    NextBlockId = 0,
    PreviousBlockId = 1,
    RecordLength = 2,
    BlockContentLength = 3,
    IsDeleted = 4
}
