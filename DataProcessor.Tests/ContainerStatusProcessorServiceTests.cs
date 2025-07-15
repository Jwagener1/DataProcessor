using System.Text;
using DataProcessor.Abstractions;
using DataProcessor.Formatting;
using DataProcessor.Implementations;
using Moq;

namespace DataProcessor.Tests;

public class ContainerStatusProcessorServiceTests
{
    [Fact]
    public void GetTcpPayload_ValidContainerStatusRecord_ReturnsCorrectByteArray()
    {
        // Arrange
        var expectedMessage = "CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|57910|13";
        var expectedBytes = Encoding.UTF8.GetBytes(expectedMessage);
        
        var mockMessageBuilder = new Mock<IMessageBuilder<ContainerStatusRecord>>();
        mockMessageBuilder.Setup(m => m.BuildMessage(It.IsAny<ContainerStatusRecord>()))
            .Returns(expectedMessage);
            
        var processor = new ContainerStatusProcessorService(mockMessageBuilder.Object);
        
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
        
        // Act
        var result = processor.GetTcpPayload(containerRecord);
        
        // Assert
        Assert.Equal(expectedBytes, result);
        mockMessageBuilder.Verify(m => m.BuildMessage(containerRecord), Times.Once);
    }
    
    [Fact]
    public void GetTcpPayload_NullContainerStatusRecord_ThrowsArgumentNullException()
    {
        // Arrange
        var mockMessageBuilder = new Mock<IMessageBuilder<ContainerStatusRecord>>();
        var processor = new ContainerStatusProcessorService(mockMessageBuilder.Object);
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => processor.GetTcpPayload(null!));
    }
    
    [Fact]
    public void CreateContainerStatus_ValidInputs_CreatesCorrectRecord()
    {
        // Arrange
        var formatter = new DecimalFormatter(0, RoundingMode.Truncate);
        var messageBuilder = new ContainerStatusMessageBuilder("|", formatter, System.Globalization.CultureInfo.InvariantCulture);
        var processor = new ContainerStatusProcessorService(messageBuilder);
        
        string barcode = "317164239";
        float length = 44f;
        float width = 35f;
        float height = 38f;
        float weight = 13f;
        
        // Act
        var result = processor.CreateContainerStatus(barcode, length, width, height, weight);
        
        // Assert
        Assert.Equal("CONTAINERSTATUS", result.StatusType);
        Assert.Equal(barcode, result.Barcode);
        Assert.Equal("SCANNED", result.Status);
        Assert.Equal("DIMS", result.DimensionType);
        Assert.Equal(length, result.Length);
        Assert.Equal(width, result.Width);
        Assert.Equal(height, result.Height);
        Assert.Equal(length * width * height, result.Volume);
        Assert.Equal(weight, result.Weight);
        
        // Verify that the message is built correctly
        var message = messageBuilder.BuildMessage(result);
        Assert.Equal("CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|58520|13", message);
    }
}