namespace MyStartUpCompany.Api.Common.Utilities;

/// <summary>
/// Helper class for SQL LIKE pattern escaping and utilities.
/// Provides methods for secure pattern matching in EF.Functions.Like() queries,
/// preventing SQL injection and ensuring correct pattern matching behavior.
/// </summary>
public static class SqlLikeHelper
{
    /// <summary>
    /// Escapes special characters in LIKE patterns to prevent SQL injection
    /// and ensure correct pattern matching.
    /// </summary>
    /// <param name="parameter">The search parameter to escape</param>
    /// <returns>Escaped parameter safe for use in EF.Functions.Like()</returns>
    /// <remarks>
    /// Escapes the following special characters: [ ] % _
    /// These characters have special meaning in SQL LIKE patterns:
    /// - [ and ] are used for character ranges
    /// - % matches any sequence of characters
    /// - _ matches any single character
    /// 
    /// By escaping them, we ensure the user's input is treated literally.
    /// </remarks>
    public static string EscapeLikeParameter(string parameter)
    {
        if (string.IsNullOrEmpty(parameter))
            return parameter;

        return parameter
            .Replace("[", "[[]")
            .Replace("%", "[%]")
            .Replace("_", "[_]");
    }

    /// <summary>
    /// Creates a LIKE pattern for substring matching (wildcard on both sides).
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <returns>Escaped pattern for LIKE query (e.g., "%term%")</returns>
    /// <example>
    /// var pattern = SqlLikeHelper.CreateLikePattern("test");
    /// // Result: "%test%"
    /// var result = companies.Where(c => EF.Functions.Like(c.Name, pattern));
    /// </example>
    public static string CreateLikePattern(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return "%";

        var escaped = EscapeLikeParameter(searchTerm.Trim());
        return $"%{escaped}%";
    }

    /// <summary>
    /// Creates a LIKE pattern for exact prefix matching (wildcard on right side only).
    /// </summary>
    /// <param name="prefix">The prefix to match</param>
    /// <returns>Escaped pattern for LIKE query (e.g., "prefix%")</returns>
    /// <example>
    /// var pattern = SqlLikeHelper.CreatePrefixPattern("ABC");
    /// // Result: "ABC%"
    /// var result = companies.Where(c => EF.Functions.Like(c.Code, pattern));
    /// </example>
    public static string CreatePrefixPattern(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            return "%";

        var escaped = EscapeLikeParameter(prefix.Trim());
        return $"{escaped}%";
    }

    /// <summary>
    /// Creates a LIKE pattern for exact suffix matching (wildcard on left side only).
    /// </summary>
    /// <param name="suffix">The suffix to match</param>
    /// <returns>Escaped pattern for LIKE query (e.g., "%suffix")</returns>
    /// <example>
    /// var pattern = SqlLikeHelper.CreateSuffixPattern("txt");
    /// // Result: "%txt"
    /// var result = files.Where(f => EF.Functions.Like(f.Extension, pattern));
    /// </example>
    public static string CreateSuffixPattern(string suffix)
    {
        if (string.IsNullOrWhiteSpace(suffix))
            return "%";

        var escaped = EscapeLikeParameter(suffix.Trim());
        return $"%{escaped}";
    }

    /// <summary>
    /// Creates a LIKE pattern for exact match (no wildcards).
    /// </summary>
    /// <param name="exactValue">The exact value to match</param>
    /// <returns>Escaped pattern for LIKE query (e.g., "exact")</returns>
    /// <example>
    /// var pattern = SqlLikeHelper.CreateExactPattern("value");
    /// // Result: "value"
    /// var result = companies.Where(c => EF.Functions.Like(c.Code, pattern));
    /// </example>
    public static string CreateExactPattern(string exactValue)
    {
        if (string.IsNullOrWhiteSpace(exactValue))
            return "";

        return EscapeLikeParameter(exactValue.Trim());
    }
}
