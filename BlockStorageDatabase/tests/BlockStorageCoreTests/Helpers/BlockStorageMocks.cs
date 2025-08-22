using BlockStorageCore.Interfaces;
using Moq;

namespace BlockStorageCoreTests.Helpers;
internal static class BlockStorageMocks {
    public static Mock<IBlockStorage> GetMockStorage() {
        var mockStorage = new Mock<IBlockStorage>();

        mockStorage.Setup(storage => storage.BlockSize).Returns(4096);
        mockStorage.Setup(storage => storage.BlockHeaderSize).Returns(48);
        mockStorage.Setup(storage => storage.BlockHeaderFieldSize).Returns(8);
        mockStorage.Setup(storage => storage.BlockContentSize).Returns(4096 - 48);


        return mockStorage;
    }
}
