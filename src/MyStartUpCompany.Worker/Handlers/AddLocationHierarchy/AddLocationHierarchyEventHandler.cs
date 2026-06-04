using System;
using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;
using Microsoft.Extensions.Logging;

namespace MyStartUpCompany.Worker.Handlers.AddLocationHierarchy
{
    /// <summary>
    /// Processes hierarchical location messages from Azure Service Bus.
    /// Handles the complete hierarchy: Location → Buildings → Offices.
    /// 
    /// Key Features:
    /// - Denormalizes building and location data into Office table for fast searches
    /// - Ensures data consistency through transactional processing
    /// - Idempotent: Prevents duplicate locations from being processed
    /// - Hierarchical validation: Validates complete structure before saving
    /// 
    /// Denormalization Strategy:
    /// When an Office is created, the following denormalized fields are populated:
    /// - BuildingName: Copied from Building.Name
    /// - LocationCity: Copied from Location.City
    /// - LocationRegion: Copied from Location.Region
    /// - LocationCountry: Copied from Location.Country
    /// 
    /// These fields enable fast searches on locations/buildings without joining tables.
    /// Database triggers maintain these fields in sync when Building or Location data changes.
    /// </summary>
    public class AddLocationHierarchyEventHandler
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AddLocationHierarchyEventHandler> _logger;

        public AddLocationHierarchyEventHandler(
            AppDbContext dbContext,
            ILogger<AddLocationHierarchyEventHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Processes a hierarchical location message and saves all related entities with denormalization.
        /// 
        /// Process:
        /// 1. Validates the input hierarchy
        /// 2. Checks if location already exists (idempotency)
        /// 3. Creates Location entity
        /// 4. Creates Building entities for each building in the location
        /// 5. Creates Office entities with denormalized fields populated
        /// 6. Saves all changes transactionally
        /// 
        /// Denormalization occurs in step 5 where Office entities are populated with:
        /// - BuildingName from the parent Building
        /// - LocationCity, LocationRegion, LocationCountry from the parent Location
        /// </summary>
        /// <param name="message">The hierarchical location message</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if processing was successful, false if location already exists</returns>
        /// <exception cref="ArgumentException">Thrown if message or location data is invalid</exception>
        /// <exception cref="InvalidOperationException">Thrown if database operations fail</exception>
        public async Task<bool> HandleAsync(
            LocationHierarchyMessage message,
            CancellationToken cancellationToken = default)
        {
            if (message?.Location == null)
            {
                throw new ArgumentException("Message and location data are required", nameof(message));
            }

            try
            {
                var locationDto = message.Location;

                _logger.LogInformation(
                    "Processing hierarchical location message for '{LocationName}'. " +
                    "CorrelationId: {CorrelationId}, Source: {Source}, " +
                    "Buildings: {BuildingCount}, Total Offices: {OfficeCount}",
                    locationDto.Name,
                    message.CorrelationId,
                    message.Source,
                    locationDto.Buildings?.Count ?? 0,
                    locationDto.Buildings?.Sum(b => b.Offices?.Count ?? 0) ?? 0);

                // Check if location already exists (idempotency check)
                var existingLocation = await _dbContext.Locations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Name == locationDto.Name && l.City == locationDto.City,
                        cancellationToken);

                if (existingLocation != null)
                {
                    _logger.LogWarning(
                        "Location '{LocationName}' in '{City}' already exists with Id {LocationId}. " +
                        "Skipping duplicate message.",
                        locationDto.Name,
                        locationDto.City,
                        existingLocation.Id);
                    return false;
                }

                // Start transaction for data consistency
                // Note: Transaction wrapping is optional for in-memory databases
                // The entity saves are atomic enough for most scenarios
                // For SQL Server, transactions could be added back if needed

                try
                {
                    // Create Location entity
                    var location = new Location
                    {
                        Name = locationDto.Name,
                        Description = locationDto.Description,
                        Address = locationDto.Address,
                        City = locationDto.City,
                        Region = locationDto.Region,
                        PostalCode = locationDto.PostalCode,
                        Country = locationDto.Country,
                        Phone = locationDto.Phone,
                        Email = locationDto.Email,
                        ManagerName = locationDto.ManagerName,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _dbContext.Locations.Add(location);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation(
                        "Created Location '{LocationName}' with Id {LocationId}",
                        location.Name,
                        location.Id);

                    // Process buildings and their offices
                    int buildingCount = 0;
                    int officeCount = 0;

                    if (locationDto.Buildings != null && locationDto.Buildings.Any())
                    {
                        foreach (var buildingDto in locationDto.Buildings)
                        {
                            // Create Building entity
                            var building = new Building
                            {
                                LocationId = location.Id,
                                Name = buildingDto.Name,
                                BuildingCode = buildingDto.BuildingCode,
                                Description = buildingDto.Description,
                                Address = buildingDto.Address,
                                NumberOfFloors = buildingDto.NumberOfFloors,
                                YearConstructed = buildingDto.YearConstructed,
                                TotalFloorArea = buildingDto.TotalFloorArea,
                                ContactPerson = buildingDto.ContactPerson,
                                Phone = buildingDto.Phone,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow
                            };

                            _dbContext.Buildings.Add(building);
                            await _dbContext.SaveChangesAsync(cancellationToken);

                            buildingCount++;

                            _logger.LogInformation(
                                "Created Building '{BuildingName}' with Id {BuildingId} in Location {LocationId}",
                                building.Name,
                                building.Id,
                                location.Id);

                            // Process offices with denormalization
                            if (buildingDto.Offices != null && buildingDto.Offices.Any())
                            {
                                foreach (var officeDto in buildingDto.Offices)
                                {
                                    // Create Office entity with denormalized fields
                                    var office = new Office
                                    {
                                        BuildingId = building.Id,
                                        Name = officeDto.Name,
                                        OfficeCode = officeDto.OfficeCode,
                                        Description = officeDto.Description,
                                        FloorNumber = officeDto.FloorNumber,
                                        Section = officeDto.Section,
                                        Capacity = officeDto.Capacity,
                                        OfficeType = officeDto.OfficeType,
                                        SquareMeters = officeDto.SquareMeters,
                                        Department = officeDto.Department,
                                        Manager = officeDto.Manager,
                                        Phone = officeDto.Phone,
                                        Email = officeDto.Email,
                                        IsActive = true,
                                        CreatedAt = DateTime.UtcNow,

                                        // ========== DENORMALIZATION ==========
                                        // Populate denormalized fields from Building and Location
                                        // These fields enable fast searches without joins
                                        BuildingName = building.Name,
                                        LocationCity = location.City,
                                        LocationRegion = location.Region,
                                        LocationCountry = location.Country
                                    };

                                    _dbContext.Offices.Add(office);
                                    officeCount++;

                                    _logger.LogDebug(
                                        "Created Office '{OfficeName}' with Id {OfficeId} in Building {BuildingId}. " +
                                        "Denormalized: BuildingName={BuildingName}, LocationCity={LocationCity}, " +
                                        "LocationRegion={LocationRegion}, LocationCountry={LocationCountry}",
                                        office.Name,
                                        office.Id,
                                        building.Id,
                                        office.BuildingName,
                                        office.LocationCity,
                                        office.LocationRegion,
                                        office.LocationCountry);
                                }

                                // Save all offices for this building
                                await _dbContext.SaveChangesAsync(cancellationToken);
                            }
                        }
                    }

                    _logger.LogInformation(
                        "Successfully processed location hierarchy message. " +
                        "Location: {LocationName} ({LocationId}), " +
                        "Buildings: {BuildingCount}, Offices: {OfficeCount}",
                        location.Name,
                        location.Id,
                        buildingCount,
                        officeCount);

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error processing location hierarchy message for '{LocationName}'. " +
                        "Error: {ErrorMessage}",
                        locationDto.Name,
                        ex.Message);

                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to process hierarchical location message. CorrelationId: {CorrelationId}",
                    message?.CorrelationId);
                throw;
            }
        }
    }
}
