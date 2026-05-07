using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence
{
    // DB context for the application, responsible for managing database connections and operations
    public class AppDbContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // Configures the model and relationships between entities
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure the Company entity
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id); // Set Id as the primary key
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100); // Set Name as required with a max length
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(200); // Set Address as required with a max length
                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50); // Set City as required with a max length
                entity.Property(e => e.PostalCode)
                    .IsRequired()
                    .HasMaxLength(20); // Set PostalCode as required with a max length
                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(50); // Set Country as required with a max length
                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20); // Set Phone as required with a max length
            });
        }
    }
}
