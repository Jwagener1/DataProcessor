using System.Globalization;
using System.Text;
using DataProcessor.Formatting;

namespace DataProcessor.IntegrationTests;

public class FixedWidthIntegrationTest
{
    [Fact]
    public void FixedWidthFormatter_IntegratesWithContainerProcessor()
    {
        // Arrange - Create a processor for fixed-width formatting
        var processor = new FixedWidthContainerProcessorService(
            decimalPlaces: 2,
            roundingMode: RoundingMode.Truncate,
            culture: CultureInfo.InvariantCulture);
        
        // Create a record with the test values from the example file
        var record = processor.CreateContainerStatus(
            barcode: "317164239",
            length: 2.90f,
            width: 2.80f,
            height: 16.40f,
            weight: 0.08f
        );
        
        // Override the volume which would normally be calculated
        record.Volume = 131.31098f;
        
        // Act - Get the formatted message
        var formattedMessage = processor.GetFormattedMessage(record);
        
        // Assert - Check against expected format
        // Format: Weight(10) Volume(9) Barcode(12) Blank(9) Length(10) Width(10) Height(10)
        Assert.Equal("      0.08   131.31317164239         2.90      2.80     16.40", formattedMessage);
        
        // Act - Get TCP payload
        var payload = processor.GetTcpPayload(record);
        
        // Assert - Check the payload matches expected output
        var expectedPayloadString = "      0.08   131.31317164239         2.90      2.80     16.40";
        var expectedPayload = Encoding.UTF8.GetBytes(expectedPayloadString);
        Assert.Equal(expectedPayload, payload);
    }
}