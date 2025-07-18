using System.Globalization;
using System.Text;
using DataProcessor.Formatting;

namespace DataProcessor.Examples;

/// <summary>
/// Example usage of the fixed-width format system
/// </summary>
public static class FixedWidthFormatExample
{
    /// <summary>
    /// Demonstrates how to format a container status record with fixed-width formatting
    /// </summary>
    public static void DemonstrateFixedWidthFormatting()
    {
        // Create a container status processor
        var processor = new FixedWidthContainerProcessorService(
            decimalPlaces: 2,
            roundingMode: RoundingMode.Truncate,
            culture: CultureInfo.InvariantCulture);
        
        // Create a container status record
        var record = processor.CreateContainerStatus(
            barcode: "317164239",
            length: 2.90f,
            width: 2.80f,
            height: 16.40f,
            weight: 0.08f
        );
        
        // Get a fixed-width formatted message
        string message = processor.GetFormattedMessage(record);
        
        // Display the message
        Console.WriteLine("Fixed-Width Formatted Message:");
        Console.WriteLine(message);
        
        // Format legend
        Console.WriteLine("\nFormat Legend:");
        Console.WriteLine("Weight(10) Volume(9) Barcode(12) Blank(9) Length(10) Width(10) Height(10)");
        
        // Create a byte array payload for TCP transmission
        byte[] payload = processor.GetTcpPayload(record);
        
        // Display the payload as bytes
        Console.WriteLine("\nTCP Payload (bytes):");
        Console.WriteLine(BitConverter.ToString(payload));
        
        // Display the payload as a string
        Console.WriteLine("\nTCP Payload (string):");
        Console.WriteLine(Encoding.UTF8.GetString(payload));
    }
    
    /// <summary>
    /// Demonstrates how to format a flexible record with fixed-width formatting
    /// </summary>
    public static void DemonstrateFlexibleFixedWidthFormatting()
    {
        // Create a fixed-width formatter
        var formatter = new FixedWidthFormatter(2, RoundingMode.Truncate);
        
        // Create a flexible record
        var record = new FlexibleRecord();
        record.SetValue("Weight", 0.08f);
        record.SetValue("Volume", 131.31f);
        record.SetValue("Barcode", "317164239");
        record.SetValue("Length", 2.90f);
        record.SetValue("Width", 2.80f);
        record.SetValue("Height", 16.40f);
        
        // Define field widths
        var widthMap = new Dictionary<string, int>
        {
            { "Weight", FixedWidthFormatter.DefaultWidths.Weight },
            { "Volume", FixedWidthFormatter.DefaultWidths.Volume },
            { "Barcode", FixedWidthFormatter.DefaultWidths.Barcode },
            { "Blank", FixedWidthFormatter.DefaultWidths.Blank },
            { "Length", FixedWidthFormatter.DefaultWidths.Length },
            { "Width", FixedWidthFormatter.DefaultWidths.Width },
            { "Height", FixedWidthFormatter.DefaultWidths.Height }
        };
        
        // Define field order
        var fieldOrder = new[] { "Weight", "Volume", "Barcode", "Blank", "Length", "Width", "Height" };
        
        // Format the record
        string formatted = formatter.FormatFlexibleRecord(record, fieldOrder, widthMap);
        
        // Display the formatted string
        Console.WriteLine("Flexible Fixed-Width Formatted Record:");
        Console.WriteLine(formatted);
        
        // Format legend
        Console.WriteLine("\nFormat Legend:");
        Console.WriteLine("Weight(10) Volume(9) Barcode(12) Blank(9) Length(10) Width(10) Height(10)");
    }
}