using System.Collections.Generic;

namespace DataProcessor;

/// <summary>
/// A flexible data record that stores values in a dictionary
/// </summary>
public class FlexibleRecord
{
    private readonly Dictionary<string, object> _data = new();
    
    /// <summary>
    /// Gets or sets a value in the record
    /// </summary>
    /// <param name="key">The key for the value</param>
    /// <returns>The value, or null if the key doesn't exist</returns>
    public object? this[string key]
    {
        get => _data.TryGetValue(key, out var value) ? value : null;
        set
        {
            if (value == null && _data.ContainsKey(key))
            {
                _data.Remove(key);
            }
            else if (value != null)
            {
                _data[key] = value;
            }
        }
    }
    
    /// <summary>
    /// Gets all keys in the record
    /// </summary>
    public IEnumerable<string> Keys => _data.Keys;
    
    /// <summary>
    /// Determines whether the record contains the specified key
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if the key exists, false otherwise</returns>
    public bool ContainsKey(string key) => _data.ContainsKey(key);
    
    /// <summary>
    /// Gets a typed value from the record
    /// </summary>
    /// <typeparam name="T">The type to convert the value to</typeparam>
    /// <param name="key">The key for the value</param>
    /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
    /// <returns>The value converted to type T, or defaultValue if the key doesn't exist</returns>
    public T GetValue<T>(string key, T defaultValue = default!)
    {
        if (!_data.TryGetValue(key, out var value))
        {
            return defaultValue;
        }
        
        if (value is T typedValue)
        {
            return typedValue;
        }
        
        try
        {
            // Try to convert the value to the requested type
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Sets a value in the record
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="key">The key for the value</param>
    /// <param name="value">The value to set</param>
    public void SetValue<T>(string key, T value)
    {
        if (value == null && _data.ContainsKey(key))
        {
            _data.Remove(key);
        }
        else if (value != null)
        {
            _data[key] = value;
        }
    }
}