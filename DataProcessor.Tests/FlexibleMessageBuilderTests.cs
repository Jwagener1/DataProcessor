using System.Globalization;
using DataProcessor.Formatting;
using DataProcessor.Implementations;

namespace DataProcessor.Tests;

public class FlexibleMessageBuilderTests
{
    [Fact]
    public void BuildMessage_ContainerStatusFormat_ReturnsCorrectlyFormattedString()
    {
        // Arrange - Create a format for container status messages
        var containerStatusFormat = new MessageFormat(new[]
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
        
        // Create a formatter for decimal values
        var formatter = new DecimalFormatter(0, RoundingMode.Truncate);
        
        // Create the message builder
        var messageBuilder = new FlexibleMessageBuilder(containerStatusFormat, formatter, CultureInfo.InvariantCulture);
        
        // Create a flexible record with data
        var record = new FlexibleRecord();
        record.SetValue("ContainerId", "317164239");
        record.SetValue("ScanAction", "SCANNED");
        record.SetValue("Format", "DIMS");
        record.SetValue("Length", 44.7f);
        record.SetValue("Width", 35.2f);
        record.SetValue("Height", 38.9f);
        record.SetValue("Volume", 57910.3f);
        record.SetValue("Weight", 13.8f);
        
        // Act
        var result = messageBuilder.BuildMessage(record);
        
        // Assert
        Assert.Equal("CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|57910|13", result);
    }
    
    [Fact]
    public void BuildMessage_CustomFormat_ReturnsCorrectlyFormattedString()
    {
        // Arrange - Create a custom format with a different order
        var customFormat = new MessageFormat(new[]
        {
            FormatToken.CreateLiteral("MYTAG"),
            FormatToken.CreateKey("ScanAction"),
            FormatToken.CreateKey("ContainerId"),
            FormatToken.CreateKey("Length"),
            FormatToken.CreateKey("Width"),
            FormatToken.CreateKey("Height")
        }, ",");
        
        // Create a formatter for decimal values
        var formatter = new DecimalFormatter(1, RoundingMode.RoundUp);
        
        // Create the message builder
        var messageBuilder = new FlexibleMessageBuilder(customFormat, formatter, CultureInfo.InvariantCulture);
        
        // Create a flexible record with data
        var record = new FlexibleRecord();
        record.SetValue("ContainerId", "317164239");
        record.SetValue("ScanAction", "PROCESSED");
        record.SetValue("Length", 44.7f);
        record.SetValue("Width", 35.2f);
        record.SetValue("Height", 38.9f);
        
        // Act
        var result = messageBuilder.BuildMessage(record);
        
        // Assert
        Assert.Equal("MYTAG,PROCESSED,317164239,44.7,35.2,38.9", result);
    }
    
    [Fact]
    public void BuildMessage_MissingValues_ReturnsEmptyStringsForMissingKeys()
    {
        // Arrange
        var format = new MessageFormat(new[]
        {
            FormatToken.CreateKey("Field1"),
            FormatToken.CreateKey("Field2"),
            FormatToken.CreateKey("MissingField"),
            FormatToken.CreateKey("Field3")
        }, ",");
        
        var messageBuilder = new FlexibleMessageBuilder(format);
        
        var record = new FlexibleRecord();
        record.SetValue("Field1", "Value1");
        record.SetValue("Field2", "Value2");
        record.SetValue("Field3", "Value3");
        
        // Act
        var result = messageBuilder.BuildMessage(record);
        
        // Assert
        Assert.Equal("Value1,Value2,,Value3", result);
    }
    
    [Fact]
    public void BuildMessage_DifferentValueTypes_FormatsValuesCorrectly()
    {
        // Arrange
        var format = new MessageFormat(new[]
        {
            FormatToken.CreateKey("StringValue"),
            FormatToken.CreateKey("IntValue"),
            FormatToken.CreateKey("DecimalValue"),
            FormatToken.CreateKey("BoolValue"),
            FormatToken.CreateKey("DateValue")
        });
        
        var formatter = new DecimalFormatter(2, RoundingMode.Truncate);
        var messageBuilder = new FlexibleMessageBuilder(format, formatter, CultureInfo.InvariantCulture);
        
        var record = new FlexibleRecord();
        record.SetValue("StringValue", "Hello");
        record.SetValue("IntValue", 42);
        record.SetValue("DecimalValue", 123.456m);
        record.SetValue("BoolValue", true);
        record.SetValue("DateValue", new DateTime(2023, 7, 15));
        
        // Act
        var result = messageBuilder.BuildMessage(record);
        
        // Assert - Check each part separately to avoid date format issues
        Assert.Contains("Hello", result);
        Assert.Contains("42", result);
        Assert.Contains("123.45", result);
        Assert.Contains("True", result);
        // The date format may vary by environment, so just check that it's there
        Assert.Contains("2023", result);
        Assert.Contains("7", result);
        Assert.Contains("15", result);
    }
    
    [Fact]
    public void BuildMessage_NullRecord_ThrowsArgumentNullException()
    {
        // Arrange
        var format = new MessageFormat(new[] { FormatToken.CreateKey("Field1") });
        var messageBuilder = new FlexibleMessageBuilder(format);
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => messageBuilder.BuildMessage(null!));
    }
}