using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Building entity.
/// Defines table structure, relationships, indexes, and constraints.
/// </summary>
public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        // Table configuration
        builder.ToTable("Buildings");
        builder.HasKey(b => b.Id);

        // Column configurations with appropriate lengths
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.BuildingCode)
            .HasMaxLength(100);

        builder.Property(b => b.Description)
            .HasMaxLength(2000);

        builder.Property(b => b.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.NumberOfFloors)
            .IsRequired(false);

        builder.Property(b => b.YearConstructed)
            .IsRequired(false);

        builder.Property(b => b.TotalFloorArea)
            .HasPrecision(10, 2)
            .IsRequired(false);

        builder.Property(b => b.ContactPerson)
            .HasMaxLength(300);

        builder.Property(b => b.Phone)
            .HasMaxLength(50);

        builder.Property(b => b.IsActive)
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(b => b.UpdatedAt)
            .IsRequired(false);

        // Foreign key configuration for Location relationship
        builder.HasOne(b => b.Location)
            .WithMany(l => l.Buildings)
            .HasForeignKey(b => b.LocationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // ========== PERFORMANCE INDEXES ==========

        // Primary lookup by location
        builder.HasIndex(b => b.LocationId)
            .HasDatabaseName("IX_Building_LocationId");

        // Building code lookup (if used for reference)
        builder.HasIndex(b => b.BuildingCode)
            .HasDatabaseName("IX_Building_Code")
            .IsUnique(false);

        // Active buildings query
        builder.HasIndex(b => new { b.IsActive, b.LocationId })
            .HasDatabaseName("IX_Building_Active_Location");

        // Combined lookup for location and active status
        builder.HasIndex(b => new { b.LocationId, b.IsActive })
            .HasDatabaseName("IX_Building_Location_Active");

        // Search by name
        builder.HasIndex(b => b.Name)
            .HasDatabaseName("IX_Building_Name");
    }
}
