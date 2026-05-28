using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Configurations;

/// <summary>
/// EF Core configuration for ProjectTypeReference entity
/// Defines the schema, constraints, and seeds all valid project types
/// </summary>
public class ProjectTypeReferenceConfiguration : IEntityTypeConfiguration<ProjectTypeReference>
{
    public void Configure(EntityTypeBuilder<ProjectTypeReference> builder)
    {
        // Table configuration
        builder.ToTable("ProjectTypeReferences", t =>
        {
            t.HasComment("Reference data for all valid ProjectType enum values");
        });

        // Key configuration
        builder.HasKey(p => p.Id);

        // Property configurations
        builder.Property(p => p.EnumName)
            .HasMaxLength(50)
            .IsRequired()
            .HasComment("Enum member name matching ProjectType enum");

        builder.Property(p => p.DisplayName)
            .HasMaxLength(100)
            .IsRequired()
            .HasComment("User-friendly display name for UI/reports");

        builder.Property(p => p.Description)
            .HasMaxLength(500)
            .IsRequired()
            .HasComment("Detailed description of the project type");

        builder.Property(p => p.IconIdentifier)
            .HasMaxLength(10)
            .HasComment("Icon or emoji identifier for UI (e.g., '🎮', '☁️')");

        builder.Property(p => p.DisplayOrder)
            .HasComment("Sort order for UI dropdowns");

        builder.Property(p => p.IsActive)
            .HasComment("Is this project type currently active/usable");

        builder.Property(p => p.ColorCode)
            .HasMaxLength(7)
            .HasComment("Hex color code for UI categorization (e.g., '#FF5733')");

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("When this reference was created");

        builder.Property(p => p.UpdatedAt)
            .HasComment("When this reference was last updated");

        // Index for efficient lookups by enum name
        builder.HasIndex(p => p.EnumName).IsUnique();

        // Seed data - matches ProjectType enum exactly
        // Note: Using 1-based IDs for seed data (EF requires non-zero for seed entities)
        // The EnumName can still be used as the reference by external systems
        builder.HasData(GetSeedData());
    }

    private static IEnumerable<ProjectTypeReference> GetSeedData()
    {
        return new[]
        {
            new ProjectTypeReference
            {
                Id = 1,
                EnumName = "GameDevelopment",
                DisplayName = "Game Development",
                Description = "Game design, engine development, art, audio, and game programming projects",
                IconIdentifier = "🎮",
                DisplayOrder = 1,
                IsActive = true,
                ColorCode = "#FF6B6B",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProjectTypeReference
            {
                Id = 2,
                EnumName = "CloudService",
                DisplayName = "Cloud Service",
                Description = "Cloud infrastructure, migration, deployment, and cloud-native application projects",
                IconIdentifier = "☁️",
                DisplayOrder = 2,
                IsActive = true,
                ColorCode = "#4ECDC4",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProjectTypeReference
            {
                Id = 3,
                EnumName = "CustomerSupport",
                DisplayName = "Customer Support",
                Description = "Customer support platforms, ticketing systems, and customer engagement tools",
                IconIdentifier = "🎧",
                DisplayOrder = 3,
                IsActive = true,
                ColorCode = "#95E1D3",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProjectTypeReference
            {
                Id = 4,
                EnumName = "DataAnalytics",
                DisplayName = "Data Analytics",
                Description = "Data pipelines, analytics engines, reporting, BI tools, and data warehouse projects",
                IconIdentifier = "📊",
                DisplayOrder = 4,
                IsActive = true,
                ColorCode = "#F38181",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProjectTypeReference
            {
                Id = 5,
                EnumName = "Infrastructure",
                DisplayName = "Infrastructure",
                Description = "CI/CD pipelines, monitoring systems, DevOps, container orchestration, and infrastructure management",
                IconIdentifier = "⚙️",
                DisplayOrder = 5,
                IsActive = true,
                ColorCode = "#AA96DA",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProjectTypeReference
            {
                Id = 6,
                EnumName = "Security",
                DisplayName = "Security",
                Description = "Security implementation, compliance, auditing, penetration testing, and security infrastructure",
                IconIdentifier = "🔒",
                DisplayOrder = 6,
                IsActive = true,
                ColorCode = "#FCBAD3",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProjectTypeReference
            {
                Id = 7,
                EnumName = "MobileApp",
                DisplayName = "Mobile App",
                Description = "iOS, Android, and cross-platform mobile application development projects",
                IconIdentifier = "📱",
                DisplayOrder = 7,
                IsActive = true,
                ColorCode = "#FFFFD2",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProjectTypeReference
            {
                Id = 8,
                EnumName = "WebApplication",
                DisplayName = "Web Application",
                Description = "Frontend and backend web application development, SPA projects, and web services",
                IconIdentifier = "🌐",
                DisplayOrder = 8,
                IsActive = true,
                ColorCode = "#80D8FF",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProjectTypeReference
            {
                Id = 9,
                EnumName = "ApiDevelopment",
                DisplayName = "API Development",
                Description = "REST APIs, GraphQL, gRPC, and other API interface development and integration projects",
                IconIdentifier = "🔗",
                DisplayOrder = 9,
                IsActive = true,
                ColorCode = "#B2DFDB",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProjectTypeReference
            {
                Id = 10,
                EnumName = "MachineLearning",
                DisplayName = "Machine Learning",
                Description = "ML model development, AI implementations, training pipelines, and predictive analytics",
                IconIdentifier = "🤖",
                DisplayOrder = 10,
                IsActive = true,
                ColorCode = "#FFB74D",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProjectTypeReference
            {
                Id = 11,
                EnumName = "Integration",
                DisplayName = "Integration",
                Description = "System integration, middleware development, data interchange, and enterprise integration projects",
                IconIdentifier = "🔀",
                DisplayOrder = 11,
                IsActive = true,
                ColorCode = "#CE93D8",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProjectTypeReference
            {
                Id = 12,
                EnumName = "Research",
                DisplayName = "Research",
                Description = "Research and POC (Proof of Concept) projects, experimentation, and prototyping",
                IconIdentifier = "🔬",
                DisplayOrder = 12,
                IsActive = true,
                ColorCode = "#64B5F6",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProjectTypeReference
            {
                Id = 13,
                EnumName = "Other",
                DisplayName = "Other",
                Description = "Miscellaneous projects that don't fit into standard categories",
                IconIdentifier = "📦",
                DisplayOrder = 13,
                IsActive = true,
                ColorCode = "#B0BEC5",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };
    }
}
