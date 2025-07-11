using System.Globalization;
using DataProcessor.Formatting;
using DataProcessor.Implementations;

namespace DataProcessor.Tests;

public class FixedDelimiterMessageBuilderTests
{
    [Fact]
    public void BuildMessage_ValidDataRecord_ReturnsFormattedString()
    {
        // Arrange
        var messageBuilder = new FixedDelimiterMessageBuilder();
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 123.45m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        Assert.Equal("1,TestItem,123.45", result);
    }
    
    [Fact]
    public void BuildMessage_CustomDelimiter_UsesProvidedDelimiter()
    {
        // Arrange
        var messageBuilder = new FixedDelimiterMessageBuilder("|");
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 123.45m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        Assert.Equal("1|TestItem|123.45", result);
    }
    
    [Fact]
    public void BuildMessage_NullDataRecord_ThrowsArgumentNullException()
    {
        // Arrange
        var messageBuilder = new FixedDelimiterMessageBuilder();
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => messageBuilder.BuildMessage(null!));
    }
    
    [Fact]
    public void BuildMessage_WithDecimalFormatter_FormatsValueAccordingly()
    {
        // Arrange
        var formatter = new DecimalFormatter(1, RoundingMode.Truncate);
        var messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 123.456m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        Assert.Equal("1,TestItem,123.4", result);
    }
    
    [Theory]
    [InlineData(0, RoundingMode.Truncate, "1,TestItem,123")]
    [InlineData(0, RoundingMode.RoundUp, "1,TestItem,124")]
    [InlineData(0, RoundingMode.RoundDown, "1,TestItem,123")]
    [InlineData(2, RoundingMode.Truncate, "1,TestItem,123.45")]
    [InlineData(2, RoundingMode.RoundUp, "1,TestItem,123.46")]
    [InlineData(2, RoundingMode.RoundDown, "1,TestItem,123.45")]
    [InlineData(3, RoundingMode.Truncate, "1,TestItem,123.456")]
    public void BuildMessage_DifferentFormattingOptions_FormatsCorrectly(int decimalPlaces, RoundingMode mode, string expected)
    {
        // Arrange
        var formatter = new DecimalFormatter(decimalPlaces, mode);
        var messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 123.4567m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void BuildMessage_CustomCulture_UsesProvidedCulture()
    {
        // Arrange
        var culture = new CultureInfo("de-DE"); // Uses comma as decimal separator
        var formatter = new DecimalFormatter(2, RoundingMode.Truncate);
        var messageBuilder = new FixedDelimiterMessageBuilder(";", formatter, culture);
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 123.45m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        Assert.Equal("1;TestItem;123,45", result); // Note the comma as decimal separator
    }
    
    [Fact]
    public void BuildMessage_NullDelimiter_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FixedDelimiterMessageBuilder(null!));
    }
    
    [Fact] 
    public void BuildMessage_NullCulture_ThrowsArgumentNullException()
    {
        // Act & Assert
        var formatter = new DecimalFormatter(2, RoundingMode.Truncate);
        Assert.Throws<ArgumentNullException>(() => 
            new FixedDelimiterMessageBuilder(",", formatter, null!));
    }
    
    [Fact]
    public void BuildMessage_ZeroValue_FormatsCorrectly()
    {
        // Arrange
        var formatter = new DecimalFormatter(2, RoundingMode.Truncate);
        var messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 0.0m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        Assert.Equal("1,TestItem,0.00", result);
    }
    
    [Fact]
    public void BuildMessage_NegativeValue_FormatsCorrectly()
    {
        // Arrange
        var formatter = new DecimalFormatter(2, RoundingMode.Truncate);
        var messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = -123.456m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        Assert.Equal("1,TestItem,-123.45", result);
    }
    
    [Theory]
    [InlineData("", "Empty name")]
    [InlineData(" ", "Whitespace name")]
    [InlineData("Name,With,Commas", "Name with commas")]
    [InlineData("Name\"With\"Quotes", "Name with quotes")]
    public void BuildMessage_SpecialNameCharacters_HandlesCorrectly(string name, string testName)
    {
        // Arrange - Using the testName parameter for logging/documentation of the test case
        System.Console.WriteLine($"Testing special name format: {testName}");
        
        var messageBuilder = new FixedDelimiterMessageBuilder();
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = name,
            Value = 123.45m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        Assert.Equal($"1,{name},123.45", result);
    }
    
    [Fact]
    public void BuildMessage_SpecificRequirementExample_Truncate()
    {
        // Arrange - example from requirements: 33.999 (trim) -> 33
        var formatter = new DecimalFormatter(0, RoundingMode.Truncate);
        var messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 33.999m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        Assert.Equal("1,TestItem,33", result);
    }
    
    [Fact]
    public void BuildMessage_SpecificRequirementExample_RoundUp()
    {
        // Arrange - example from requirements: 33.933 (rndup) -> 34
        var formatter = new DecimalFormatter(0, RoundingMode.RoundUp);
        var messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 33.933m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        Assert.Equal("1,TestItem,34", result);
    }
    
    [Fact]
    public void BuildMessage_SpecificRequirementExample_RoundDown()
    {
        // Arrange - example from requirements: 33.933 (rnddwn) -> 33
        var formatter = new DecimalFormatter(0, RoundingMode.RoundDown);
        var messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 33.933m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        Assert.Equal("1,TestItem,33", result);
    }
    
    [Fact]
    public void BuildMessage_DefaultConstructor_UsesInvariantCulture()
    {
        // Arrange
        var messageBuilder = new FixedDelimiterMessageBuilder();
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "TestItem",
            Value = 123.45m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        // Instead of using Contains, verify the entire string
        Assert.Equal("1,TestItem,123.45", result);
    }
    
    [Fact]
    public void BuildMessage_EmptyString_FormatsCorrectly()
    {
        // Arrange
        var messageBuilder = new FixedDelimiterMessageBuilder(",");
        var dataRecord = new DataRecord
        {
            Id = 1,
            Name = "",
            Value = 123.45m
        };
        
        // Act
        var result = messageBuilder.BuildMessage(dataRecord);
        
        // Assert
        Assert.Equal("1,,123.45", result); // Note the double delimiter for empty name
    }
}