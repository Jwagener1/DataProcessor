using System.Globalization;
using System.Text;
using DataProcessor.Formatting;
using DataProcessor.Implementations;

namespace DataProcessor;

/// <summary>
/// Processor service for handling FlexibleRecord objects with client-specific formats
/// </summary>
public class FlexibleProcessorService
{
    private readonly MessageFormatProvider _formatProvider;
    private readonly DecimalFormatter _decimalFormatter;
    private readonly CultureInfo _culture;
    
    /// <summary>
    /// Initializes a new instance of the FlexibleProcessorService class with default formatting
    /// </summary>
    /// <param name="formatProvider">The provider of client-specific message formats</param>
    public FlexibleProcessorService(MessageFormatProvider formatProvider)
        : this(formatProvider, new DecimalFormatter(0, RoundingMode.Truncate), CultureInfo.InvariantCulture)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the FlexibleProcessorService class with custom formatting
    /// </summary>
    /// <param name="formatProvider">The provider of client-specific message formats</param>
    /// <param name="decimalFormatter">The formatter to use for decimal values</param>
    /// <param name="culture">The culture to use for formatting</param>
    public FlexibleProcessorService(MessageFormatProvider formatProvider, DecimalFormatter decimalFormatter, CultureInfo culture)
    {
        _formatProvider = formatProvider ?? throw new ArgumentNullException(nameof(formatProvider));
        _decimalFormatter = decimalFormatter ?? throw new ArgumentNullException(nameof(decimalFormatter));
        _culture = culture ?? throw new ArgumentNullException(nameof(culture));
    }
    
    /// <summary>
    /// Gets a TCP payload byte array for a FlexibleRecord using a client-specific format
    /// </summary>
    /// <param name="clientId">The unique identifier for the client</param>
    /// <param name="record">The FlexibleRecord to convert to a byte array</param>
    /// <returns>UTF-8 bytes of the built message</returns>
    public byte[] GetTcpPayload(string clientId, FlexibleRecord record)
    {
        if (record == null)
        {
            throw new ArgumentNullException(nameof(record));
        }
        
        var format = _formatProvider.GetFormat(clientId);
        if (format == null)
        {
            throw new ArgumentException($"No message format registered for client '{clientId}'", nameof(clientId));
        }
        
        var messageBuilder = new FlexibleMessageBuilder(format, _decimalFormatter, _culture);
        string message = messageBuilder.BuildMessage(record);
        
        return Encoding.UTF8.GetBytes(message);
    }
    
    /// <summary>
    /// Gets a formatted message string for a FlexibleRecord using a client-specific format
    /// </summary>
    /// <param name="clientId">The unique identifier for the client</param>
    /// <param name="record">The FlexibleRecord to format</param>
    /// <returns>A formatted message string</returns>
    public string GetMessage(string clientId, FlexibleRecord record)
    {
        if (record == null)
        {
            throw new ArgumentNullException(nameof(record));
        }
        
        var format = _formatProvider.GetFormat(clientId);
        if (format == null)
        {
            throw new ArgumentException($"No message format registered for client '{clientId}'", nameof(clientId));
        }
        
        var messageBuilder = new FlexibleMessageBuilder(format, _decimalFormatter, _culture);
        return messageBuilder.BuildMessage(record);
    }
    
    /// <summary>
    /// Creates a container status record with the specified values
    /// </summary>
    /// <param name="containerId">The container ID</param>
    /// <param name="scanAction">The scan action (default: "SCANNED")</param>
    /// <param name="format">The format specifier (default: "DIMS")</param>
    /// <param name="length">The length measurement</param>
    /// <param name="width">The width measurement</param>
    /// <param name="height">The height measurement</param>
    /// <param name="weight">The weight measurement</param>
    /// <returns>A FlexibleRecord with the container status data</returns>
    public FlexibleRecord CreateContainerStatus(
        string containerId,
        string scanAction = "SCANNED",
        string format = "DIMS",
        float length = 0,
        float width = 0,
        float height = 0,
        float weight = 0)
    {
        var record = new FlexibleRecord();
        
        record.SetValue("ContainerId", containerId);
        record.SetValue("ScanAction", scanAction);
        record.SetValue("Format", format);
        record.SetValue("Length", length);
        record.SetValue("Width", width);
        record.SetValue("Height", height);
        
        // Calculate volume
        float volume = length * width * height;
        record.SetValue("Volume", volume);
        
        record.SetValue("Weight", weight);
        
        return record;
    }
}