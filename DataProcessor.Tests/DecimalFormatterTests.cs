using System.Globalization;
using DataProcessor.Formatting;

namespace DataProcessor.Tests;

public class DecimalFormatterTests
{
    private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

    [Theory]
    [InlineData(0, "33")]
    [InlineData(1, "33.9")]
    [InlineData(2, "33.93")]
    [InlineData(3, "33.933")]
    public void Format_Truncate_CorrectlyFormatsValue(int decimalPlaces, string expected)
    {
        // Arrange
        var formatter = new DecimalFormatter(decimalPlaces, RoundingMode.Truncate);
        decimal value = 33.9334m;
        
        // Act
        var result = formatter.Format(value, _culture);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(0, "34")]
    [InlineData(1, "34.0")]
    [InlineData(2, "33.94")]
    [InlineData(3, "33.934")]
    public void Format_RoundUp_CorrectlyFormatsValue(int decimalPlaces, string expected)
    {
        // Arrange
        var formatter = new DecimalFormatter(decimalPlaces, RoundingMode.RoundUp);
        decimal value = 33.9334m;
        
        // Act
        var result = formatter.Format(value, _culture);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(0, "33")]
    [InlineData(1, "33.9")]
    [InlineData(2, "33.93")]
    [InlineData(3, "33.933")]
    public void Format_RoundDown_CorrectlyFormatsValue(int decimalPlaces, string expected)
    {
        // Arrange
        var formatter = new DecimalFormatter(decimalPlaces, RoundingMode.RoundDown);
        decimal value = 33.9334m;
        
        // Act
        var result = formatter.Format(value, _culture);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Constructor_InvalidDecimalPlaces_ThrowsArgumentOutOfRangeException(int decimalPlaces)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            new DecimalFormatter(decimalPlaces, RoundingMode.Truncate));
    }
    
    [Fact]
    public void Format_NullCulture_ThrowsArgumentNullException()
    {
        // Arrange
        var formatter = new DecimalFormatter(2, RoundingMode.Truncate);
        decimal value = 33.9334m;
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => formatter.Format(value, null!));
    }
    
    [Theory]
    [InlineData(0, RoundingMode.Truncate, "0")]
    [InlineData(0, RoundingMode.RoundUp, "1")]
    [InlineData(0, RoundingMode.RoundDown, "0")]
    public void Format_NegativeValuesNearZero_HandlesCorrectly(int decimalPlaces, RoundingMode mode, string expected)
    {
        // Arrange
        var formatter = new DecimalFormatter(decimalPlaces, mode);
        decimal value = -0.3m;
        
        // Act
        var result = formatter.Format(value, _culture);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(0, "34")]
    [InlineData(2, "33.94")]
    public void Format_SpecificRequirementExample_RoundUp(int decimalPlaces, string expected)
    {
        // Arrange - example from requirements: 33.933 (rndup) -> 34
        var formatter = new DecimalFormatter(decimalPlaces, RoundingMode.RoundUp);
        decimal value = 33.933m;
        
        // Act
        var result = formatter.Format(value, _culture);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(0, "33")]
    [InlineData(2, "33.93")]
    public void Format_SpecificRequirementExample_RoundDown(int decimalPlaces, string expected)
    {
        // Arrange - example from requirements: 33.933 (rnddwn) -> 33
        var formatter = new DecimalFormatter(decimalPlaces, RoundingMode.RoundDown);
        decimal value = 33.933m;
        
        // Act
        var result = formatter.Format(value, _culture);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(0, "33")]
    [InlineData(2, "33.93")]
    public void Format_SpecificRequirementExample_Truncate(int decimalPlaces, string expected)
    {
        // Arrange - example from requirements: 33.999 (trim) -> 33
        var formatter = new DecimalFormatter(decimalPlaces, RoundingMode.Truncate);
        decimal value = 33.999m;
        
        // Act
        var result = formatter.Format(value, _culture);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(0, RoundingMode.Truncate, "0")]
    [InlineData(1, RoundingMode.Truncate, "0.0")]
    [InlineData(2, RoundingMode.Truncate, "0.00")]
    [InlineData(3, RoundingMode.Truncate, "0.000")]
    public void Format_ZeroValue_FormatsCorrectly(int decimalPlaces, RoundingMode mode, string expected)
    {
        // Arrange
        var formatter = new DecimalFormatter(decimalPlaces, mode);
        decimal value = 0.0m;
        
        // Act
        var result = formatter.Format(value, _culture);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(0, RoundingMode.Truncate, "-33")]
    [InlineData(1, RoundingMode.Truncate, "-33.9")]
    [InlineData(2, RoundingMode.Truncate, "-33.93")]
    public void Format_NegativeValues_FormatsCorrectly(int decimalPlaces, RoundingMode mode, string expected)
    {
        // Arrange
        var formatter = new DecimalFormatter(decimalPlaces, mode);
        decimal value = -33.938m;
        
        // Act
        var result = formatter.Format(value, _culture);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(0, RoundingMode.RoundUp, "-33")]
    [InlineData(1, RoundingMode.RoundUp, "-33.9")]
    [InlineData(2, RoundingMode.RoundUp, "-33.93")]
    public void Format_NegativeValues_RoundUp_RoundsTowardZero(int decimalPlaces, RoundingMode mode, string expected)
    {
        // Arrange
        var formatter = new DecimalFormatter(decimalPlaces, mode);
        decimal value = -33.938m;
        
        // Act
        var result = formatter.Format(value, _culture);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(0, RoundingMode.RoundDown, "-34")]
    [InlineData(1, RoundingMode.RoundDown, "-34.0")]
    [InlineData(2, RoundingMode.RoundDown, "-33.94")]
    public void Format_NegativeValues_RoundDown_RoundsAwayFromZero(int decimalPlaces, RoundingMode mode, string expected)
    {
        // Arrange
        var formatter = new DecimalFormatter(decimalPlaces, mode);
        decimal value = -33.938m;
        
        // Act
        var result = formatter.Format(value, _culture);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void Format_DifferentCulture_UsesCorrectDecimalSeparator()
    {
        // Arrange
        var formatter = new DecimalFormatter(2, RoundingMode.Truncate);
        decimal value = 123.45m;
        var germanCulture = new CultureInfo("de-DE"); // Uses comma as decimal separator
        
        // Act
        var result = formatter.Format(value, germanCulture);
        
        // Assert
        Assert.Equal("123,45", result);
    }
    
    [Fact]
    public void Format_ExtremeLargeValues_HandledCorrectly()
    {
        // Arrange
        var formatter = new DecimalFormatter(0, RoundingMode.Truncate);
        
        // Act & Assert
        // Just verify it doesn't throw an exception for large values
        var result1 = formatter.Format(decimal.MaxValue, _culture);
        Assert.NotNull(result1);
        
        var result2 = formatter.Format(decimal.MinValue, _culture);
        Assert.NotNull(result2);
    }
    
    [Fact]
    public void ApplyRounding_ReturnsCorrectProperties()
    {
        // Arrange
        int decimalPlaces = 2;
        var roundingMode = RoundingMode.Truncate;
        var formatter = new DecimalFormatter(decimalPlaces, roundingMode);
        
        // Act & Assert
        Assert.Equal(decimalPlaces, formatter.DecimalPlaces);
        Assert.Equal(roundingMode, formatter.RoundingMode);
    }
}