using System.Globalization;
using System.Text;
using DataProcessor.Formatting;
using DataProcessor.Implementations;

namespace DataProcessor;

/// <summary>
/// Processor service for handling ContainerStatusRecord objects with fixed-width formatting
/// </summary>
public class FixedWidthContainerProcessorService
{
    private readonly ContainerStatusFixedWidthMessageBuilder _messageBuilder;
    
    /// <summary>
    /// Initializes a new instance of the FixedWidthContainerProcessorService class
    /// </summary>
    /// <param name="decimalPlaces">Number of decimal places (0-3)</param>
    /// <param name="roundingMode">Rounding mode to use</param>
    /// <param name="culture">The culture to use for formatting</param>
    public FixedWidthContainerProcessorService(
        int decimalPlaces = 2,
        RoundingMode roundingMode = RoundingMode.Truncate,
        CultureInfo? culture = null)
    {
        _messageBuilder = new ContainerStatusFixedWidthMessageBuilder(
            decimalPlaces, roundingMode, culture);
    }
    
    /// <summary>
    /// Gets a TCP payload byte array for a ContainerStatusRecord using fixed-width formatting
    /// </summary>
    /// <param name="record">The ContainerStatusRecord to convert to a byte array</param>
    /// <returns>UTF-8 bytes of the fixed-width formatted message</returns>
    public byte[] GetTcpPayload(ContainerStatusRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        
        string message = _messageBuilder.BuildMessage(record);
        return Encoding.UTF8.GetBytes(message);
    }
    
    /// <summary>
    /// Gets a fixed-width formatted string for a ContainerStatusRecord
    /// </summary>
    /// <param name="record">The ContainerStatusRecord to format</param>
    /// <returns>A fixed-width formatted message</returns>
    public string GetFormattedMessage(ContainerStatusRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        
        return _messageBuilder.BuildMessage(record);
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
        ArgumentNullException.ThrowIfNull(barcode);
        
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