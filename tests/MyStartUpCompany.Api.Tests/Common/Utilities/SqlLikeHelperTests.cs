namespace MyStartUpCompany.Api.Tests.Common.Utilities;

using MyStartUpCompany.Api.Common.Utilities;

/// <summary>
/// Unit tests for SqlLikeHelper utility class
/// </summary>
public class SqlLikeHelperTests
{
    #region EscapeLikeParameter Tests

    [Fact]
    public void EscapeLikeParameter_WithNull_ReturnsNull()
    {
        var result = SqlLikeHelper.EscapeLikeParameter(null!);
        Assert.Null(result);
    }

    [Fact]
    public void EscapeLikeParameter_WithEmpty_ReturnsEmpty()
    {
        var result = SqlLikeHelper.EscapeLikeParameter("");
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("simple", "simple")]
    [InlineData("test123", "test123")]
    [InlineData("with space", "with space")]
    public void EscapeLikeParameter_WithNoSpecialCharacters_ReturnsUnchanged(string input, string expected)
    {
        var result = SqlLikeHelper.EscapeLikeParameter(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("[", "[[]")]
    [InlineData("test[123]", "test[[]123]")]
    [InlineData("[test]", "[[]test]")]
    public void EscapeLikeParameter_EscapesBrackets(string input, string expected)
    {
        var result = SqlLikeHelper.EscapeLikeParameter(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("%", "[%]")]
    [InlineData("100%", "100[%]")]
    [InlineData("test%value", "test[%]value")]
    public void EscapeLikeParameter_EscapesPercentSign(string input, string expected)
    {
        var result = SqlLikeHelper.EscapeLikeParameter(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("_", "[_]")]
    [InlineData("test_value", "test[_]value")]
    [InlineData("__", "[_][_]")]
    public void EscapeLikeParameter_EscapesUnderscore(string input, string expected)
    {
        var result = SqlLikeHelper.EscapeLikeParameter(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EscapeLikeParameter_EscapesMultipleSpecialCharacters()
    {
        var input = "test[100]_%";
        var expected = "test[[]100][_][%]";
        var result = SqlLikeHelper.EscapeLikeParameter(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EscapeLikeParameter_HandlesConsecutiveSpecialCharacters()
    {
        var input = "[[[";
        var expected = "[[][[][[]";
        var result = SqlLikeHelper.EscapeLikeParameter(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region CreateLikePattern Tests

    [Fact]
    public void CreateLikePattern_WithNull_ReturnsPercentSign()
    {
        var result = SqlLikeHelper.CreateLikePattern(null!);
        Assert.Equal("%", result);
    }

    [Fact]
    public void CreateLikePattern_WithEmptyString_ReturnsPercentSign()
    {
        var result = SqlLikeHelper.CreateLikePattern("");
        Assert.Equal("%", result);
    }

    [Fact]
    public void CreateLikePattern_WithWhitespaceOnly_ReturnsPercentSign()
    {
        var result = SqlLikeHelper.CreateLikePattern("   ");
        Assert.Equal("%", result);
    }

    [Theory]
    [InlineData("test", "%test%")]
    [InlineData("search", "%search%")]
    [InlineData("a", "%a%")]
    public void CreateLikePattern_WithSimpleText_ReturnsSurroundedByPercent(string input, string expected)
    {
        var result = SqlLikeHelper.CreateLikePattern(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("  test  ", "%test%")]
    [InlineData("\ttest\t", "%test%")]
    [InlineData("  a  ", "%a%")]
    public void CreateLikePattern_TrimsWhitespace(string input, string expected)
    {
        var result = SqlLikeHelper.CreateLikePattern(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("test%", "%test[%]%")]
    [InlineData("test_value", "%test[_]value%")]
    [InlineData("[test]", "%[[]test]%")]
    public void CreateLikePattern_EscapesSpecialCharacters(string input, string expected)
    {
        var result = SqlLikeHelper.CreateLikePattern(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CreateLikePattern_WithComplexText_HandlesAllEscapes()
    {
        var input = "test[100]_%";
        var expected = "%test[[]100][_][%]%";
        var result = SqlLikeHelper.CreateLikePattern(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region CreatePrefixPattern Tests

    [Fact]
    public void CreatePrefixPattern_WithNull_ReturnsPercentSign()
    {
        var result = SqlLikeHelper.CreatePrefixPattern(null!);
        Assert.Equal("%", result);
    }

    [Fact]
    public void CreatePrefixPattern_WithEmpty_ReturnsPercentSign()
    {
        var result = SqlLikeHelper.CreatePrefixPattern("");
        Assert.Equal("%", result);
    }

    [Theory]
    [InlineData("ABC", "ABC%")]
    [InlineData("prefix", "prefix%")]
    [InlineData("T", "T%")]
    public void CreatePrefixPattern_WithSimpleText_HasPercentOnRight(string input, string expected)
    {
        var result = SqlLikeHelper.CreatePrefixPattern(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("  ABC  ", "ABC%")]
    [InlineData("\tpre\t", "pre%")]
    public void CreatePrefixPattern_TrimsWhitespace(string input, string expected)
    {
        var result = SqlLikeHelper.CreatePrefixPattern(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("ABC%", "ABC[%]%")]
    [InlineData("pre_fix", "pre[_]fix%")]
    [InlineData("[ABC]", "[[]ABC]%")]
    public void CreatePrefixPattern_EscapesSpecialCharacters(string input, string expected)
    {
        var result = SqlLikeHelper.CreatePrefixPattern(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region CreateSuffixPattern Tests

    [Fact]
    public void CreateSuffixPattern_WithNull_ReturnsPercentSign()
    {
        var result = SqlLikeHelper.CreateSuffixPattern(null!);
        Assert.Equal("%", result);
    }

    [Fact]
    public void CreateSuffixPattern_WithEmpty_ReturnsPercentSign()
    {
        var result = SqlLikeHelper.CreateSuffixPattern("");
        Assert.Equal("%", result);
    }

    [Theory]
    [InlineData("txt", "%txt")]
    [InlineData("suffix", "%suffix")]
    [InlineData("X", "%X")]
    public void CreateSuffixPattern_WithSimpleText_HasPercentOnLeft(string input, string expected)
    {
        var result = SqlLikeHelper.CreateSuffixPattern(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("  txt  ", "%txt")]
    [InlineData("\tsuf\t", "%suf")]
    public void CreateSuffixPattern_TrimsWhitespace(string input, string expected)
    {
        var result = SqlLikeHelper.CreateSuffixPattern(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("txt%", "%txt[%]")]
    [InlineData("suf_fix", "%suf[_]fix")]
    [InlineData("[TXT]", "%[[]TXT]")]
    public void CreateSuffixPattern_EscapesSpecialCharacters(string input, string expected)
    {
        var result = SqlLikeHelper.CreateSuffixPattern(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region CreateExactPattern Tests

    [Fact]
    public void CreateExactPattern_WithNull_ReturnsEmpty()
    {
        var result = SqlLikeHelper.CreateExactPattern(null!);
        Assert.Empty(result);
    }

    [Fact]
    public void CreateExactPattern_WithEmpty_ReturnsEmpty()
    {
        var result = SqlLikeHelper.CreateExactPattern("");
        Assert.Empty(result);
    }

    [Fact]
    public void CreateExactPattern_WithWhitespaceOnly_ReturnsEmpty()
    {
        var result = SqlLikeHelper.CreateExactPattern("   ");
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("exact", "exact")]
    [InlineData("value", "value")]
    [InlineData("X", "X")]
    public void CreateExactPattern_WithSimpleText_ReturnsWithoutWildcards(string input, string expected)
    {
        var result = SqlLikeHelper.CreateExactPattern(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("  exact  ", "exact")]
    [InlineData("\tval\t", "val")]
    public void CreateExactPattern_TrimsWhitespace(string input, string expected)
    {
        var result = SqlLikeHelper.CreateExactPattern(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("exact%", "exact[%]")]
    [InlineData("val_ue", "val[_]ue")]
    [InlineData("[exact]", "[[]exact]")]
    public void CreateExactPattern_EscapesSpecialCharacters(string input, string expected)
    {
        var result = SqlLikeHelper.CreateExactPattern(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CreateExactPattern_WithComplexText_HandlesAllEscapes()
    {
        var input = "exact[100]_%";
        var expected = "exact[[]100][_][%]";
        var result = SqlLikeHelper.CreateExactPattern(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void AllMethods_ProduceDifferentPatterns_ForSameInput()
    {
        const string input = "test";

        var likePattern = SqlLikeHelper.CreateLikePattern(input);
        var prefixPattern = SqlLikeHelper.CreatePrefixPattern(input);
        var suffixPattern = SqlLikeHelper.CreateSuffixPattern(input);
        var exactPattern = SqlLikeHelper.CreateExactPattern(input);

        Assert.Equal("%test%", likePattern);
        Assert.Equal("test%", prefixPattern);
        Assert.Equal("%test", suffixPattern);
        Assert.Equal("test", exactPattern);

        // Verify all are different
        Assert.NotEqual(likePattern, prefixPattern);
        Assert.NotEqual(likePattern, suffixPattern);
        Assert.NotEqual(likePattern, exactPattern);
        Assert.NotEqual(prefixPattern, suffixPattern);
        Assert.NotEqual(prefixPattern, exactPattern);
        Assert.NotEqual(suffixPattern, exactPattern);
    }

    [Fact]
    public void SecurityTest_SqlInjectionAttempt_IsEscaped()
    {
        const string injection = "'; DROP TABLE companies; --";

        var result = SqlLikeHelper.EscapeLikeParameter(injection);

        // Should escape the characters that would have special meaning in LIKE
        Assert.DoesNotContain("%", result.Replace("[%]", ""));
        Assert.DoesNotContain("_", result.Replace("[_]", ""));

        // The pattern should be safe for use in queries
        var pattern = SqlLikeHelper.CreateLikePattern(injection);
        Assert.NotNull(pattern);
        Assert.NotEmpty(pattern);
    }

    [Fact]
    public void UniversalCharacterTest_HandlesVariousInputs()
    {
        var inputs = new[]
        {
            "simple",
            "with spaces",
            "123-456",
            "special@chars",
            "Unicode™",
            "café",
            "日本語"
        };

        foreach (var input in inputs)
        {
            // All methods should complete without throwing exceptions
            var _ = SqlLikeHelper.EscapeLikeParameter(input);
            var __ = SqlLikeHelper.CreateLikePattern(input);
            var ___ = SqlLikeHelper.CreatePrefixPattern(input);
            var ____ = SqlLikeHelper.CreateSuffixPattern(input);
            var _____ = SqlLikeHelper.CreateExactPattern(input);
        }
    }

    #endregion
}
