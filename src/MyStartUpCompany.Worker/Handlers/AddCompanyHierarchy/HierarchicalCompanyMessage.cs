namespace MyStartUpCompany.Worker.Handlers.AddCompanyHierarchy
{
    /// <summary>
    /// Represents a hierarchical company message received from Azure Service Bus.
    /// Contains company with nested locations, buildings, and offices.
    /// </summary>
    public class HierarchicalCompanyMessage
    {
        /// <summary>
        /// Unique identifier for this message (correlation ID or event ID)
        /// </summary>
        public string? CorrelationId { get; set; }

        /// <summary>
        /// Timestamp when the message was created (ISO 8601 format)
        /// </summary>
        public string? CreatedAt { get; set; }

        /// <summary>
        /// Source system identifier that sent the message
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Message version for handling schema evolution
        /// </summary>
        public string? Version { get; set; } = "1.0";

        /// <summary>
        /// The company with its complete hierarchical structure
        /// </summary>
        public required CompanyHierarchyInputDto Company { get; set; }
    }

    /// <summary>
    /// Represents a company with its complete hierarchical structure.
    /// </summary>
    public class CompanyHierarchyInputDto
    {
        /// <summary>
        /// Company name
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Company description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Company headquarters address
        /// </summary>
        public required string Address { get; set; }

        /// <summary>
        /// Headquarters city
        /// </summary>
        public required string City { get; set; }

        /// <summary>
        /// Headquarters region/state
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Headquarters postal code
        /// </summary>
        public required string PostalCode { get; set; }

        /// <summary>
        /// Headquarters country
        /// </summary>
        public required string Country { get; set; }

        /// <summary>
        /// Primary company phone
        /// </summary>
        public required string Phone { get; set; }

        /// <summary>
        /// Collection of locations under this company
        /// </summary>
        public ICollection<LocationInputDto> Locations { get; set; } = new List<LocationInputDto>();
    }

    /// <summary>
    /// Represents a location with its nested buildings.
    /// </summary>
    public class LocationInputDto
    {
        /// <summary>
        /// Location name/identifier
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Location description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Location address
        /// </summary>
        public required string Address { get; set; }

        /// <summary>
        /// Location city
        /// </summary>
        public required string City { get; set; }

        /// <summary>
        /// Location region/state
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Location postal code
        /// </summary>
        public required string PostalCode { get; set; }

        /// <summary>
        /// Location country
        /// </summary>
        public required string Country { get; set; }

        /// <summary>
        /// Location contact phone
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Location contact email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Location manager name
        /// </summary>
        public string? ManagerName { get; set; }

        /// <summary>
        /// Collection of buildings in this location
        /// </summary>
        public ICollection<BuildingInputDto> Buildings { get; set; } = new List<BuildingInputDto>();
    }

    /// <summary>
    /// Represents a building with its nested office branches.
    /// </summary>
    public class BuildingInputDto
    {
        /// <summary>
        /// Building name/identifier
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Building code for reference
        /// </summary>
        public string? BuildingCode { get; set; }

        /// <summary>
        /// Building description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Building address
        /// </summary>
        public required string Address { get; set; }

        /// <summary>
        /// Number of floors in the building
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
        /// Building contact person
        /// </summary>
        public string? ContactPerson { get; set; }

        /// <summary>
        /// Building contact phone
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Collection of office branches in this building
        /// </summary>
        public ICollection<OfficeInputDto> Offices { get; set; } = new List<OfficeInputDto>();
    }

    /// <summary>
    /// Represents an office branch within a building.
    /// </summary>
    public class OfficeInputDto
    {
        /// <summary>
        /// Office name/identifier
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Office code for reference
        /// </summary>
        public string? OfficeCode { get; set; }

        /// <summary>
        /// Office description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Floor number where office is located
        /// </summary>
        public int? FloorNumber { get; set; }

        /// <summary>
        /// Section or wing identifier
        /// </summary>
        public string? Section { get; set; }

        /// <summary>
        /// Office capacity (number of workstations)
        /// </summary>
        public int? Capacity { get; set; }

        /// <summary>
        /// Office type (e.g., Open Office, Cubicles, Private, Meeting Room)
        /// </summary>
        public string? OfficeType { get; set; }

        /// <summary>
        /// Office area in square meters
        /// </summary>
        public decimal? SquareMeters { get; set; }

        /// <summary>
        /// Department or team name
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Office manager or head
        /// </summary>
        public string? Manager { get; set; }

        /// <summary>
        /// Office contact phone
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Office contact email
        /// </summary>
        public string? Email { get; set; }
    }
}
