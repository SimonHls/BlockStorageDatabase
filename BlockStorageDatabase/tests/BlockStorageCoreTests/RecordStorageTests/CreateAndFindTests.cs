using BlockStorageCore.Entities;

namespace BlockStorageCoreTests.RecordStorageTests;
public class CreateAndFindTests {

    [Fact]
    public void Create_CreatesRecordInEmptyDatabaseWhenReceivingByteArray() {
        // == Arrange ==
        var stream = new MemoryStream();
        var blockStorage = new BlockStorage(stream, 1024, 48);

        var recordStorage = new RecordStorage(blockStorage);
        var testData = GenerateRecordData(500);

        // == Act ==
        var newRecordId = recordStorage.Create(testData);

        var recordData = recordStorage.Find(newRecordId);

        // == Assert ==
        Assert.Equal(1, (int)newRecordId);
        Assert.Equal(testData, recordData);
    }

    private byte[] GenerateRecordData(int length) {
        byte[] byteArray = new byte[length];

        var random = new Random();
        random.NextBytes(byteArray);
        return byteArray;
    }
}
