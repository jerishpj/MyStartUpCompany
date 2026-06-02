namespace MyStartUpCompany.Persistence.Entities
{
    /// <summary>
    /// Represents an office branch within a building.
    /// A building can have multiple office branches.
    /// </summary>
    public class Office
    {
        /// <summary>
        /// Unique identifier for the office
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the parent building
        /// </summary>
        public int BuildingId { get; set; }

        /// <summary>
        /// Office name or identifier (e.g., "Floor 3 - Sales", "Executive Suite")
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Office code for easy reference (e.g., "F3-A", "EX-01")
        /// </summary>
        public string? OfficeCode { get; set; }

        /// <summary>
        /// Brief description of the office
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Floor number where this office is located
        /// </summary>
        public int? FloorNumber { get; set; }

        /// <summary>
        /// Section or wing identifier (e.g., "Wing A", "Section North")
        /// </summary>
        public string? Section { get; set; }

        /// <summary>
        /// Office capacity (number of workstations/employees)
        /// </summary>
        public int? Capacity { get; set; }

        /// <summary>
        /// Office type (e.g., "Open Office", "Cubicles", "Private", "Meeting Room")
        /// </summary>
        public string? OfficeType { get; set; }

        /// <summary>
        /// Office area in square meters
        /// </summary>
        public decimal? SquareMeters { get; set; }

        /// <summary>
        /// Department or team operating from this office
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Office manager or head
        /// </summary>
        public string? Manager { get; set; }

        /// <summary>
        /// Contact phone for this office
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Contact email for this office
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Indicates if this office is currently operational
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// When this office record was created (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When this office record was last updated (UTC, nullable)
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Navigation property to the parent building
        /// </summary>
        public virtual Building? Building { get; set; }
    }
}
