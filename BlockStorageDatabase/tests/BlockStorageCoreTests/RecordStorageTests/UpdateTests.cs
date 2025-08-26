using BlockStorageCore.Entities;
using BlockStorageCoreTests.Helpers;

namespace BlockStorageCoreTests.RecordStorageTests;
public class UpdateTests {
    [Fact]
    void Update_UpdatesDataInRecord() {
        // == Arrange ==
        var stream = new MemoryStream();
        var blockStorage = new BlockStorage(stream, 1024, 48);
        var recordStorage = new RecordStorage(blockStorage);
        var initialData = RecordStorageMocks.GenerateRecordData(2048); // should need three blocks
        var updateData = RecordStorageMocks.GenerateRecordData(2050); // should also need three blocks, different length just to make sure we get a different array
        var newRecordId = recordStorage.Create(initialData);
        var initialStreamLength = stream.Length;

        // == Act ==
        var initialDataFromRecord = recordStorage.Find(newRecordId);
        recordStorage.Update(newRecordId, updateData);
        var updateDataFromRecord = recordStorage.Find(newRecordId);
        var streamLengthAfterUpdate = stream.Length;

        // == Assert ==
        // Sanity check -> initial data was written corretly
        Assert.Equal(initialData, initialDataFromRecord);
        // Record was updated
        Assert.Equal(updateData, updateDataFromRecord);
        // Stream length did not change
        Assert.Equal(initialStreamLength, streamLengthAfterUpdate);

        stream.Dispose();
    }

    [Fact]
    void Update_FreesUpBlock_WhenUpdatedRecordNeedsLessBlocks() {
        // == Arrange ==
        var stream = new MemoryStream();
        var blockStorage = new BlockStorage(stream, 1024, 48);
        var recordStorage = new RecordStorage(blockStorage);
        var initialData = RecordStorageMocks.GenerateRecordData(2048); // should need three blocks
        var updateData = RecordStorageMocks.GenerateRecordData(1200); // should need two blocks
        var newRecordId = recordStorage.Create(initialData);
        uint kIsDeletedHeader = 4;

        // == Act ==
        var initialDataFromRecord = recordStorage.Find(newRecordId);
        // Block 3 should be used before update
        var block3IsDeletedBeforeUpdate = blockStorage.Find(3).GetHeader(kIsDeletedHeader) == 1L;
        recordStorage.Update(newRecordId, updateData);
        var updateDataFromRecord = recordStorage.Find(newRecordId);
        // Block 3 should be free
        var block3IsDeletedAfterUpdate = blockStorage.Find(3).GetHeader(kIsDeletedHeader) == 1L;

        // == Assert ==
        // Sanity check -> initial data was written corretly
        Assert.Equal(initialData, initialDataFromRecord);
        // Record was updated
        Assert.Equal(updateData, updateDataFromRecord);
        // Block 3 was used before update
        Assert.False(block3IsDeletedBeforeUpdate);
        // Block 3 was freed after update
        Assert.True(block3IsDeletedAfterUpdate);

        stream.Dispose();
    }

    [Fact]
    void Update_AllocatesNewBlocks_WhenUpdatedRecordNeedsMoreBlocksThenInitialRecord() {
        // == Arrange ==
        var stream = new MemoryStream();
        var blockStorage = new BlockStorage(stream, 1024, 48);
        var recordStorage = new RecordStorage(blockStorage);
        var initialData = RecordStorageMocks.GenerateRecordData(2048); // should need three blocks
        var updateData = RecordStorageMocks.GenerateRecordData(4500);
        // Should need 6 blocks (4,6 blocks for data, one block for free list)
        var expectedFinalStreamLength = 1024 * 6;
        var newRecordId = recordStorage.Create(initialData);

        // == Act ==
        var initialDataFromRecord = recordStorage.Find(newRecordId);
        recordStorage.Update(newRecordId, updateData);
        var updateDataFromRecord = recordStorage.Find(newRecordId);

        // == Assert ==
        // Sanity check -> initial data was written corretly
        Assert.Equal(initialData, initialDataFromRecord);
        // Record was updated
        Assert.Equal(updateData, updateDataFromRecord);
        // Stream is now longer
        Assert.Equal(expectedFinalStreamLength, stream.Length);

        stream.Dispose();
    }
}
