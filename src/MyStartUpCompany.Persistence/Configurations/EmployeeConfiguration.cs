using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Configurations;

/// <summary>
/// Entity configuration for Employee with performance optimizations
/// </summary>
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(e => e.Id);

        // Column configurations
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.City)
            .HasMaxLength(200);

        builder.Property(e => e.Region)
            .HasMaxLength(100);

        builder.Property(e => e.PostalCode)
            .HasMaxLength(50);

        builder.Property(e => e.Country)
            .HasMaxLength(200);

        builder.Property(e => e.Phone)
            .HasMaxLength(50);

        builder.Property(e => e.Email)
            .HasMaxLength(320); // RFC 5321 max email length

        // Indexes for common queries
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Employee_Name");

        builder.HasIndex(e => e.Email)
            .HasDatabaseName("IX_Employee_Email")
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");

        builder.HasIndex(e => e.Title)
            .HasDatabaseName("IX_Employee_Title");
    }
}