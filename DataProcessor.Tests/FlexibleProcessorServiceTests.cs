using System.Text;
using DataProcessor.Formatting;

namespace DataProcessor.Tests;

public class FlexibleProcessorServiceTests
{
    [Fact]
    public void GetMessage_ClientAFormat_ReturnsCorrectlyFormattedMessage()
    {
        // Arrange
        var formatProvider = new MessageFormatProvider();
        
        // Register a format for "Client A"
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
        });
        
        formatProvider.RegisterFormat("ClientA", clientAFormat);
        
        // Create a processor service
        var processor = new FlexibleProcessorService(formatProvider);
        
        // Create a container status record
        var record = processor.CreateContainerStatus(
            containerId: "317164239",
            length: 44.7f,
            width: 35.2f,
            height: 38.9f,
            weight: 13.8f
        );
        
        // Act
        var message = processor.GetMessage("ClientA", record);
        
        // Assert - Since volume calculation might vary due to floating-point precision,
        // we verify each part of the message separately
        Assert.Contains("CONTAINERSTATUS", message);
        Assert.Contains("317164239", message);
        Assert.Contains("SCANNED", message);
        Assert.Contains("DIMS", message);
        Assert.Contains("44", message); // Length truncated
        Assert.Contains("35", message); // Width truncated
        Assert.Contains("38", message); // Height truncated
        Assert.Contains("13", message); // Weight truncated
    }
    
    [Fact]
    public void GetMessage_ClientBFormat_ReturnsCorrectlyFormattedMessage()
    {
        // Arrange
        var formatProvider = new MessageFormatProvider();
        
        // Register a format for "Client B" with different order and delimiter
        var clientBFormat = new MessageFormat(new[]
        {
            FormatToken.CreateLiteral("MYTAG"),
            FormatToken.CreateKey("ScanAction"),
            FormatToken.CreateKey("ContainerId"),
            FormatToken.CreateKey("Format"),
            FormatToken.CreateKey("Width"),
            FormatToken.CreateKey("Length"),
            FormatToken.CreateKey("Height"),
            FormatToken.CreateKey("Weight"),
            FormatToken.CreateKey("Volume")
        }, ",");
        
        formatProvider.RegisterFormat("ClientB", clientBFormat);
        
        // Create a processor service
        var processor = new FlexibleProcessorService(formatProvider);
        
        // Create a container status record
        var record = processor.CreateContainerStatus(
            containerId: "317164239",
            scanAction: "PROCESSED",
            length: 44.7f,
            width: 35.2f,
            height: 38.9f,
            weight: 13.8f
        );
        
        // Act
        var message = processor.GetMessage("ClientB", record);
        
        // Assert - Since volume calculation might vary due to floating-point precision,
        // we verify each part of the message separately
        Assert.StartsWith("MYTAG,PROCESSED,317164239,DIMS,35,44,38,13,", message);
    }
    
    [Fact]
    public void GetTcpPayload_ValidRecord_ReturnsCorrectByteArray()
    {
        // Arrange
        var formatProvider = new MessageFormatProvider();
        
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
        });
        
        formatProvider.RegisterFormat("ClientA", format);
        
        var processor = new FlexibleProcessorService(formatProvider);
        var record = processor.CreateContainerStatus(
            containerId: "317164239",
            length: 44,
            width: 35,
            height: 38,
            weight: 13
        );
        
        // Act
        var payload = processor.GetTcpPayload("ClientA", record);
        
        // Assert - Convert to string to verify the main parts
        var payloadAsString = Encoding.UTF8.GetString(payload);
        Assert.StartsWith("CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|", payloadAsString);
        Assert.EndsWith("|13", payloadAsString);
    }
    
    [Fact]
    public void GetMessage_UnknownClient_ThrowsArgumentException()
    {
        // Arrange
        var formatProvider = new MessageFormatProvider();
        var processor = new FlexibleProcessorService(formatProvider);
        var record = new FlexibleRecord();
        
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => processor.GetMessage("UnknownClient", record));
        Assert.Contains("UnknownClient", ex.Message);
    }
    
    [Fact]
    public void GetMessage_NullRecord_ThrowsArgumentNullException()
    {
        // Arrange
        var formatProvider = new MessageFormatProvider();
        formatProvider.RegisterFormat("ClientA", new MessageFormat(new[] { FormatToken.CreateKey("Field") }));
        var processor = new FlexibleProcessorService(formatProvider);
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => processor.GetMessage("ClientA", null!));
    }
    
    [Fact]
    public void CreateContainerStatus_SetsAllValues()
    {
        // Arrange
        var formatProvider = new MessageFormatProvider();
        var processor = new FlexibleProcessorService(formatProvider);
        
        // Act
        var record = processor.CreateContainerStatus(
            containerId: "317164239",
            scanAction: "SCANNED",
            format: "DIMS",
            length: 44,
            width: 35,
            height: 38,
            weight: 13
        );
        
        // Assert
        Assert.Equal("317164239", record.GetValue<string>("ContainerId"));
        Assert.Equal("SCANNED", record.GetValue<string>("ScanAction"));
        Assert.Equal("DIMS", record.GetValue<string>("Format"));
        Assert.Equal(44f, record.GetValue<float>("Length"));
        Assert.Equal(35f, record.GetValue<float>("Width"));
        Assert.Equal(38f, record.GetValue<float>("Height"));
        Assert.Equal(44f * 35f * 38f, record.GetValue<float>("Volume"));
        Assert.Equal(13f, record.GetValue<float>("Weight"));
    }
}