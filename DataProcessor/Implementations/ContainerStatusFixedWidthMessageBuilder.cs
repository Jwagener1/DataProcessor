using System.Globalization;
using DataProcessor.Abstractions;
using DataProcessor.Formatting;

namespace DataProcessor.Implementations;

/// <summary>
/// A message builder specifically for container status records using fixed-width formatting
/// </summary>
public class ContainerStatusFixedWidthMessageBuilder : IMessageBuilder<ContainerStatusRecord>
{
    private readonly FixedWidthFormatter _formatter;
    private readonly CultureInfo _culture;
    
    /// <summary>
    /// Initializes a new instance of the ContainerStatusFixedWidthMessageBuilder class
    /// </summary>
    /// <param name="decimalPlaces">Number of decimal places (0-3)</param>
    /// <param name="roundingMode">Rounding mode to use</param>
    /// <param name="culture">The culture to use for formatting</param>
    public ContainerStatusFixedWidthMessageBuilder(
        int decimalPlaces = 2, 
        RoundingMode roundingMode = RoundingMode.Truncate,
        CultureInfo? culture = null)
    {
        _formatter = new FixedWidthFormatter(decimalPlaces, roundingMode);
        _culture = culture ?? CultureInfo.InvariantCulture;
    }
    
    /// <summary>
    /// Builds a fixed-width formatted message for a container status record
    /// </summary>
    /// <param name="record">The container status record</param>
    /// <returns>A fixed-width formatted message</returns>
    public string BuildMessage(ContainerStatusRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        
        return _formatter.FormatContainerStatus(record, _culture);
    }
}