using System.Globalization;
using DataProcessor.Abstractions;
using DataProcessor.Formatting;

namespace DataProcessor.Implementations;

/// <summary>
/// Builds messages from ContainerStatusRecord objects using a fixed delimiter
/// </summary>
public class ContainerStatusMessageBuilder : IMessageBuilder<ContainerStatusRecord>
{
    private readonly string _delimiter;
    private readonly CultureInfo _culture;
    private readonly DecimalFormatter _decimalFormatter;

    /// <summary>
    /// Initializes a new instance of the ContainerStatusMessageBuilder class
    /// </summary>
    /// <param name="delimiter">The delimiter to use between fields (default: pipe)</param>
    public ContainerStatusMessageBuilder(string delimiter = "|")
        : this(delimiter, new DecimalFormatter(0, RoundingMode.Truncate), CultureInfo.InvariantCulture)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ContainerStatusMessageBuilder class with custom decimal formatting and culture
    /// </summary>
    /// <param name="delimiter">The delimiter to use between fields</param>
    /// <param name="decimalFormatter">The formatter to use for decimal values</param>
    /// <param name="culture">The culture to use for formatting</param>
    public ContainerStatusMessageBuilder(string delimiter, DecimalFormatter decimalFormatter, CultureInfo culture)
    {
        _delimiter = delimiter ?? throw new ArgumentNullException(nameof(delimiter));
        _decimalFormatter = decimalFormatter ?? throw new ArgumentNullException(nameof(decimalFormatter));
        _culture = culture ?? throw new ArgumentNullException(nameof(culture));
    }

    /// <summary>
    /// Builds a delimited message from a ContainerStatusRecord
    /// </summary>
    /// <param name="data">The ContainerStatusRecord to format</param>
    /// <returns>A string with ContainerStatusRecord properties separated by the delimiter</returns>
    public string BuildMessage(ContainerStatusRecord data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        
        // Format all numeric values using the decimal formatter
        string lengthStr = _decimalFormatter.Format(Convert.ToDecimal(data.Length), _culture);
        string widthStr = _decimalFormatter.Format(Convert.ToDecimal(data.Width), _culture);
        string heightStr = _decimalFormatter.Format(Convert.ToDecimal(data.Height), _culture);
        string volumeStr = _decimalFormatter.Format(Convert.ToDecimal(data.Volume), _culture);
        string weightStr = _decimalFormatter.Format(Convert.ToDecimal(data.Weight), _culture);

        // Combine all fields with the delimiter
        return string.Join(_delimiter, new[]
        {
            data.StatusType,
            data.Barcode,
            data.Status,
            data.DimensionType,
            lengthStr,
            widthStr,
            heightStr,
            volumeStr,
            weightStr
        });
    }
}