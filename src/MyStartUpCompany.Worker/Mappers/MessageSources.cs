namespace MyStartUpCompany.Worker.Mappers
{
    /// <summary>
    /// Enumeration of supported message sources.
    /// Add new sources here when extending the system with additional source types.
    /// </summary>
    public enum MessageSource
    {
        /// <summary>
        /// Messages from Source A system.
        /// </summary>
        SourceA,

        /// <summary>
        /// Messages from Source B system.
        /// </summary>
        SourceB,

        /// <summary>
        /// Messages from Source C system.
        /// </summary>
        SourceC,

        /// <summary>
        /// Direct CompanyInputDto format (already in target schema).
        /// </summary>
        Direct,

        /// <summary>
        /// Unknown or unspecified source.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Source identifier constants for use in message routing and mapper selection.
    /// These strings match the values used in message headers or message properties.
    /// </summary>
    public static class MessageSources
    {
        /// <summary>
        /// Identifier for Source A system.
        /// </summary>
        public const string SourceA = "SourceA";

        /// <summary>
        /// Identifier for Source B system.
        /// </summary>
        public const string SourceB = "SourceB";

        /// <summary>
        /// Identifier for Source C system.
        /// </summary>
        public const string SourceC = "SourceC";

        /// <summary>
        /// Identifier for direct CompanyInputDto format.
        /// </summary>
        public const string Direct = "Direct";

        /// <summary>
        /// Identifier for unknown sources.
        /// </summary>
        public const string Unknown = "Unknown";

        /// <summary>
        /// Gets all supported source identifiers.
        /// </summary>
        public static IEnumerable<string> GetAllSources()
        {
            return new[]
            {
                SourceA,
                SourceB,
                SourceC,
                Direct
            };
        }

        /// <summary>
        /// Converts MessageSource enum to its string identifier.
        /// </summary>
        public static string ToIdentifier(MessageSource source)
        {
            return source switch
            {
                MessageSource.SourceA => SourceA,
                MessageSource.SourceB => SourceB,
                MessageSource.SourceC => SourceC,
                MessageSource.Direct => Direct,
                _ => Unknown
            };
        }

        /// <summary>
        /// Converts string identifier to MessageSource enum.
        /// </summary>
        public static MessageSource FromIdentifier(string identifier)
        {
            return identifier switch
            {
                SourceA => MessageSource.SourceA,
                SourceB => MessageSource.SourceB,
                SourceC => MessageSource.SourceC,
                Direct => MessageSource.Direct,
                _ => MessageSource.Unknown
            };
        }
    }
}
