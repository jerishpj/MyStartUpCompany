using System.Diagnostics.CodeAnalysis;

namespace MyStartUpCompany.Api.Shared.Constants;

/// <summary>
/// Constants for API validation rules.
/// Centralized here to ensure consistency across the application and easy modification.
/// Excluded from code coverage as it contains only constants with no executable logic.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ValidationConstants
{
    /// <summary>
    /// Pagination constraints to prevent resource exhaustion and improve performance.
    /// </summary>
    public static class Pagination
    {
        /// <summary>
        /// Minimum allowed page number (1-based indexing).
        /// </summary>
        public const int MinPageNumber = 1;

        /// <summary>
        /// Maximum allowed page number to prevent excessively large skip values.
        /// </summary>
        public const int MaxPageNumber = 10000;

        /// <summary>
        /// Minimum items per page.
        /// </summary>
        public const int MinPageSize = 1;

        /// <summary>
        /// Maximum items per page.
        /// Prevents DoS attacks by limiting memory allocation and database load.
        /// </summary>
        public const int MaxPageSize = 100;

        /// <summary>
        /// Default items per page when not specified.
        /// </summary>
        public const int DefaultPageSize = 10;

        /// <summary>
        /// Error message template for page number validation.
        /// </summary>
        public static string PageNumberErrorMessage => 
            $"PageNumber must be between {MinPageNumber} and {MaxPageNumber}";

        /// <summary>
        /// Error message template for page size validation.
        /// </summary>
        public static string PageSizeErrorMessage =>
            $"PageSize must be between {MinPageSize} and {MaxPageSize}";
    }

    /// <summary>
    /// Search/filter constraints.
    /// </summary>
    public static class Search
    {
        /// <summary>
        /// Maximum length for search terms to prevent excessive pattern matching.
        /// </summary>
        public const int MaxSearchTermLength = 500;

        /// <summary>
        /// Minimum length for meaningful search terms.
        /// </summary>
        public const int MinSearchTermLength = 1;

        /// <summary>
        /// Maximum length for filter fields (city, country, region, etc.).
        /// </summary>
        public const int MaxFilterFieldLength = 100;
    }

    /// <summary>
    /// Database operation constraints.
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// Maximum command timeout in seconds for database operations.
        /// Prevents long-running queries from locking resources indefinitely.
        /// </summary>
        public const int CommandTimeoutSeconds = 30;
    }
}
