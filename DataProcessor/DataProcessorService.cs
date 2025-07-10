using System.Text;
using DataProcessor.Abstractions;

namespace DataProcessor;

/// <summary>
/// Main processor for handling DataRecord objects
/// </summary>
public class DataProcessorService
{
    private readonly IMessageBuilder<DataRecord> _messageBuilder;
    private readonly IFileCreator<DataRecord> _fileCreator;
    
    /// <summary>
    /// Initializes a new instance of the DataProcessorService class
    /// </summary>
    /// <param name="messageBuilder">The message builder to use for formatting data</param>
    /// <param name="fileCreator">The file creator to use for writing files</param>
    public DataProcessorService(
        IMessageBuilder<DataRecord> messageBuilder,
        IFileCreator<DataRecord> fileCreator)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _fileCreator = fileCreator ?? throw new ArgumentNullException(nameof(fileCreator));
    }
    
    /// <summary>
    /// Gets a TCP payload byte array for a DataRecord
    /// </summary>
    /// <param name="record">The DataRecord to convert to a byte array</param>
    /// <returns>UTF-8 bytes of the built message</returns>
    public byte[] GetTcpPayload(DataRecord record)
    {
        if (record == null)
        {
            throw new ArgumentNullException(nameof(record));
        }
        
        string message = _messageBuilder.BuildMessage(record);
        return Encoding.UTF8.GetBytes(message);
    }
    
    /// <summary>
    /// Writes a collection of DataRecord objects to a file
    /// </summary>
    /// <param name="records">The collection of records to write</param>
    /// <param name="path">The path where the file should be created</param>
    public void WriteDataFile(IEnumerable<DataRecord> records, string path)
    {
        if (records == null)
        {
            throw new ArgumentNullException(nameof(records));
        }
        
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }
        
        _fileCreator.CreateFile(records, path);
    }
}