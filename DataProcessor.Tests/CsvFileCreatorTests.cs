using DataProcessor.Abstractions;
using DataProcessor.Implementations;
using Moq;
using System.Globalization;
using System.Text;

namespace DataProcessor.Tests;

public class CsvFileCreatorTests : IDisposable
{
    private readonly string _tempFilePath;
    
    public CsvFileCreatorTests()
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
    public void CreateFile_ValidData_CreatesFileWithCorrectContent()
    {
        // Arrange
        var mockMessageBuilder = new Mock<IMessageBuilder<DataRecord>>();
        mockMessageBuilder.Setup(m => m.BuildMessage(It.IsAny<DataRecord>()))
            .Returns<DataRecord>(r => $"{r.Id},{r.Name},{r.Value.ToString(CultureInfo.InvariantCulture)}");
            
        var fileCreator = new CsvFileCreator(mockMessageBuilder.Object);
        
        var testData = new List<DataRecord>
        {
            new() { Id = 1, Name = "Item1", Value = 10.5m },
            new() { Id = 2, Name = "Item2", Value = 20.75m }
        };
        
        // Act
        fileCreator.CreateFile(testData, _tempFilePath);
        
        // Assert
        Assert.True(File.Exists(_tempFilePath));
        
        var fileContent = File.ReadAllText(_tempFilePath);
        var lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        
        Assert.Equal(3, lines.Length); // Header + 2 data rows
        Assert.Equal("Id,Name,Value", lines[0]);
        Assert.Equal("1,Item1,10.5", lines[1]);
        Assert.Equal("2,Item2,20.75", lines[2]);
        
        // Verify message builder was called for each record
        mockMessageBuilder.Verify(m => m.BuildMessage(It.IsAny<DataRecord>()), Times.Exactly(2));
    }
    
    [Fact]
    public void CreateFile_EmptyData_CreatesFileWithOnlyHeader()
    {
        // Arrange
        var mockMessageBuilder = new Mock<IMessageBuilder<DataRecord>>();
        var fileCreator = new CsvFileCreator(mockMessageBuilder.Object);
        
        // Act
        fileCreator.CreateFile(new List<DataRecord>(), _tempFilePath);
        
        // Assert
        Assert.True(File.Exists(_tempFilePath));
        
        var fileContent = File.ReadAllText(_tempFilePath);
        var lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        
        Assert.Single(lines); // Only header
        Assert.Equal("Id,Name,Value", lines[0]);
    }
    
    [Fact]
    public void CreateFile_NullData_ThrowsArgumentNullException()
    {
        // Arrange
        var mockMessageBuilder = new Mock<IMessageBuilder<DataRecord>>();
        var fileCreator = new CsvFileCreator(mockMessageBuilder.Object);
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => fileCreator.CreateFile(null!, _tempFilePath));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateFile_InvalidFilePath_ThrowsArgumentException(string invalidPath)
    {
        // Arrange
        var mockMessageBuilder = new Mock<IMessageBuilder<DataRecord>>();
        var fileCreator = new CsvFileCreator(mockMessageBuilder.Object);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            fileCreator.CreateFile(new List<DataRecord>(), invalidPath));
    }
}