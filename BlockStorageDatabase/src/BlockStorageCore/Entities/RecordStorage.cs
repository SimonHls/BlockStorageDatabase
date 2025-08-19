using BlockStorageCore.Interfaces;

namespace BlockStorageCore.Entities;

public class RecordStorage : IRecordStorage {
    public uint Create() {
        throw new NotImplementedException();
    }

    public uint Create(byte[] data) {
        throw new NotImplementedException();
    }

    public uint Create(Func<uint, byte[]> dataGenerator) {
        throw new NotImplementedException();
    }

    public void Delete(uint recordId) {
        throw new NotImplementedException();
    }

    public byte[] Find(uint recordId) {
        throw new NotImplementedException();
    }

    public void Update(uint recordId, byte[] data) {
        throw new NotImplementedException();
    }
}
