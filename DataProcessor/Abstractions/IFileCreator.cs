namespace DataProcessor.Abstractions;

/// <summary>
/// Interface for creating files from collections of data objects
/// </summary>
/// <typeparam name="T">The type of data to create files from</typeparam>
public interface IFileCreator<T>
{
    /// <summary>
    /// Creates a file from a collection of data objects
    /// </summary>
    /// <param name="data">The collection of data to write to the file</param>
    /// <param name="filePath">The path where the file should be created</param>
    void CreateFile(IEnumerable<T> data, string filePath);
}