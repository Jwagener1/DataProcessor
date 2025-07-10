using System.Globalization;
using System.Text;
using DataProcessor.Abstractions;
using DataProcessor.Formatting;
using DataProcessor.Implementations;

namespace DataProcessor.Tests;

public class FormattingIntegrationTests : IDisposable
{
    private readonly string _tempFilePath;
    
    public FormattingIntegrationTests()
    {
        // Create a temporary file path for testing
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}.csv");
    }
    
    public void Dispose()
    {
        // Clean up after tests
        if (File.Exists(_tempFilePath))
        {
            File.Delete(_tempFilePath);
        }
    }
    
    [Fact]
    public void ProcessorWithTruncateFormatter_WritesCorrectlyFormattedData()
    {
        // Arrange
        var formatter = new DecimalFormatter(0, RoundingMode.Truncate);
        IMessageBuilder<DataRecord> messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        IFileCreator<DataRecord> fileCreator = new CsvFileCreator(messageBuilder);
        
        var processor = new DataProcessorService(messageBuilder, fileCreator);
        
        var testData = new List<DataRecord>
        {
            new() { Id = 1, Name = "Item1", Value = 10.9m }, // Should truncate to 10
            new() { Id = 2, Name = "Item2", Value = 33.999m } // Should truncate to 33
        };
        
        // Act
        processor.WriteDataFile(testData, _tempFilePath);
        
        // Assert
        Assert.True(File.Exists(_tempFilePath));
        
        var fileContent = File.ReadAllText(_tempFilePath);
        var lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        
        Assert.Equal(3, lines.Length); // Header + 2 data rows
        Assert.Equal("Id,Name,Value", lines[0]);
        Assert.Equal("1,Item1,10", lines[1]);
        Assert.Equal("2,Item2,33", lines[2]);
    }
    
    [Fact]
    public void ProcessorWithRoundUpFormatter_WritesCorrectlyFormattedData()
    {
        // Arrange
        var formatter = new DecimalFormatter(0, RoundingMode.RoundUp);
        IMessageBuilder<DataRecord> messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        IFileCreator<DataRecord> fileCreator = new CsvFileCreator(messageBuilder);
        
        var processor = new DataProcessorService(messageBuilder, fileCreator);
        
        var testData = new List<DataRecord>
        {
            new() { Id = 1, Name = "Item1", Value = 10.1m }, // Should round up to 11
            new() { Id = 2, Name = "Item2", Value = 33.933m } // Should round up to 34
        };
        
        // Act
        processor.WriteDataFile(testData, _tempFilePath);
        
        // Assert
        Assert.True(File.Exists(_tempFilePath));
        
        var fileContent = File.ReadAllText(_tempFilePath);
        var lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        
        Assert.Equal(3, lines.Length); // Header + 2 data rows
        Assert.Equal("Id,Name,Value", lines[0]);
        Assert.Equal("1,Item1,11", lines[1]);
        Assert.Equal("2,Item2,34", lines[2]);
    }
    
    [Fact]
    public void ProcessorWithRoundDownFormatter_WritesCorrectlyFormattedData()
    {
        // Arrange
        var formatter = new DecimalFormatter(0, RoundingMode.RoundDown);
        IMessageBuilder<DataRecord> messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        IFileCreator<DataRecord> fileCreator = new CsvFileCreator(messageBuilder);
        
        var processor = new DataProcessorService(messageBuilder, fileCreator);
        
        var testData = new List<DataRecord>
        {
            new() { Id = 1, Name = "Item1", Value = 10.9m }, // Should round down to 10
            new() { Id = 2, Name = "Item2", Value = 33.933m } // Should round down to 33
        };
        
        // Act
        processor.WriteDataFile(testData, _tempFilePath);
        
        // Assert
        Assert.True(File.Exists(_tempFilePath));
        
        var fileContent = File.ReadAllText(_tempFilePath);
        var lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        
        Assert.Equal(3, lines.Length); // Header + 2 data rows
        Assert.Equal("Id,Name,Value", lines[0]);
        Assert.Equal("1,Item1,10", lines[1]);
        Assert.Equal("2,Item2,33", lines[2]);
    }
    
    [Fact]
    public void ProcessorWithDecimalPlacesFormatter_WritesCorrectlyFormattedData()
    {
        // Arrange
        var formatter = new DecimalFormatter(2, RoundingMode.Truncate);
        IMessageBuilder<DataRecord> messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        IFileCreator<DataRecord> fileCreator = new CsvFileCreator(messageBuilder);
        
        var processor = new DataProcessorService(messageBuilder, fileCreator);
        
        var testData = new List<DataRecord>
        {
            new() { Id = 1, Name = "Item1", Value = 10.999m }, // Should format to 10.99
            new() { Id = 2, Name = "Item2", Value = 33.9m } // Should format to 33.90
        };
        
        // Act
        processor.WriteDataFile(testData, _tempFilePath);
        
        // Assert
        Assert.True(File.Exists(_tempFilePath));
        
        var fileContent = File.ReadAllText(_tempFilePath);
        var lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        
        Assert.Equal(3, lines.Length); // Header + 2 data rows
        Assert.Equal("Id,Name,Value", lines[0]);
        Assert.Equal("1,Item1,10.99", lines[1]);
        Assert.Equal("2,Item2,33.90", lines[2]);
    }
    
    [Fact]
    public void ProcessorWithFormatter_GetsTcpPayload_FormattsCorrectly()
    {
        // Arrange
        var formatter = new DecimalFormatter(0, RoundingMode.Truncate);
        IMessageBuilder<DataRecord> messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        IFileCreator<DataRecord> fileCreator = new CsvFileCreator(messageBuilder);
        
        var processor = new DataProcessorService(messageBuilder, fileCreator);
        
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 123.999m // Should format to 123
        };
        
        // Act
        var payload = processor.GetTcpPayload(dataRecord);
        
        // Assert
        var expectedBytes = Encoding.UTF8.GetBytes("1,TestItem,123");
        Assert.Equal(expectedBytes, payload);
    }
}