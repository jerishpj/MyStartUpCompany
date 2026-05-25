using MyStartUpCompany.Worker.Handlers.AddCompany;
using MyStartUpCompany.Worker.Mappers.Sources;

namespace MyStartUpCompany.Worker.Mappers
{
    /// <summary>
    /// Concrete implementation of IMapperFactory that manages mapper registration and resolution.
    /// Uses a dictionary-based approach for O(1) mapper lookups based on source identifier.
    /// </summary>
    public class MapperFactory : IMapperFactory
    {
        private readonly Dictionary<string, IMessageMapper<object>> _mappers;
        private readonly ILogger<MapperFactory> _logger;

        /// <summary>
        /// Initializes the factory with a service provider for lazy resolution of mappers.
        /// </summary>
        /// <param name="serviceProvider">DI service provider to resolve mappers</param>
        /// <param name="logger">Logger for diagnostics</param>
        public MapperFactory(IServiceProvider serviceProvider, ILogger<MapperFactory> logger)
        {
            _logger = logger;
            _mappers = new Dictionary<string, IMessageMapper<object>>(StringComparer.OrdinalIgnoreCase);

            // Resolve all registered mappers from DI container
            InitializeMappers(serviceProvider);
        }

        /// <summary>
        /// Dynamically discovers and registers all IMessageMapper implementations from the DI container.
        /// </summary>
        /// <remarks>
        /// This method resolves known mapper implementations from the service provider.
        /// </remarks>
        private void InitializeMappers(IServiceProvider serviceProvider)
        {
            try
            {
                // Try to resolve known mapper implementations
                // These are registered explicitly in MapperExtensions

                // Register SourceA mapper
                try
                {
                    var sourceAMapper = serviceProvider.GetService(typeof(SourceAMapper)) as SourceAMapper;
                    if (sourceAMapper != null)
                    {
                        _mappers[MessageSources.SourceA] = sourceAMapper;
                        _logger.LogInformation("Registered mapper for source: {Source}", MessageSources.SourceA);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to register SourceA mapper");
                }

                // Register SourceB mapper
                try
                {
                    var sourceBMapper = serviceProvider.GetService(typeof(SourceBMapper)) as SourceBMapper;
                    if (sourceBMapper != null)
                    {
                        _mappers[MessageSources.SourceB] = sourceBMapper;
                        _logger.LogInformation("Registered mapper for source: {Source}", MessageSources.SourceB);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to register SourceB mapper");
                }

                // Register SourceC mapper
                try
                {
                    var sourceCMapper = serviceProvider.GetService(typeof(SourceCMapper)) as SourceCMapper;
                    if (sourceCMapper != null)
                    {
                        _mappers[MessageSources.SourceC] = sourceCMapper;
                        _logger.LogInformation("Registered mapper for source: {Source}", MessageSources.SourceC);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to register SourceC mapper");
                }

                if (_mappers.Count == 0)
                {
                    _logger.LogWarning("No mappers were registered. Messages may not be properly mapped.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing mappers from DI container");
            }
        }

        /// <summary>
        /// Registers a mapper for a specific source.
        /// </summary>
        /// <param name="source">Source identifier (case-insensitive)</param>
        /// <param name="mapper">The mapper instance</param>
        public void RegisterMapper(string source, IMessageMapper<object> mapper)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("Source cannot be null or empty", nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            _mappers[source] = mapper;
            _logger.LogInformation("Registered mapper for source: {Source}", source);
        }

        public IMessageMapper<object>? GetMapper(string source, string? messageJson = null)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                _logger.LogWarning("Mapper requested with null or empty source");
                return null;
            }

            if (_mappers.TryGetValue(source, out var mapper))
            {
                _logger.LogDebug("Found mapper for source: {Source}", source);
                return mapper;
            }

            _logger.LogWarning(
                "No mapper found for source: {Source}. Available sources: {AvailableSources}",
                source,
                string.Join(", ", _mappers.Keys));

            return null;
        }

        public CompanyInputDto? MapMessage(string source, object rawMessage)
        {
            try
            {
                var mapper = GetMapper(source);
                if (mapper == null)
                {
                    _logger.LogWarning("Cannot map message: no mapper found for source {Source}", source);
                    return null;
                }

                var mapped = mapper.Map(rawMessage);

                if (mapped != null)
                {
                    _logger.LogDebug("Successfully mapped message from source: {Source}", source);
                }
                else
                {
                    _logger.LogWarning("Mapper returned null for source: {Source}", source);
                }

                return mapped;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping message from source: {Source}", source);
                return null;
            }
        }
    }

    /// <summary>
    /// Registry interface for storing mapper registrations that can be queried by the factory.
    /// This allows mappers to be registered in the DI container and discovered at runtime.
    /// </summary>
    public interface IMapperRegistry
    {
        /// <summary>
        /// Gets all registered mappers with their source identifiers.
        /// </summary>
        IEnumerable<(string source, IMessageMapper<object> mapper)> GetMappers();
    }

    /// <summary>
    /// Concrete implementation of mapper registry that stores mappers in a dictionary.
    /// </summary>
    public class MapperRegistry : IMapperRegistry
    {
        private readonly Dictionary<string, IMessageMapper<object>> _mappers;

        public MapperRegistry()
        {
            _mappers = new Dictionary<string, IMessageMapper<object>>(StringComparer.OrdinalIgnoreCase);
        }

        public void Register(string source, IMessageMapper<object> mapper)
        {
            _mappers[source] = mapper;
        }

        public IEnumerable<(string source, IMessageMapper<object> mapper)> GetMappers()
        {
            return _mappers.Select(kvp => (kvp.Key, kvp.Value));
        }
    }
}
