using System.Text;
using DataProcessor.Abstractions;

namespace DataProcessor;

/// <summary>
/// Processor service for handling ContainerStatusRecord objects
/// </summary>
public class ContainerStatusProcessorService
{
    private readonly IMessageBuilder<ContainerStatusRecord> _messageBuilder;
    
    /// <summary>
    /// Initializes a new instance of the ContainerStatusProcessorService class
    /// </summary>
    /// <param name="messageBuilder">The message builder to use for formatting container status records</param>
    public ContainerStatusProcessorService(IMessageBuilder<ContainerStatusRecord> messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }
    
    /// <summary>
    /// Gets a TCP payload byte array for a ContainerStatusRecord
    /// </summary>
    /// <param name="record">The ContainerStatusRecord to convert to a byte array</param>
    /// <returns>UTF-8 bytes of the built message</returns>
    public byte[] GetTcpPayload(ContainerStatusRecord record)
    {
        if (record == null)
        {
            throw new ArgumentNullException(nameof(record));
        }
        
        string message = _messageBuilder.BuildMessage(record);
        return Encoding.UTF8.GetBytes(message);
    }
    
    /// <summary>
    /// Creates a pre-configured container status record with the provided values
    /// </summary>
    /// <param name="barcode">The barcode to include in the record</param>
    /// <param name="length">The length measurement</param>
    /// <param name="width">The width measurement</param>
    /// <param name="height">The height measurement</param>
    /// <param name="weight">The weight measurement</param>
    /// <returns>A pre-configured ContainerStatusRecord</returns>
    public ContainerStatusRecord CreateContainerStatus(
        string barcode,
        float length,
        float width,
        float height,
        float weight)
    {
        // Calculate volume based on length, width, height
        float volume = length * width * height;
        
        return new ContainerStatusRecord(
            statusType: "CONTAINERSTATUS",
            barcode: barcode,
            status: "SCANNED",
            dimensionType: "DIMS",
            length: length,
            width: width,
            height: height,
            volume: volume,
            weight: weight
        );
    }
}