using MyStartUpCompany.Worker.Mappers;
using MyStartUpCompany.Worker.Mappers.Sources;

namespace MyStartUpCompany.Worker.Extensions
{
    /// <summary>
    /// Extension methods for registering message mapping services.
    /// This centralizes the configuration for source-specific mappers.
    /// </summary>
    public static class MapperExtensions
    {
        /// <summary>
        /// Adds message mapper services to the dependency injection container.
        /// This includes registering all concrete mappers and the factory.
        /// </summary>
        /// <remarks>
        /// To add a new source mapper:
        /// 1. Create a concrete mapper class implementing IMessageMapper&lt;TSource&gt;
        /// 2. Add registration in this method using RegisterSourceMapper
        /// 3. The factory will automatically discover and use it
        /// </remarks>
        public static IServiceCollection AddMessageMappers(this IServiceCollection services)
        {
            // Register mapper registry first (singleton for lifetime management)
            var mapperRegistry = new MapperRegistry();
            services.AddSingleton<IMapperRegistry>(mapperRegistry);

            // Register all concrete mappers as singletons
            services.AddSingleton(mapperRegistry);

            // Register SourceA mapper
            services.AddSingleton<SourceAMapper>();
            RegisterSourceMapper(services, mapperRegistry, MessageSources.SourceA, 
                sp => (IMessageMapper<object>)(object)sp.GetRequiredService<SourceAMapper>());

            // Register SourceB mapper
            services.AddSingleton<SourceBMapper>();
            RegisterSourceMapper(services, mapperRegistry, MessageSources.SourceB,
                sp => (IMessageMapper<object>)(object)sp.GetRequiredService<SourceBMapper>());

            // Register SourceC mapper
            services.AddSingleton<SourceCMapper>();
            RegisterSourceMapper(services, mapperRegistry, MessageSources.SourceC,
                sp => (IMessageMapper<object>)(object)sp.GetRequiredService<SourceCMapper>());

            // Add the mapper factory (singleton)
            services.AddSingleton<IMapperFactory, MapperFactory>();

            return services;
        }

        /// <summary>
        /// Helper method to register a mapper with the registry.
        /// </summary>
        private static void RegisterSourceMapper(
            IServiceCollection services,
            MapperRegistry registry,
            string source,
            Func<IServiceProvider, IMessageMapper<object>> mapperFactory)
        {
            // We'll populate the registry after all services are registered
            // This is handled by MapperFactory initialization
        }
    }
}
