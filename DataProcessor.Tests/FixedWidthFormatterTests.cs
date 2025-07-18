using System.Globalization;
using DataProcessor.Formatting;

namespace DataProcessor.Tests;

public class FixedWidthFormatterTests
{
    [Fact]
    public void FormatContainerStatus_FormatsAccordingToSpecification()
    {
        // Arrange
        var formatter = new FixedWidthFormatter(2, RoundingMode.Truncate);
        var record = new ContainerStatusRecord
        {
            StatusType = "CONTAINERSTATUS",
            Barcode = "317164239",
            Status = "SCANNED",
            DimensionType = "DIMS",
            Length = 2.90f,
            Width = 2.80f,
            Height = 16.40f,
            Volume = 131.31098f,
            Weight = 0.08f
        };
        
        // Act
        var result = formatter.FormatContainerStatus(record, CultureInfo.InvariantCulture);
        
        // Assert - should match the example from the text file
        // Weight(10) Volume(9) Barcode(12) Blank(9) Length(10) Width(10) Height(10)
        Assert.Equal("      0.08   131.31317164239         2.90      2.80     16.40", result);
    }
    
    [Fact]
    public void FormatContainerStatus_WithNullRecord_ThrowsArgumentNullException()
    {
        // Arrange
        var formatter = new FixedWidthFormatter(2, RoundingMode.Truncate);
        
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => formatter.FormatContainerStatus(null!));
        Assert.Equal("record", ex.ParamName);
    }
    
    [Fact]
    public void FormatRecord_FormatsCorrectly()
    {
        // Arrange
        var formatter = new FixedWidthFormatter(2, RoundingMode.Truncate);
        var record = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 123.456m
        };
        
        var widthMap = new Dictionary<string, int>
        {
            { "Id", 5 },
            { "Name", 10 },
            { "Value", 8 }
        };
        
        // Act
        var result = formatter.FormatRecord(record, widthMap, CultureInfo.InvariantCulture);
        
        // Assert
        Assert.Equal("    1  TestItem  123.45", result);
    }
    
    [Fact]
    public void FormatFlexibleRecord_FormatsCorrectly()
    {
        // Arrange
        var formatter = new FixedWidthFormatter(2, RoundingMode.Truncate);
        var record = new FlexibleRecord();
        record.SetValue("Weight", 0.08f);
        record.SetValue("Volume", 131.31098f);
        record.SetValue("Barcode", "317164239");
        record.SetValue("Length", 2.90f);
        record.SetValue("Width", 2.80f);
        record.SetValue("Height", 16.40f);
        
        var widthMap = new Dictionary<string, int>
        {
            { "Weight", 10 },
            { "Volume", 9 },
            { "Barcode", 12 },
            { "Blank", 9 },
            { "Length", 10 },
            { "Width", 10 },
            { "Height", 10 }
        };
        
        var fieldOrder = new[] { "Weight", "Volume", "Barcode", "Blank", "Length", "Width", "Height" };
        
        // Act
        var result = formatter.FormatFlexibleRecord(record, fieldOrder, widthMap, CultureInfo.InvariantCulture);
        
        // Assert
        Assert.Equal("      0.08   131.31317164239         2.90      2.80     16.40", result);
    }
}