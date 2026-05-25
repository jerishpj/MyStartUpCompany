using MyStartUpCompany.Worker.Handlers.AddCompany;

namespace MyStartUpCompany.Worker.Mappers
{
    /// <summary>
    /// Factory interface for resolving the appropriate mapper based on message source.
    /// Implements the Factory Pattern to abstract the creation and selection of mappers.
    /// 
    /// This allows the message processing logic to be source-agnostic.
    /// The factory handles the complexity of determining which mapper to use.
    /// </summary>
    public interface IMapperFactory
    {
        /// <summary>
        /// Gets the appropriate mapper for the given source and message type.
        /// </summary>
        /// <param name="source">The identifier of the message source (e.g., "SourceA", "SourceB")</param>
        /// <param name="messageJson">Optional: JSON string of the message for inspection (for diagnostics)</param>
        /// <returns>A mapper instance that can transform the source message to CompanyInputDto, or null if source is not supported</returns>
        /// <remarks>
        /// Implementations should:
        /// - Return null if the source is not recognized (safe fallback)
        /// - Support case-insensitive source matching
        /// - Log when an unsupported source is requested
        /// </remarks>
        IMessageMapper<object>? GetMapper(string source, string? messageJson = null);

        /// <summary>
        /// Maps a raw message object to CompanyInputDto using the appropriate source mapper.
        /// This is a convenience method that combines mapper resolution and execution.
        /// </summary>
        /// <param name="source">The identifier of the message source</param>
        /// <param name="rawMessage">The raw message object from the source</param>
        /// <returns>Mapped CompanyInputDto, or null if mapping fails or source is unsupported</returns>
        CompanyInputDto? MapMessage(string source, object rawMessage);
    }
}
