namespace MyStartUpCompany.Persistence.Entities
{
    /// <summary>
    /// Represents a physical building within a location.
    /// A location can have multiple buildings.
    /// </summary>
    public class Building
    {
        /// <summary>
        /// Unique identifier for the building
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the parent location
        /// </summary>
        public int LocationId { get; set; }

        /// <summary>
        /// Building name or identifier (e.g., "Building A", "East Tower")
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Building code for easy reference
        /// </summary>
        public string? BuildingCode { get; set; }

        /// <summary>
        /// Brief description of the building
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Street address of the building
        /// </summary>
        public required string Address { get; set; }

        /// <summary>
        /// Total number of floors in this building
        /// </summary>
        public int? NumberOfFloors { get; set; }

        /// <summary>
        /// Year the building was constructed
        /// </summary>
        public int? YearConstructed { get; set; }

        /// <summary>
        /// Total floor area in square meters
        /// </summary>
        public decimal? TotalFloorArea { get; set; }

        /// <summary>
        /// Primary contact for the building
        /// </summary>
        public string? ContactPerson { get; set; }

        /// <summary>
        /// Contact phone number for the building
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Indicates if this building is operational
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// When this building record was created (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When this building record was last updated (UTC, nullable)
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Navigation property to the parent location
        /// </summary>
        public virtual Location? Location { get; set; }

        /// <summary>
        /// Navigation property to office branches within this building
        /// </summary>
        public virtual ICollection<Office> Offices { get; set; } = new List<Office>();
    }
}
