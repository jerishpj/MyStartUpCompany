using MyStartUpCompany.Worker.Handlers.AddCompany;

namespace MyStartUpCompany.Worker.Mappers
{
    /// <summary>
    /// Generic interface for mapping messages from different sources to CompanyInputDto.
    /// Implements the Strategy Pattern to allow different sources to have different mapping logic.
    /// 
    /// Each source should implement this interface with its own specific transformation logic.
    /// This keeps the mapping logic decoupled from the core business logic and database persistence.
    /// </summary>
    /// <typeparam name="TSource">The source message type to be mapped from</typeparam>
    public interface IMessageMapper<TSource> where TSource : class
    {
        /// <summary>
        /// Maps a source message to CompanyInputDto.
        /// </summary>
        /// <param name="sourceMessage">The raw message from the source system</param>
        /// <returns>Mapped CompanyInputDto, or null if mapping fails</returns>
        /// <remarks>
        /// Implementations should:
        /// - Handle null input gracefully
        /// - Validate required fields
        /// - Transform field values according to source-specific rules
        /// - Log warnings/errors for transformation issues
        /// </remarks>
        CompanyInputDto? Map(TSource sourceMessage);
    }
}
