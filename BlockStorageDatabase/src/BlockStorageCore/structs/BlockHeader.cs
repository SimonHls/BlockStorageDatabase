using BlockStorageCore.Enums;
using System.Runtime.InteropServices;

namespace BlockStorageCore.structs;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct BlockHeader {
    public ulong MagicNumber;
    public ulong BlockId;
    public ulong NextBlockId;
    public ulong Checksum;
    private ulong _flagsAndMetadata; // For block type and data length

    public const ulong ExpectedMagicNumber = 0xBAAAAAAD_F0CACC1A; // Buon Giorno
    public const int SizeInBytes = 4 * 8; // 4 fields á 8 bytes

    public BlockType BlockType {
        get => (BlockType)(_flagsAndMetadata & 0xFF); // Get the first byte
        set => _flagsAndMetadata = (_flagsAndMetadata & 0xFFFFFFFFFFFFFF00) | (byte)value;
    }

    public ushort DataLength {
        get => (ushort)((_flagsAndMetadata >> 8) & 0xFFFF); // Shift right by 1 byte, then mask 2 bytes
        set => _flagsAndMetadata = (_flagsAndMetadata & 0xFFFFFFFFFFFF00FF) | ((ulong)value << 8);
    }

    public ulong GetHeader(HeaderField field) {
        switch (field) {
            case HeaderField.MagicNumber:
                return MagicNumber;
            case HeaderField.BlockId:
                return BlockId;
            case HeaderField.NextBlockId:
                return NextBlockId;
            case HeaderField.Checksum:
                return Checksum;
            case HeaderField.FlagsAndMetadata:
                return _flagsAndMetadata;
            default:
                throw new ArgumentOutOfRangeException(nameof(field), "Invalid header field index.");
        }
    }

    public void SetHeader(HeaderField field, ulong value) {
        switch (field) {
            case HeaderField.MagicNumber:
                MagicNumber = value;
                break;
            case HeaderField.BlockId:
                BlockId = value;
                break;
            case HeaderField.NextBlockId:
                NextBlockId = value;
                break;
            case HeaderField.Checksum:
                Checksum = value;
                break;
            case HeaderField.FlagsAndMetadata:
                _flagsAndMetadata = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(field), "Invalid header field index.");
        }
    }

    public void Initialize(ulong blockId) {
        MagicNumber = ExpectedMagicNumber;
        BlockId = blockId;
        NextBlockId = 0; // Represents no next block
        Checksum = 0;
        _flagsAndMetadata = 0;

        // Set default values using the properties
        BlockType = BlockType.Unused;
        DataLength = 0;
    }
}