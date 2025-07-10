using DataProcessor.Abstractions;
using Moq;
using System.Text;

namespace DataProcessor.Tests;

public class DataProcessorServiceTests
{
    [Fact]
    public void GetTcpPayload_ValidDataRecord_ReturnsCorrectByteArray()
    {
        // Arrange
        var expectedMessage = "1,TestName,123.45";
        var expectedBytes = Encoding.UTF8.GetBytes(expectedMessage);
        
        var mockMessageBuilder = new Mock<IMessageBuilder<DataRecord>>();
        mockMessageBuilder.Setup(m => m.BuildMessage(It.IsAny<DataRecord>()))
            .Returns(expectedMessage);
            
        var mockFileCreator = new Mock<IFileCreator<DataRecord>>();
        
        var processor = new DataProcessorService(mockMessageBuilder.Object, mockFileCreator.Object);
        
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestName",
            Value = 123.45m
        };
        
        // Act
        var result = processor.GetTcpPayload(dataRecord);
        
        // Assert
        Assert.Equal(expectedBytes, result);
        mockMessageBuilder.Verify(m => m.BuildMessage(dataRecord), Times.Once);
    }
    
    [Fact]
    public void GetTcpPayload_NullDataRecord_ThrowsArgumentNullException()
    {
        // Arrange
        var mockMessageBuilder = new Mock<IMessageBuilder<DataRecord>>();
        var mockFileCreator = new Mock<IFileCreator<DataRecord>>();
        
        var processor = new DataProcessorService(mockMessageBuilder.Object, mockFileCreator.Object);
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => processor.GetTcpPayload(null!));
    }
    
    [Fact]
    public void WriteDataFile_ValidParameters_CallsFileCreator()
    {
        // Arrange
        var mockMessageBuilder = new Mock<IMessageBuilder<DataRecord>>();
        var mockFileCreator = new Mock<IFileCreator<DataRecord>>();
        
        var processor = new DataProcessorService(mockMessageBuilder.Object, mockFileCreator.Object);
        
        var testData = new List<DataRecord>
        {
            new() { Id = 1, Name = "Item1", Value = 10.5m },
            new() { Id = 2, Name = "Item2", Value = 20.75m }
        };
        
        var filePath = "test.csv";
        
        // Act
        processor.WriteDataFile(testData, filePath);
        
        // Assert
        mockFileCreator.Verify(c => c.CreateFile(testData, filePath), Times.Once);
    }
    
    [Fact]
    public void WriteDataFile_NullRecords_ThrowsArgumentNullException()
    {
        // Arrange
        var mockMessageBuilder = new Mock<IMessageBuilder<DataRecord>>();
        var mockFileCreator = new Mock<IFileCreator<DataRecord>>();
        
        var processor = new DataProcessorService(mockMessageBuilder.Object, mockFileCreator.Object);
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => processor.WriteDataFile(null!, "test.csv"));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void WriteDataFile_InvalidPath_ThrowsArgumentException(string invalidPath)
    {
        // Arrange
        var mockMessageBuilder = new Mock<IMessageBuilder<DataRecord>>();
        var mockFileCreator = new Mock<IFileCreator<DataRecord>>();
        
        var processor = new DataProcessorService(mockMessageBuilder.Object, mockFileCreator.Object);
        
        var testData = new List<DataRecord>
        {
            new() { Id = 1, Name = "Item1", Value = 10.5m }
        };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => processor.WriteDataFile(testData, invalidPath));
    }
}