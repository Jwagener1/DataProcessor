using System.Collections.ObjectModel;

namespace DataProcessor.Formatting;

/// <summary>
/// Defines a message format using a collection of format tokens
/// </summary>
public class MessageFormat
{
    /// <summary>
    /// The collection of format tokens that define this message format
    /// </summary>
    public ReadOnlyCollection<FormatToken> Tokens { get; }
    
    /// <summary>
    /// The delimiter to use between tokens
    /// </summary>
    public string Delimiter { get; }
    
    /// <summary>
    /// Creates a message format with the specified tokens and delimiter
    /// </summary>
    /// <param name="tokens">The tokens that define the message format</param>
    /// <param name="delimiter">The delimiter to use between tokens (default: pipe)</param>
    public MessageFormat(IEnumerable<FormatToken> tokens, string delimiter = "|")
    {
        if (tokens == null)
        {
            throw new ArgumentNullException(nameof(tokens));
        }
        
        var tokensList = tokens.ToList();
        if (!tokensList.Any())
        {
            throw new ArgumentException("At least one token must be provided", nameof(tokens));
        }
        
        Tokens = new ReadOnlyCollection<FormatToken>(tokensList);
        Delimiter = delimiter ?? throw new ArgumentNullException(nameof(delimiter));
    }
}