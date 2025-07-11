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
}