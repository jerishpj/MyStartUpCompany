using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Office entity.
/// Defines table structure, relationships, indexes, and constraints.
/// </summary>
public class OfficeConfiguration : IEntityTypeConfiguration<Office>
{
    public void Configure(EntityTypeBuilder<Office> builder)
    {
        // Table configuration
        builder.ToTable("Offices");
        builder.HasKey(o => o.Id);

        // Column configurations with appropriate lengths
        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.OfficeCode)
            .HasMaxLength(100);

        builder.Property(o => o.Description)
            .HasMaxLength(2000);

        builder.Property(o => o.FloorNumber)
            .IsRequired(false);

        builder.Property(o => o.Section)
            .HasMaxLength(100);

        builder.Property(o => o.Capacity)
            .IsRequired(false);

        builder.Property(o => o.OfficeType)
            .HasMaxLength(100);

        builder.Property(o => o.SquareMeters)
            .HasPrecision(10, 2)
            .IsRequired(false);

        builder.Property(o => o.Department)
            .HasMaxLength(200);

        builder.Property(o => o.Manager)
            .HasMaxLength(300);

        builder.Property(o => o.Phone)
            .HasMaxLength(50);

        builder.Property(o => o.Email)
            .HasMaxLength(256);

        // ========== DENORMALIZED COLUMN CONFIGURATIONS ==========
        builder.Property(o => o.BuildingName)
            .HasMaxLength(500);

        builder.Property(o => o.LocationCity)
            .HasMaxLength(200);

        builder.Property(o => o.LocationRegion)
            .HasMaxLength(100);

        builder.Property(o => o.LocationCountry)
            .HasMaxLength(200);

        builder.Property(o => o.IsActive)
            .HasDefaultValue(true);

        builder.Property(o => o.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(o => o.UpdatedAt)
            .IsRequired(false);

        // Foreign key configuration for Building relationship
        builder.HasOne(o => o.Building)
            .WithMany(b => b.Offices)
            .HasForeignKey(o => o.BuildingId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // ========== PERFORMANCE INDEXES ==========

        // Primary lookup by building
        builder.HasIndex(o => o.BuildingId)
            .HasDatabaseName("IX_Office_BuildingId");

        // Office code lookup (if used for reference)
        builder.HasIndex(o => o.OfficeCode)
            .HasDatabaseName("IX_Office_Code")
            .IsUnique(false);

        // Active offices query
        builder.HasIndex(o => new { o.IsActive, o.BuildingId })
            .HasDatabaseName("IX_Office_Active_Building");

        // Combined lookup for building and active status
        builder.HasIndex(o => new { o.BuildingId, o.IsActive })
            .HasDatabaseName("IX_Office_Building_Active");

        // Filter by department
        builder.HasIndex(o => o.Department)
            .HasDatabaseName("IX_Office_Department");

        // Filter by office type
        builder.HasIndex(o => o.OfficeType)
            .HasDatabaseName("IX_Office_Type");

        // Search by name
        builder.HasIndex(o => o.Name)
            .HasDatabaseName("IX_Office_Name");

        // ========== DENORMALIZATION PERFORMANCE INDEXES ==========
        // These indexes are critical for fast search queries without joins

        // Building name search (single table lookup instead of join)
        builder.HasIndex(o => o.BuildingName)
            .HasDatabaseName("IX_Office_BuildingName");

        // Location geographic search (city, region, country filtering)
        builder.HasIndex(o => new { o.LocationCountry, o.LocationRegion, o.LocationCity })
            .HasDatabaseName("IX_Office_Location_Geographic");

        // Combined: Active + Location for filtered searches
        builder.HasIndex(o => new { o.IsActive, o.LocationCity, o.LocationCountry })
            .HasDatabaseName("IX_Office_Active_Location_Geographic");

        // Combined: Active + Building name for department searches with location filter
        builder.HasIndex(o => new { o.IsActive, o.BuildingName, o.Department })
            .HasDatabaseName("IX_Office_Active_Building_Department");

        // Multi-field search: Building + Office Type + Active status
        builder.HasIndex(o => new { o.BuildingName, o.OfficeType, o.IsActive })
            .HasDatabaseName("IX_Office_Building_Type_Active");

        // Location + Department combined search
        builder.HasIndex(o => new { o.LocationCity, o.Department, o.IsActive })
            .HasDatabaseName("IX_Office_Location_Department_Active");
    }
}
