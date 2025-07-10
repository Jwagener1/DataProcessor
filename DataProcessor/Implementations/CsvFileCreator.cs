using System.Text;
using DataProcessor.Abstractions;

namespace DataProcessor.Implementations;

/// <summary>
/// Creates CSV files from DataRecord objects
/// </summary>
public class CsvFileCreator : IFileCreator<DataRecord>
{
    private readonly IMessageBuilder<DataRecord> _messageBuilder;
    
    /// <summary>
    /// Initializes a new instance of the CsvFileCreator class
    /// </summary>
    /// <param name="messageBuilder">The message builder to use for formatting data rows</param>
    public CsvFileCreator(IMessageBuilder<DataRecord> messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }
    
    /// <summary>
    /// Creates a CSV file with header row and data rows from DataRecord objects
    /// </summary>
    /// <param name="data">The collection of DataRecord objects to write to the file</param>
    /// <param name="filePath">The path where the CSV file should be created</param>
    public void CreateFile(IEnumerable<DataRecord> data, string filePath)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }
        
        // Ensure the directory exists
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // Write the file with UTF-8 encoding
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
        
        // Write header
        writer.WriteLine("Id,Name,Value");
        
        // Write data rows
        foreach (var record in data)
        {
            writer.WriteLine(_messageBuilder.BuildMessage(record));
        }
    }
}