using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Configurations;

/// <summary>
/// Entity configuration for Project with optimized indexing strategy.
/// Indexes are designed for the hybrid storage approach:
/// - Searchable columns (ProjectIdentifier, Code, Location) are indexed individually and in combinations
/// - JSON column is configured for EF Core serialization/deserialization
/// </summary>
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Table configuration
        builder.ToTable("Projects");
        builder.HasKey(p => p.Id);

        // Foreign key to Company
        builder.HasOne(p => p.Company)
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // ========== NORMALIZED COLUMN CONFIGURATIONS ==========

        builder.Property(p => p.ProjectIdentifier)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Unique project identifier, e.g., PROJ-2024-001");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("Project name/title");

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Project code/abbreviation, e.g., DVP, MKT");

        builder.Property(p => p.Location)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Project location/site, e.g., San Francisco, Remote");

        builder.Property(p => p.CompanyId)
            .HasComment("Foreign key to Company");

        // ========== JSON COLUMN CONFIGURATION ==========

        builder.Property(p => p.Details)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                v => System.Text.Json.JsonSerializer.Deserialize<Entities.ValueObjects.ProjectDetails>(v, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Entities.ValueObjects.ProjectDetails { Status = "Planning" })
            .HasComment("Project details stored as JSON: Budget, Status, Dates, Team, Tags, Metrics, Metadata");

        // ========== TIMESTAMP COLUMNS ==========

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasComment("When the project record was created");

        builder.Property(p => p.UpdatedAt)
            .HasComment("When the project record was last updated");

        // ========== PERFORMANCE INDEXES ==========

        // Primary search index - ProjectIdentifier (must be unique for external system integration)
        builder.HasIndex(p => p.ProjectIdentifier)
            .HasDatabaseName("IX_Project_ProjectIdentifier")
            .IsUnique();

        // Code-based filtering index
        builder.HasIndex(p => p.Code)
            .HasDatabaseName("IX_Project_Code");

        // Location-based filtering index
        builder.HasIndex(p => p.Location)
            .HasDatabaseName("IX_Project_Location");

        // Name index for search and sorting
        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Project_Name");

        // Company-based filtering (foreign key index)
        builder.HasIndex(p => p.CompanyId)
            .HasDatabaseName("IX_Project_CompanyId");

        // ========== COMPOSITE INDEXES FOR COMMON QUERIES ==========

        // Composite index for finding projects by company and code (most common query pattern)
        builder.HasIndex(p => new { p.CompanyId, p.Code })
            .HasDatabaseName("IX_Project_CompanyId_Code");

        // Composite index for location and code filtering
        builder.HasIndex(p => new { p.Location, p.Code })
            .HasDatabaseName("IX_Project_Location_Code");

        // Composite index for name and location (for sorted location-based queries)
        builder.HasIndex(p => new { p.Location, p.Name, p.Id })
            .HasDatabaseName("IX_Project_Location_Name_Id");

        // Composite index for company, location, and code (for comprehensive filtering)
        builder.HasIndex(p => new { p.CompanyId, p.Location, p.Code })
            .HasDatabaseName("IX_Project_CompanyId_Location_Code");
    }
}
