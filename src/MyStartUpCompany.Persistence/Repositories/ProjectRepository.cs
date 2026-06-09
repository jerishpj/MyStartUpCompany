using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for Project entity with upsert functionality.
    /// Business key: ProjectIdentifier (globally unique across system)
    /// Implements the Industry-standard Upsert Pattern:
    /// - If a record matching the unique key (ProjectIdentifier) exists, update it
    /// - If no matching record exists, insert a new one
    /// </summary>
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Project?> FindByProjectIdentifierAsync(string projectIdentifier, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(p => p.ProjectIdentifier == projectIdentifier, cancellationToken);
        }

        public async Task<Project> UpsertAsync(Project project, CancellationToken cancellationToken = default)
        {
            // Find existing project by identifier (business key)
            var existingProject = await FindByProjectIdentifierAsync(project.ProjectIdentifier, cancellationToken);

            if (existingProject != null)
            {
                // UPDATE scenario
                existingProject.Name = project.Name;
                existingProject.Code = project.Code;
                existingProject.Location = project.Location;
                existingProject.Type = project.Type;
                existingProject.Details = project.Details;
                existingProject.UpdatedAt = DateTime.UtcNow;

                Update(existingProject);
                await SaveChangesAsync(cancellationToken);

                return existingProject;
            }

            // INSERT scenario
            await AddAsync(project, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            return project;
        }
    }
}
