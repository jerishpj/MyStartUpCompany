using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;
using MyStartUpCompany.Persistence.Entities.Enums;
using MyStartUpCompany.Persistence.Repositories;
using MyStartUpCompany.Worker.Tests.Utilities;

namespace MyStartUpCompany.Worker.Tests.Repositories
{
    /// <summary>
    /// Tests for the ProjectRepository upsert functionality.
    /// </summary>
    public class ProjectRepositoryUpsertTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly IProjectRepository _repository;

        public ProjectRepositoryUpsertTests()
        {
            var uniqueDbName = Guid.NewGuid().ToString();
            _dbContext = TestDataFactory.CreateInMemoryAppDbContext(uniqueDbName);
            _repository = new ProjectRepository(_dbContext);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        [Fact]
        public async Task UpsertAsync_WithNewProject_ShouldInsertRecord()
        {
            // Arrange
            var project = new Project
            {
                ProjectIdentifier = "PROJ-2024-001",
                Name = "Cloud Migration",
                Code = "CM",
                Location = "San Francisco",
                Type = ProjectType.CloudService
            };

            // Act
            var result = await _repository.UpsertAsync(project);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.ProjectIdentifier.Should().Be("PROJ-2024-001");
        }

        [Fact]
        public async Task UpsertAsync_WithExistingProjectByIdentifier_ShouldUpdateRecord()
        {
            // Arrange
            var originalProject = new Project
            {
                ProjectIdentifier = "PROJ-2024-002",
                Name = "Original Name",
                Code = "ON",
                Location = "Boston",
                Type = ProjectType.CustomerSupport
            };

            await _repository.UpsertAsync(originalProject);
            var originalId = originalProject.Id;

            var updatedProject = new Project
            {
                ProjectIdentifier = "PROJ-2024-002",
                Name = "Updated Name",
                Code = "UN",
                Location = "New York",
                Type = ProjectType.CustomerSupport
            };

            // Act
            var result = await _repository.UpsertAsync(updatedProject);

            // Assert
            result.Id.Should().Be(originalId);
            result.Name.Should().Be("Updated Name");
        }

        [Fact]
        public async Task UpsertAsync_WithDifferentIdentifier_ShouldInsertNewRecord()
        {
            // Arrange
            var project1 = new Project
            {
                ProjectIdentifier = "PROJ-2024-003",
                Name = "Project Alpha",
                Code = "PA",
                Location = "SF",
                Type = ProjectType.CloudService
            };

            var project2 = new Project
            {
                ProjectIdentifier = "PROJ-2024-004",
                Name = "Project Beta",
                Code = "PB",
                Location = "LA",
                Type = ProjectType.CloudService
            };

            // Act
            await _repository.UpsertAsync(project1);
            var result2 = await _repository.UpsertAsync(project2);

            // Assert
            project1.Id.Should().NotBe(result2.Id);
        }
    }
}
