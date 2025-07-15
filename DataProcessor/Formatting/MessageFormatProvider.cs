using System.Collections.Concurrent;

namespace DataProcessor.Formatting;

/// <summary>
/// Provides access to client-specific message formats
/// </summary>
public class MessageFormatProvider
{
    private readonly ConcurrentDictionary<string, MessageFormat> _formats = new();
    
    /// <summary>
    /// Registers a message format for a specific client
    /// </summary>
    /// <param name="clientId">The unique identifier for the client</param>
    /// <param name="format">The message format for the client</param>
    /// <returns>True if the format was registered successfully, false if the client already had a format</returns>
    public bool RegisterFormat(string clientId, MessageFormat format)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("Client ID cannot be empty", nameof(clientId));
        }
        
        return _formats.TryAdd(clientId, format ?? throw new ArgumentNullException(nameof(format)));
    }
    
    /// <summary>
    /// Updates or registers a message format for a specific client
    /// </summary>
    /// <param name="clientId">The unique identifier for the client</param>
    /// <param name="format">The message format for the client</param>
    public void SetFormat(string clientId, MessageFormat format)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("Client ID cannot be empty", nameof(clientId));
        }
        
        _formats[clientId] = format ?? throw new ArgumentNullException(nameof(format));
    }
    
    /// <summary>
    /// Gets the message format for a specific client
    /// </summary>
    /// <param name="clientId">The unique identifier for the client</param>
    /// <returns>The message format for the client, or null if no format is registered</returns>
    public MessageFormat? GetFormat(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("Client ID cannot be empty", nameof(clientId));
        }
        
        _formats.TryGetValue(clientId, out var format);
        return format;
    }
    
    /// <summary>
    /// Determines whether a message format is registered for a specific client
    /// </summary>
    /// <param name="clientId">The unique identifier for the client</param>
    /// <returns>True if a format is registered for the client, false otherwise</returns>
    public bool HasFormat(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("Client ID cannot be empty", nameof(clientId));
        }
        
        return _formats.ContainsKey(clientId);
    }
    
    /// <summary>
    /// Removes the message format for a specific client
    /// </summary>
    /// <param name="clientId">The unique identifier for the client</param>
    /// <returns>True if the format was removed, false if no format was registered for the client</returns>
    public bool RemoveFormat(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("Client ID cannot be empty", nameof(clientId));
        }
        
        return _formats.TryRemove(clientId, out _);
    }
}