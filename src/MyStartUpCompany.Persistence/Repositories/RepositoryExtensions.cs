using Microsoft.Extensions.DependencyInjection;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Extension methods for registering repositories in the dependency injection container.
    /// Provides convenient methods to register all repository implementations at once.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Registers all repositories with the dependency injection container.
        /// This includes repositories for: Company, Employee, Location, Project, Building, Office
        /// </summary>
        /// <param name="services">The service collection to register repositories with</param>
        /// <returns>The updated service collection for method chaining</returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Register generic repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register entity-specific repositories
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddScoped<IOfficeRepository, OfficeRepository>();

            return services;
        }

        /// <summary>
        /// Registers repositories with custom service lifetime.
        /// Useful for testing scenarios where you need transient repositories.
        /// </summary>
        /// <param name="services">The service collection to register repositories with</param>
        /// <param name="lifetime">The service lifetime (Transient, Scoped, or Singleton)</param>
        /// <returns>The updated service collection for method chaining</returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services, ServiceLifetime lifetime)
        {
            // Register generic repository
            services.Add(new ServiceDescriptor(typeof(IRepository<>), typeof(Repository<>), lifetime));

            // Register entity-specific repositories
            services.Add(new ServiceDescriptor(typeof(ICompanyRepository), typeof(CompanyRepository), lifetime));
            services.Add(new ServiceDescriptor(typeof(IEmployeeRepository), typeof(EmployeeRepository), lifetime));
            services.Add(new ServiceDescriptor(typeof(ILocationRepository), typeof(LocationRepository), lifetime));
            services.Add(new ServiceDescriptor(typeof(IProjectRepository), typeof(ProjectRepository), lifetime));
            services.Add(new ServiceDescriptor(typeof(IBuildingRepository), typeof(BuildingRepository), lifetime));
            services.Add(new ServiceDescriptor(typeof(IOfficeRepository), typeof(OfficeRepository), lifetime));

            return services;
        }
    }
}
