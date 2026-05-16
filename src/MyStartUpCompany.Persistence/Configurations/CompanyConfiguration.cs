using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Configurations;

/// <summary>
/// Entity configuration for Company with performance optimizations
/// </summary>
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        // Table configuration
        builder.ToTable("Companies");
        builder.HasKey(c => c.Id);

        // Column configurations with length constraints (improves index efficiency)
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.City)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Region)
            .HasMaxLength(100);

        builder.Property(c => c.PostalCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Country)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Phone)
            .IsRequired()
            .HasMaxLength(50);

        // ========== CRITICAL PERFORMANCE INDEXES ==========

        // Composite covering index for location-based filtering
        // This is the most important index for your query patterns
        builder.HasIndex(c => new { c.Country, c.Region, c.City, c.PostalCode })
            .HasDatabaseName("IX_Company_Location_Covering")
            .IsDescending(false, false, false, false);

        // Individual indexes for single-column filters
        builder.HasIndex(c => c.Country)
            .HasDatabaseName("IX_Company_Country");

        builder.HasIndex(c => c.Region)
            .HasDatabaseName("IX_Company_Region")
            .HasFilter("[Region] IS NOT NULL"); // Partial index for better performance

        builder.HasIndex(c => c.City)
            .HasDatabaseName("IX_Company_City");

        builder.HasIndex(c => c.PostalCode)
            .HasDatabaseName("IX_Company_PostalCode");

        // Index for name searches and sorting
        builder.HasIndex(c => c.Name)
            .HasDatabaseName("IX_Company_Name");

        // Composite index for pagination performance (name + id for stable sorting)
        builder.HasIndex(c => new { c.Name, c.Id })
            .HasDatabaseName("IX_Company_Name_Id");
    }
}