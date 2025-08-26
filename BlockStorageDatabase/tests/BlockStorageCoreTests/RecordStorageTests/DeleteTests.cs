using BlockStorageCore.Entities;
using BlockStorageCoreTests.Helpers;

namespace BlockStorageCoreTests.RecordStorageTests;

public class DeleteTests {
    [Fact]
    void Delete_FreesUpMultipleBlocks_WhichAreThenReusable() {
        // == Arrange ==
        var stream = new MemoryStream();
        var blockStorage = new BlockStorage(stream, 1024, 48);

        var recordStorage = new RecordStorage(blockStorage);
        var testData1 = RecordStorageMocks.GenerateRecordData(1500); // Should need two blocks
        var firstRecordId = recordStorage.Create(testData1);

        var testData2 = RecordStorageMocks.GenerateRecordData(500); // Should need one block

        var firstRecordData = recordStorage.Find(firstRecordId);

        var expectedStreamLength = 1024 * 3;

        // == Act ==

        // Should delete our record, so now we have record 0 with one block, and two blocks marked as deleted
        recordStorage.Delete(1);

        // This should now use block 1, because it should be marked free.
        // So we have block 0, block 1 for record 1, and block two marked free.
        var secondRecordId = recordStorage.Create(testData2);

        var secondRecordData = recordStorage.Find(secondRecordId);

        // block 2 should have been reused for second record, block 1 should be marked as deleted
        var firstBlockIsMarkedAsDeleted = blockStorage.Find(1).GetHeader(4) == 1L;

        // == Assert ==

        // testData1 was created
        Assert.Equal(testData1, firstRecordData);

        // testData2 was created
        Assert.Equal(testData2, secondRecordData);

        // block 2 is marked as deleted
        Assert.True(firstBlockIsMarkedAsDeleted);

        // stream has 3 blocks total
        Assert.Equal(expectedStreamLength, stream.Length);

        stream.Dispose();
    }
}
