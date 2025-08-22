namespace BlockStorageCore.Interfaces;

public interface IRecordStorage {
    /// <summary>
    /// Effectively update an record
    /// </summary>
    void Update(uint recordId, byte[] data);

    /// <summary>
    /// Grab a record's data
    /// </summary>
    byte[]? Find(uint recordId);

    /// <summary>
    /// This creates new empty record
    /// </summary>
    uint Create();

    /// <summary>
    /// This creates new record with given data and returns its ID
    /// </summary>
    uint Create(byte[] data);

    /// <summary>
    /// Similar to Create(byte[] data), but with dataGenerator which generates
    /// data after a record is allocated.
    /// I needed to wrpa my head around this: 
    /// Basically, the data generator allows us to so stuff with the alocated record id before giving the writer the data.
    /// In our case, we use the record id to build up indexing for our new record. 
    /// </summary>
    uint Create(Func<uint, byte[]> dataGenerator);

    /// <summary>
    /// This deletes a record by its id
    /// </summary>
    void Delete(uint recordId);
}