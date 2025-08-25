namespace BlockStorageCoreTests.Helpers;
public static class RecordStorageMocks {
    public static byte[] GenerateRecordData(int length) {
        byte[] byteArray = new byte[length];

        var random = new Random();
        random.NextBytes(byteArray);
        return byteArray;
    }
}
