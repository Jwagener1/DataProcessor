using System.Text;
using DataProcessor.Formatting;
using DataProcessor.Implementations;

namespace DataProcessor.Examples;

/// <summary>
/// Example usage of the ContainerStatusRecord and ContainerStatusMessageBuilder
/// </summary>
public static class ContainerStatusExample
{
    /// <summary>
    /// Creates a container status message in the format: CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|57910|13
    /// </summary>
    /// <returns>The formatted container status message</returns>
    public static string CreateExampleContainerStatusMessage()
    {
        // Create a container status record with your data
        var containerRecord = new ContainerStatusRecord
        {
            StatusType = "CONTAINERSTATUS",
            Barcode = "317164239",
            Status = "SCANNED",
            DimensionType = "DIMS",
            Length = 44.7f,
            Width = 35.2f,
            Height = 38.9f,
            Volume = 57910.3f,
            Weight = 13.8f
        };
        
        // Create a formatter that truncates decimal values (no decimal places)
        var formatter = new DecimalFormatter(0, RoundingMode.Truncate);
        
        // Create the message builder with pipe delimiter
        var messageBuilder = new ContainerStatusMessageBuilder("|", formatter, System.Globalization.CultureInfo.InvariantCulture);
        
        // Build and return the message
        return messageBuilder.BuildMessage(containerRecord);
    }
    
    /// <summary>
    /// Creates a byte array payload for TCP transmission with the container status message
    /// </summary>
    /// <returns>The byte array payload</returns>
    public static byte[] CreateContainerStatusPayload()
    {
        string message = CreateExampleContainerStatusMessage();
        return Encoding.UTF8.GetBytes(message);
    }
    
    /// <summary>
    /// Demonstrates using the ContainerStatusProcessorService
    /// </summary>
    /// <returns>The container status message</returns>
    public static string UseContainerStatusProcessor()
    {
        // Create a formatter that truncates decimal values
        var formatter = new DecimalFormatter(0, RoundingMode.Truncate);
        
        // Create the message builder
        var messageBuilder = new ContainerStatusMessageBuilder("|", formatter, System.Globalization.CultureInfo.InvariantCulture);
        
        // Create the processor service
        var processor = new ContainerStatusProcessorService(messageBuilder);
        
        // Use the helper method to create a pre-configured container status record
        var containerRecord = processor.CreateContainerStatus(
            barcode: "317164239",
            length: 44f,
            width: 35f,
            height: 38f,
            weight: 13f
        );
        
        // Get the TCP payload
        byte[] payload = processor.GetTcpPayload(containerRecord);
        
        // Convert back to string for demonstration
        return Encoding.UTF8.GetString(payload);
    }
}