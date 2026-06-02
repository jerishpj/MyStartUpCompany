using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Location entity.
/// Defines table structure, relationships, indexes, and constraints.
/// </summary>
public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        // Table configuration
        builder.ToTable("Locations");
        builder.HasKey(l => l.Id);

        // Column configurations with appropriate lengths
        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.Description)
            .HasMaxLength(2000);

        builder.Property(l => l.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.City)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Region)
            .HasMaxLength(100);

        builder.Property(l => l.PostalCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.Country)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Phone)
            .HasMaxLength(50);

        builder.Property(l => l.Email)
            .HasMaxLength(256);

        builder.Property(l => l.ManagerName)
            .HasMaxLength(300);

        builder.Property(l => l.IsActive)
            .HasDefaultValue(true);

        builder.Property(l => l.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(l => l.UpdatedAt)
            .IsRequired(false);

        // Foreign key configuration for Company relationship
        builder.HasOne(l => l.Company)
            .WithMany()
            .HasForeignKey(l => l.CompanyId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // ========== PERFORMANCE INDEXES ==========

        // Primary lookup by company
        builder.HasIndex(l => l.CompanyId)
            .HasDatabaseName("IX_Location_CompanyId");

        // Location-based filtering
        builder.HasIndex(l => new { l.Country, l.Region, l.City })
            .HasDatabaseName("IX_Location_Geographic");

        // Active locations query
        builder.HasIndex(l => new { l.IsActive, l.CompanyId })
            .HasDatabaseName("IX_Location_Active_Company");

        // Filter by company and active status
        builder.HasIndex(l => new { l.CompanyId, l.IsActive })
            .HasDatabaseName("IX_Location_Company_Active");

        // Composite index for full-text search scenarios
        builder.HasIndex(l => l.Name)
            .HasDatabaseName("IX_Location_Name");
    }
}
