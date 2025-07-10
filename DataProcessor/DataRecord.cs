namespace DataProcessor;

/// <summary>
/// Data Transfer Object (DTO) for representing a data record
/// </summary>
public class DataRecord
{
    /// <summary>
    /// Unique identifier for the record
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the record
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Value associated with the record
    /// </summary>
    public decimal Value { get; set; }
}