namespace MyStartUpCompany.Persistence.Entities
{
    /// <summary>
    /// Represents a physical location/branch of a company.
    /// A company can have multiple locations.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Unique identifier for the location
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the parent company
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// Name or identifier of this location (e.g., "New York Branch", "Headquarters")
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Brief description of the location
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Street address of the location
        /// </summary>
        public required string Address { get; set; }

        /// <summary>
        /// City where the location is situated
        /// </summary>
        public required string City { get; set; }

        /// <summary>
        /// State or region (optional)
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Postal or ZIP code
        /// </summary>
        public required string PostalCode { get; set; }

        /// <summary>
        /// Country where the location is situated
        /// </summary>
        public required string Country { get; set; }

        /// <summary>
        /// Primary contact phone number for this location
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Primary contact email for this location
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Manager or contact person name
        /// </summary>
        public string? ManagerName { get; set; }

        /// <summary>
        /// Indicates if this location is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// When this location record was created (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When this location record was last updated (UTC, nullable)
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Navigation property to the parent company
        /// </summary>
        public virtual Company? Company { get; set; }

        /// <summary>
        /// Navigation property to buildings within this location
        /// </summary>
        public virtual ICollection<Building> Buildings { get; set; } = new List<Building>();
    }
}
