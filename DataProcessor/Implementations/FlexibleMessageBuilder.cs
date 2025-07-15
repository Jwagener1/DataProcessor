using System.Globalization;
using DataProcessor.Abstractions;
using DataProcessor.Formatting;

namespace DataProcessor.Implementations;

/// <summary>
/// Builds messages from FlexibleRecord objects using a configurable format
/// </summary>
public class FlexibleMessageBuilder : IMessageBuilder<FlexibleRecord>
{
    private readonly MessageFormat _messageFormat;
    private readonly CultureInfo _culture;
    private readonly DecimalFormatter? _decimalFormatter;
    
    /// <summary>
    /// Initializes a new instance of the FlexibleMessageBuilder class
    /// </summary>
    /// <param name="messageFormat">The format to use for building messages</param>
    public FlexibleMessageBuilder(MessageFormat messageFormat)
        : this(messageFormat, null, CultureInfo.InvariantCulture)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the FlexibleMessageBuilder class with custom decimal formatting
    /// </summary>
    /// <param name="messageFormat">The format to use for building messages</param>
    /// <param name="decimalFormatter">The formatter to use for decimal values</param>
    public FlexibleMessageBuilder(MessageFormat messageFormat, DecimalFormatter decimalFormatter)
        : this(messageFormat, decimalFormatter, CultureInfo.InvariantCulture)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the FlexibleMessageBuilder class with custom decimal formatting and culture
    /// </summary>
    /// <param name="messageFormat">The format to use for building messages</param>
    /// <param name="decimalFormatter">The formatter to use for decimal values</param>
    /// <param name="culture">The culture to use for formatting</param>
    public FlexibleMessageBuilder(MessageFormat messageFormat, DecimalFormatter? decimalFormatter, CultureInfo culture)
    {
        _messageFormat = messageFormat ?? throw new ArgumentNullException(nameof(messageFormat));
        _culture = culture ?? throw new ArgumentNullException(nameof(culture));
        _decimalFormatter = decimalFormatter;
    }
    
    /// <summary>
    /// Builds a message from a FlexibleRecord according to the defined format
    /// </summary>
    /// <param name="data">The FlexibleRecord containing the data to format</param>
    /// <returns>A formatted message string</returns>
    public string BuildMessage(FlexibleRecord data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        
        var values = new List<string>();
        
        foreach (var token in _messageFormat.Tokens)
        {
            if (token.IsLiteral)
            {
                values.Add(token.Literal!);
            }
            else if (token.Key != null && data.ContainsKey(token.Key))
            {
                var value = data[token.Key];
                string formattedValue = FormatValue(value);
                values.Add(formattedValue);
            }
            else
            {
                // If the key doesn't exist, add an empty string
                values.Add(string.Empty);
            }
        }
        
        return string.Join(_messageFormat.Delimiter, values);
    }
    
    private string FormatValue(object? value)
    {
        if (value == null)
        {
            return string.Empty;
        }
        
        if (_decimalFormatter != null && (value is decimal || value is float || value is double))
        {
            try
            {
                var decimalValue = Convert.ToDecimal(value);
                return _decimalFormatter.Format(decimalValue, _culture);
            }
            catch
            {
                // If conversion fails, fall back to default formatting
            }
        }
        
        // Default formatting using the current culture
        return Convert.ToString(value, _culture) ?? string.Empty;
    }
}