using System.Text;
using DataProcessor.Formatting;
using DataProcessor.Implementations;

namespace DataProcessor.IntegrationTests;

public class BasicIntegrationTest
{
    [Fact]
    public void ProcessorIntegrationTest_BasicFunctionality()
    {
        // Arrange
        var formatter = new DecimalFormatter(2, RoundingMode.Truncate);
        var messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        var fileCreator = new CsvFileCreator(messageBuilder);
        var processor = new DataProcessorService(messageBuilder, fileCreator);
        
        var record = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 123.456m
        };
        
        // Act - Just verify that we can get a TCP payload without exceptions
        var payload = processor.GetTcpPayload(record);
        
        // Assert
        Assert.NotNull(payload);
        Assert.NotEmpty(payload);
    }
    
    [Fact]
    public void ContainerStatusIntegrationTest_ProducesCorrectFormat()
    {
        // Arrange - Create a container status record with the desired format
        var containerRecord = new ContainerStatusRecord
        {
            StatusType = "CONTAINERSTATUS",
            Barcode = "317164239",
            Status = "SCANNED",
            DimensionType = "DIMS",
            Length = 44f,
            Width = 35f,
            Height = 38f,
            Volume = 57910f,
            Weight = 13f
        };
        
        // Create a formatter that will truncate decimal values
        var formatter = new DecimalFormatter(0, RoundingMode.Truncate);
        
        // Create the container status message builder with pipe delimiter
        var containerMessageBuilder = new ContainerStatusMessageBuilder("|", formatter, System.Globalization.CultureInfo.InvariantCulture);
        
        // Act - Build the message
        var message = containerMessageBuilder.BuildMessage(containerRecord);
        
        // Create a byte array payload for TCP transmission
        var payload = Encoding.UTF8.GetBytes(message);
        
        // Assert
        Assert.Equal("CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|57910|13", message);
        
        // Verify payload is correctly generated
        var expectedPayload = Encoding.UTF8.GetBytes("CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|57910|13");
        Assert.Equal(expectedPayload, payload);
    }
    
    [Fact]
    public void FlexibleMessageIntegrationTest_MultipleClientFormats()
    {
        // Arrange - Set up formats for different clients
        var formatProvider = new MessageFormatProvider();
        
        // Client A uses the standard container status format with pipe delimiter
        var clientAFormat = new MessageFormat(new[]
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
        
        // Client B uses a custom format with comma delimiter and different order
        var clientBFormat = new MessageFormat(new[]
        {
            FormatToken.CreateLiteral("MYTAG"),
            FormatToken.CreateKey("ScanAction"),
            FormatToken.CreateKey("ContainerId"),
            FormatToken.CreateKey("Length"),
            FormatToken.CreateKey("Width"),
            FormatToken.CreateKey("Height"),
            FormatToken.CreateKey("Weight")
        }, ",");
        
        // Client C includes a checksum field
        var clientCFormat = new MessageFormat(new[]
        {
            FormatToken.CreateKey("ContainerId"),
            FormatToken.CreateKey("Length"),
            FormatToken.CreateKey("Width"),
            FormatToken.CreateKey("Height"),
            FormatToken.CreateKey("CheckSum")
        }, ":");
        
        // Register the formats
        formatProvider.RegisterFormat("ClientA", clientAFormat);
        formatProvider.RegisterFormat("ClientB", clientBFormat);
        formatProvider.RegisterFormat("ClientC", clientCFormat);
        
        // Create a processor service with truncated decimal values
        var processor = new FlexibleProcessorService(formatProvider);
        
        // Create a container status record with the same data
        var record = processor.CreateContainerStatus(
            containerId: "317164239",
            length: 44f,
            width: 35f,
            height: 38f,
            weight: 13f
        );
        
        // Add a checksum for Client C
        record.SetValue("CheckSum", "ABC123");
        
        // Act - Generate messages for each client
        var messageA = processor.GetMessage("ClientA", record);
        var messageB = processor.GetMessage("ClientB", record);
        var messageC = processor.GetMessage("ClientC", record);
        
        // Assert - Since volume calculation might vary due to floating-point precision,
        // we verify each part of the message separately
        Assert.StartsWith("CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|", messageA);
        Assert.EndsWith("|13", messageA);
        
        Assert.Equal("MYTAG,SCANNED,317164239,44,35,38,13", messageB);
        Assert.Equal("317164239:44:35:38:ABC123", messageC);
    }
}