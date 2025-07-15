using System.Globalization;
using DataProcessor.Formatting;
using DataProcessor.Implementations;

namespace DataProcessor.Tests;

public class ContainerStatusMessageBuilderTests
{
    [Fact]
    public void BuildMessage_ValidContainerStatusRecord_ReturnsFormattedString()
    {
        // Arrange
        var messageBuilder = new ContainerStatusMessageBuilder();
        var record = new ContainerStatusRecord
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
        
        // Act
        var result = messageBuilder.BuildMessage(record);
        
        // Assert - Default formatter truncates decimal places
        Assert.Equal("CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|57910|13", result);
    }
    
    [Fact]
    public void BuildMessage_CustomDelimiter_UsesProvidedDelimiter()
    {
        // Arrange
        var formatter = new DecimalFormatter(0, RoundingMode.Truncate);
        var messageBuilder = new ContainerStatusMessageBuilder(",", formatter, CultureInfo.InvariantCulture);
        var record = new ContainerStatusRecord
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
        
        // Act
        var result = messageBuilder.BuildMessage(record);
        
        // Assert
        Assert.Equal("CONTAINERSTATUS,317164239,SCANNED,DIMS,44,35,38,57910,13", result);
    }
    
    [Fact]
    public void BuildMessage_NullRecord_ThrowsArgumentNullException()
    {
        // Arrange
        var messageBuilder = new ContainerStatusMessageBuilder();
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => messageBuilder.BuildMessage(null!));
    }
    
    [Theory]
    [InlineData(RoundingMode.Truncate, "44|35|38|57910|13")]
    [InlineData(RoundingMode.RoundUp, "45|36|39|57911|14")]
    [InlineData(RoundingMode.RoundDown, "44|35|38|57910|13")]
    public void BuildMessage_DifferentRoundingModes_FormatsCorrectly(RoundingMode mode, string expectedValues)
    {
        // Arrange
        var formatter = new DecimalFormatter(0, mode);
        var messageBuilder = new ContainerStatusMessageBuilder("|", formatter, CultureInfo.InvariantCulture);
        var record = new ContainerStatusRecord
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
        
        // Act
        var result = messageBuilder.BuildMessage(record);
        
        // Assert
        var expectedResult = $"CONTAINERSTATUS|317164239|SCANNED|DIMS|{expectedValues}";
        Assert.Equal(expectedResult, result);
    }
    
    [Fact]
    public void BuildMessage_ExampleContainerStatus_ProducesExpectedFormat()
    {
        // Arrange - Example from the requirements
        var formatter = new DecimalFormatter(0, RoundingMode.Truncate);
        var messageBuilder = new ContainerStatusMessageBuilder("|", formatter, CultureInfo.InvariantCulture);
        var record = new ContainerStatusRecord(
            statusType: "CONTAINERSTATUS",
            barcode: "317164239",
            status: "SCANNED",
            dimensionType: "DIMS",
            length: 44f,
            width: 35f,
            height: 38f,
            volume: 57910f,
            weight: 13f
        );
        
        // Act
        var result = messageBuilder.BuildMessage(record);
        
        // Assert
        Assert.Equal("CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|57910|13", result);
    }
}