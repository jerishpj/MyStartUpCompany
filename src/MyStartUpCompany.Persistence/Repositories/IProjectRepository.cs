using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Repository interface for Project entity operations including upsert functionality.
    /// Business key: ProjectIdentifier (globally unique across system)
    /// </summary>
    public interface IProjectRepository : IRepository<Project>
    {
        /// <summary>
        /// Upserts a project record. If a project with the same identifier exists, it updates the record.
        /// Otherwise, it inserts a new record.
        /// </summary>
        /// <param name="project">The project to upsert</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The upserted project entity with its Id populated</returns>
        Task<Project> UpsertAsync(Project project, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a project by its project identifier.
        /// </summary>
        /// <param name="projectIdentifier">The unique project identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The project if found, otherwise null</returns>
        Task<Project?> FindByProjectIdentifierAsync(string projectIdentifier, CancellationToken cancellationToken = default);
    }
}
