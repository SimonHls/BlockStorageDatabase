using BlockStorageCore.Entities;
using BlockStorageCore.Entities.Storage;
using BlockStorageCoreTests.Helpers;

namespace BlockStorageCoreTests.RecordStorageTests;
public class CreateAndFindTests {

    [Fact]
    public void Create_CreatesRecordInEmptyDatabase_WhenReceivingByteArray() {
        // == Arrange ==
        var stream = new MemoryStream();
        var blockStorage = new BlockStorage(stream, 1024, 48);

        var recordStorage = new RecordStorage(blockStorage);
        var testData = RecordStorageMocks.GenerateRecordData(500);

        // == Act ==
        var newRecordId = recordStorage.Create(testData);

        var recordData = recordStorage.Find(newRecordId);

        // == Assert ==
        Assert.Equal(1, (int)newRecordId);
        Assert.Equal(testData, recordData);

        stream.Dispose();
    }

    [Fact]
    public void Create_CreatesRecordOverMultipleBlocksInEmptyDatabase_WhenReceivingByteArray() {
        // == Arrange ==
        var stream = new MemoryStream();
        var blockStorage = new BlockStorage(stream, 1024, 48);

        var recordStorage = new RecordStorage(blockStorage);
        var testData = RecordStorageMocks.GenerateRecordData(2048); // should need three blocks

        // == Act ==
        // Write to recordStorage and read back the result
        var newRecordId = recordStorage.Create(testData);
        var recordData = recordStorage.Find(newRecordId);

        // This block should exist
        var lastBlock = blockStorage.Find(3);

        // == Assert ==
        Assert.Equal(1, (int)newRecordId);
        Assert.Equal(testData, recordData);
        Assert.NotNull(lastBlock);
        Assert.Equal(3, (int)lastBlock.Id);

        stream.Dispose();
    }

}
