namespace DataProcessor.Abstractions;

/// <summary>
/// Interface for building messages from data objects
/// </summary>
/// <typeparam name="T">The type of data to build messages from</typeparam>
public interface IMessageBuilder<T>
{
    /// <summary>
    /// Builds a message string from the provided data
    /// </summary>
    /// <param name="data">The data to build the message from</param>
    /// <returns>A formatted message string</returns>
    string BuildMessage(T data);
}