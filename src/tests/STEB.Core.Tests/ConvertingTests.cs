using STEB2.Converting;
using Xunit;

namespace STEB.Core.Tests;

/// <summary>Converting.TextToFloat: culture-independent digit-by-digit parser of the original program.</summary>
public class ConvertingTests
{
    [Theory]
    [InlineData("12,5", 12.5f)]
    [InlineData("12.5", 12.5f)]
    [InlineData("-3,25", -3.25f)]
    [InlineData("  7 ", 7f)]
    [InlineData("0", 0f)]
    public void TextToFloat_ParsesBothDecimalSeparators(string text, float expected)
    {
        Assert.Equal(expected, STEB2.Converting.Converting.TextToFloat(text), 4);
    }

    [Fact]
    public void TextToFloat_EmptyText_IsZero() => Assert.Equal(0f, STEB2.Converting.Converting.TextToFloat(""));

    [Fact]
    public void TextToFloat_NullKeyword_IsNaN() => Assert.True(float.IsNaN(STEB2.Converting.Converting.TextToFloat("null")));

    [Fact]
    public void TextToFloat_ExponentNotation_IsZero_AsInTheOriginal()
    {
        // Original behaviour: any 'E' after the first character gives 0.
        Assert.Equal(0f, STEB2.Converting.Converting.TextToFloat("1E-05"));
    }

    [Fact]
    public void TextToFloat_NonNumericText_IsZero() => Assert.Equal(0f, STEB2.Converting.Converting.TextToFloat("abc"));
}
