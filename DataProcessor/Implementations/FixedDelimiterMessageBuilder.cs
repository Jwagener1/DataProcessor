using System.Globalization;
using DataProcessor.Abstractions;
using DataProcessor.Formatting;

namespace DataProcessor.Implementations;

/// <summary>
/// Builds messages from DataRecord objects using a fixed delimiter
/// </summary>
public class FixedDelimiterMessageBuilder : IMessageBuilder<DataRecord>
{
    private readonly string _delimiter;
    private readonly CultureInfo _culture;
    private readonly DecimalFormatter? _decimalFormatter;

    /// <summary>
    /// Initializes a new instance of the FixedDelimiterMessageBuilder class with default formatting
    /// </summary>
    /// <param name="delimiter">The delimiter to use between fields (default: comma)</param>
    public FixedDelimiterMessageBuilder(string delimiter = ",")
        : this(delimiter, null, CultureInfo.InvariantCulture)
    {
    }

    /// <summary>
    /// Initializes a new instance of the FixedDelimiterMessageBuilder class with custom decimal formatting
    /// </summary>
    /// <param name="delimiter">The delimiter to use between fields</param>
    /// <param name="decimalFormatter">The formatter to use for decimal values</param>
    public FixedDelimiterMessageBuilder(string delimiter, DecimalFormatter decimalFormatter)
        : this(delimiter, decimalFormatter, CultureInfo.InvariantCulture)
    {
    }

    /// <summary>
    /// Initializes a new instance of the FixedDelimiterMessageBuilder class with custom decimal formatting and culture
    /// </summary>
    /// <param name="delimiter">The delimiter to use between fields</param>
    /// <param name="decimalFormatter">The formatter to use for decimal values</param>
    /// <param name="culture">The culture to use for formatting</param>
    public FixedDelimiterMessageBuilder(string delimiter, DecimalFormatter? decimalFormatter, CultureInfo culture)
    {
        _delimiter = delimiter ?? throw new ArgumentNullException(nameof(delimiter));
        _culture = culture ?? throw new ArgumentNullException(nameof(culture));
        _decimalFormatter = decimalFormatter;
    }

    /// <summary>
    /// Builds a delimited message from a DataRecord
    /// </summary>
    /// <param name="data">The DataRecord to format</param>
    /// <returns>A string with DataRecord properties separated by the delimiter</returns>
    public string BuildMessage(DataRecord data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        
        string formattedValue = _decimalFormatter != null 
            ? _decimalFormatter.Format(data.Value, _culture)
            : data.Value.ToString(_culture);
        
        return $"{data.Id}{_delimiter}{data.Name}{_delimiter}{formattedValue}";
    }
}