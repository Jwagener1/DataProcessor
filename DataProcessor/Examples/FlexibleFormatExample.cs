using System.Text;
using DataProcessor.Formatting;
using DataProcessor.Implementations;

namespace DataProcessor.Examples;

/// <summary>
/// Example usage of the flexible message format system
/// </summary>
public static class FlexibleFormatExample
{
    /// <summary>
    /// Demonstrates how to set up and use client-specific message formats
    /// </summary>
    public static void DemonstrateClientFormats()
    {
        // Step 1: Create a message format provider to store client-specific formats
        var formatProvider = new MessageFormatProvider();
        
        // Step 2: Define formats for different clients
        
        // Example for "Client A" - Standard container status format with pipe delimiter
        var clientAFormat = new MessageFormat(new[]
        {
            FormatToken.CreateLiteral("CONTAINERSTATUS"),  // constant literal value
            FormatToken.CreateKey("ContainerId"),          // value from data dictionary
            FormatToken.CreateKey("ScanAction"),
            FormatToken.CreateKey("Format"),
            FormatToken.CreateKey("Length"),
            FormatToken.CreateKey("Width"),
            FormatToken.CreateKey("Height"),
            FormatToken.CreateKey("Volume"),
            FormatToken.CreateKey("Weight")
        }, "|"); // pipe delimiter
        
        // Another client might prefer a different order or extra tokens
        var clientBFormat = new MessageFormat(new[]
        {
            FormatToken.CreateLiteral("MYTAG"),
            FormatToken.CreateKey("ScanAction"),
            FormatToken.CreateKey("ContainerId"),
            FormatToken.CreateKey("Length"),
            FormatToken.CreateKey("Width"),
            FormatToken.CreateKey("Height"),
            FormatToken.CreateKey("Weight")
        }, ","); // comma delimiter
        
        // Step 3: Register the formats with the provider
        formatProvider.RegisterFormat("ClientA", clientAFormat);
        formatProvider.RegisterFormat("ClientB", clientBFormat);
        
        // Step 4: Create a processor service
        var processor = new FlexibleProcessorService(formatProvider);
        
        // Step 5: Create and populate a data record
        // Option 1: Use the helper method for container status records
        var containerRecord = processor.CreateContainerStatus(
            containerId: "317164239",
            scanAction: "SCANNED",
            format: "DIMS",
            length: 44,
            width: 35,
            height: 38,
            weight: 13
        );
        
        // Option 2: Create and populate a record manually
        var customRecord = new FlexibleRecord();
        customRecord.SetValue("ContainerId", "317164239");
        customRecord.SetValue("ScanAction", "PROCESSED");
        customRecord.SetValue("Format", "DIMS");
        customRecord.SetValue("Length", 44);
        customRecord.SetValue("Width", 35);
        customRecord.SetValue("Height", 38);
        customRecord.SetValue("Volume", 44 * 35 * 38);
        customRecord.SetValue("Weight", 13);
        customRecord.SetValue("CustomField", "Extra information");
        
        // Step 6: Generate messages for different clients using the same data
        string clientAMessage = processor.GetMessage("ClientA", containerRecord);
        string clientBMessage = processor.GetMessage("ClientB", containerRecord);
        
        // Output (for demonstration):
        Console.WriteLine($"Client A Message: {clientAMessage}");
        // Result: Client A Message: CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|58520|13
        
        Console.WriteLine($"Client B Message: {clientBMessage}");
        // Result: Client B Message: MYTAG,SCANNED,317164239,44,35,38,13
        
        // Step 7: Generate TCP payloads for network transmission
        byte[] clientAPayload = processor.GetTcpPayload("ClientA", containerRecord);
        
        // Convert back to string for demonstration
        string payloadAsString = Encoding.UTF8.GetString(clientAPayload);
        Console.WriteLine($"Client A TCP Payload (as string): {payloadAsString}");
        // Result: Client A TCP Payload (as string): CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|58520|13
    }
    
    /// <summary>
    /// Creates the example message format requested in the initial requirements
    /// </summary>
    /// <returns>The formatted message string</returns>
    public static string CreateExampleContainerStatusMessage()
    {
        // Define the message format with tokens
        var format = new MessageFormat(new[]
        {
            FormatToken.CreateLiteral("CONTAINERSTATUS"),
            FormatToken.CreateKey("ContainerId"),
            FormatToken.CreateKey("ScanAction"),
            FormatToken.CreateKey("Format"),
            FormatToken.CreateKey("Length"),
            FormatToken.CreateKey("Width"),
            FormatToken.CreateKey("Height"),
            FormatToken.CreateKey("Volume"),
            FormatToken.CreateKey("Weight")
        }, "|");
        
        // Create the message builder
        var messageBuilder = new FlexibleMessageBuilder(format, new DecimalFormatter(0, RoundingMode.Truncate));
        
        // Create and populate a data record
        var record = new FlexibleRecord();
        record.SetValue("ContainerId", "317164239");
        record.SetValue("ScanAction", "SCANNED");
        record.SetValue("Format", "DIMS");
        record.SetValue("Length", 44f);
        record.SetValue("Width", 35f);
        record.SetValue("Height", 38f);
        record.SetValue("Volume", 57910f);
        record.SetValue("Weight", 13f);
        
        // Build the message
        return messageBuilder.BuildMessage(record);
        // Result: "CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|57910|13"
    }
}