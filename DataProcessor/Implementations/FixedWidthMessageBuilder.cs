using System.Globalization;
using DataProcessor.Abstractions;
using DataProcessor.Formatting;

namespace DataProcessor.Implementations;

/// <summary>
/// Builds messages with fixed-width formatting
/// </summary>
/// <typeparam name="T">The type of record to build messages for</typeparam>
public class FixedWidthMessageBuilder<T> : IMessageBuilder<T> where T : class
{
    private readonly FixedWidthFormatter _formatter;
    private readonly Dictionary<string, int> _widthMap;
    private readonly string[] _fieldOrder;
    private readonly CultureInfo _culture;
    
    /// <summary>
    /// Initializes a new instance of the FixedWidthMessageBuilder class for standard records
    /// </summary>
    /// <param name="widthMap">A dictionary mapping property names to field widths</param>
    /// <param name="fieldOrder">The order of fields in the output</param>
    /// <param name="decimalPlaces">Number of decimal places (0-3)</param>
    /// <param name="roundingMode">Rounding mode to use</param>
    /// <param name="culture">The culture to use for formatting</param>
    public FixedWidthMessageBuilder(
        Dictionary<string, int> widthMap,
        string[] fieldOrder,
        int decimalPlaces = 2,
        RoundingMode roundingMode = RoundingMode.Truncate,
        CultureInfo? culture = null)
    {
        ArgumentNullException.ThrowIfNull(widthMap);
        ArgumentNullException.ThrowIfNull(fieldOrder);
        
        _formatter = new FixedWidthFormatter(decimalPlaces, roundingMode);
        _widthMap = widthMap;
        _fieldOrder = fieldOrder;
        _culture = culture ?? CultureInfo.InvariantCulture;
    }
    
    /// <summary>
    /// Builds a message with fixed-width formatting for the specified record
    /// </summary>
    /// <param name="record">The record to build a message for</param>
    /// <returns>A formatted message string</returns>
    public string BuildMessage(T record)
    {
        ArgumentNullException.ThrowIfNull(record);
        
        // Handle container status records specially
        if (typeof(T) == typeof(ContainerStatusRecord) && record is ContainerStatusRecord containerRecord)
        {
            return _formatter.FormatContainerStatus(containerRecord, _culture);
        }
        
        // Handle flexible records specially
        if (typeof(T) == typeof(FlexibleRecord) && record is FlexibleRecord flexibleRecord)
        {
            return _formatter.FormatFlexibleRecord(flexibleRecord, _fieldOrder, _widthMap, _culture);
        }
        
        // Handle standard records
        return _formatter.FormatRecord(record, _widthMap, _culture);
    }
}