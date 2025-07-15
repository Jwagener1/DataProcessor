namespace DataProcessor.Formatting;

/// <summary>
/// Represents a token in a message format
/// </summary>
public class FormatToken
{
    /// <summary>
    /// The literal value for this token. If set, this exact value will be used in the message.
    /// </summary>
    public string? Literal { get; }
    
    /// <summary>
    /// The key for retrieving a value from a data dictionary. If set, the value will be retrieved from the data.
    /// </summary>
    public string? Key { get; }
    
    /// <summary>
    /// Indicates whether this token is a literal value
    /// </summary>
    public bool IsLiteral => !string.IsNullOrEmpty(Literal);
    
    /// <summary>
    /// Creates a literal format token
    /// </summary>
    /// <param name="literal">The literal value to use in the message</param>
    public FormatToken(string literal)
    {
        Literal = literal ?? throw new ArgumentNullException(nameof(literal));
    }
    
    /// <summary>
    /// Creates a key-based format token
    /// </summary>
    /// <param name="key">The key to use for retrieving a value from data</param>
    public FormatToken(string key, bool isKey)
    {
        if (!isKey)
        {
            throw new ArgumentException("Use the literal constructor for literal values", nameof(isKey));
        }
        
        Key = key ?? throw new ArgumentNullException(nameof(key));
    }
    
    /// <summary>
    /// Creates a literal format token
    /// </summary>
    /// <param name="literal">The literal value to use in the message</param>
    public static FormatToken CreateLiteral(string literal) => new FormatToken(literal);
    
    /// <summary>
    /// Creates a key-based format token
    /// </summary>
    /// <param name="key">The key to use for retrieving a value from data</param>
    public static FormatToken CreateKey(string key) => new FormatToken(key, true);
}