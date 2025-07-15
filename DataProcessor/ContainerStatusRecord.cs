namespace DataProcessor;

/// <summary>
/// Data Transfer Object (DTO) for representing a container status record
/// </summary>
public class ContainerStatusRecord
{
    /// <summary>
    /// Status type (e.g., "CONTAINERSTATUS")
    /// </summary>
    public string StatusType { get; set; } = string.Empty;
    
    /// <summary>
    /// Barcode identifier
    /// </summary>
    public string Barcode { get; set; } = string.Empty;
    
    /// <summary>
    /// Status (e.g., "SCANNED")
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Dimension type (e.g., "DIMS")
    /// </summary>
    public string DimensionType { get; set; } = string.Empty;
    
    /// <summary>
    /// Length measurement
    /// </summary>
    public float Length { get; set; }
    
    /// <summary>
    /// Width measurement
    /// </summary>
    public float Width { get; set; }
    
    /// <summary>
    /// Height measurement
    /// </summary>
    public float Height { get; set; }
    
    /// <summary>
    /// Volume calculation
    /// </summary>
    public float Volume { get; set; }
    
    /// <summary>
    /// Weight measurement
    /// </summary>
    public float Weight { get; set; }
    
    /// <summary>
    /// Creates a container status record with default values
    /// </summary>
    public ContainerStatusRecord()
    {
    }
    
    /// <summary>
    /// Creates a container status record with specified values
    /// </summary>
    public ContainerStatusRecord(
        string statusType, 
        string barcode, 
        string status, 
        string dimensionType,
        float length,
        float width,
        float height,
        float volume,
        float weight)
    {
        StatusType = statusType;
        Barcode = barcode;
        Status = status;
        DimensionType = dimensionType;
        Length = length;
        Width = width;
        Height = height;
        Volume = volume;
        Weight = weight;
    }
}