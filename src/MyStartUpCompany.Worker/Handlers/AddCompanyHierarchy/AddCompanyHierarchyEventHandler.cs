using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Worker.Handlers.AddCompanyHierarchy
{
    /// <summary>
    /// Processes hierarchical company messages from Azure Service Bus.
    /// Handles the complete hierarchy: Company → Locations → Buildings → Offices.
    /// 
    /// This handler follows a transactional approach to ensure data consistency:
    /// - Either the entire hierarchy is saved successfully, or all changes are rolled back
    /// - Prevents partial/orphaned records in the database
    /// </summary>
    public class AddCompanyHierarchyEventHandler
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AddCompanyHierarchyEventHandler> _logger;

        public AddCompanyHierarchyEventHandler(
            AppDbContext dbContext,
            ILogger<AddCompanyHierarchyEventHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Processes a hierarchical company message and saves all related entities.
        /// 
        /// Process:
        /// 1. Validates the input hierarchy
        /// 2. Checks if company already exists (idempotency)
        /// 3. Creates or updates company entity
        /// 4. Processes locations and their children (buildings and offices)
        /// 5. Saves all changes transactionally
        /// </summary>
        /// <param name="message">The hierarchical company message</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if processing was successful, false if company already exists</returns>
        /// <exception cref="ArgumentException">Thrown if message or company data is invalid</exception>
        /// <exception cref="InvalidOperationException">Thrown if database operations fail</exception>
        public async Task<bool> HandleAsync(
            HierarchicalCompanyMessage message,
            CancellationToken cancellationToken = default)
        {
            if (message?.Company == null)
            {
                throw new ArgumentException("Message and company data are required", nameof(message));
            }

            try
            {
                var companyDto = message.Company;

                _logger.LogInformation(
                    "Processing hierarchical message for company '{CompanyName}'. " +
                    "CorrelationId: {CorrelationId}, Source: {Source}",
                    companyDto.Name,
                    message.CorrelationId,
                    message.Source);

                // Check if company already exists (idempotency check)
                var existingCompany = await _dbContext.Companies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Name == companyDto.Name, cancellationToken);

                if (existingCompany != null)
                {
                    _logger.LogWarning(
                        "Company '{CompanyName}' already exists with Id {CompanyId}. " +
                        "Skipping duplicate message.",
                        companyDto.Name,
                        existingCompany.Id);
                    return false;
                }

                // Start transaction for data consistency
                using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Create company entity
                    var company = new Company
                    {
                        Name = companyDto.Name,
                        Description = companyDto.Description,
                        Address = companyDto.Address,
                        City = companyDto.City,
                        Region = companyDto.Region,
                        PostalCode = companyDto.PostalCode,
                        Country = companyDto.Country,
                        Phone = companyDto.Phone
                    };

                    await _dbContext.Companies.AddAsync(company, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation(
                        "Created company '{CompanyName}' with Id {CompanyId}",
                        company.Name,
                        company.Id);

                    // Process locations
                    var locationCount = 0;
                    foreach (var locationDto in companyDto.Locations)
                    {
                        var location = new Location
                        {
                            CompanyId = company.Id,
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
                            IsActive = true
                        };

                        await _dbContext.Locations.AddAsync(location, cancellationToken);
                        await _dbContext.SaveChangesAsync(cancellationToken);
                        locationCount++;

                        _logger.LogInformation(
                            "Created location '{LocationName}' (Id: {LocationId}) for company '{CompanyName}'",
                            location.Name,
                            location.Id,
                            company.Name);

                        // Process buildings for this location
                        var buildingCount = 0;
                        foreach (var buildingDto in locationDto.Buildings)
                        {
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
                                IsActive = true
                            };

                            await _dbContext.Buildings.AddAsync(building, cancellationToken);
                            await _dbContext.SaveChangesAsync(cancellationToken);
                            buildingCount++;

                            _logger.LogInformation(
                                "Created building '{BuildingName}' (Id: {BuildingId}) in location '{LocationName}'",
                                building.Name,
                                building.Id,
                                location.Name);

                            // Process offices for this building
                            var officeCount = 0;
                            foreach (var officeDto in buildingDto.Offices)
                            {
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
                                    IsActive = true
                                };

                                await _dbContext.Offices.AddAsync(office, cancellationToken);
                                officeCount++;
                            }

                            // Save all offices for this building
                            if (officeCount > 0)
                            {
                                await _dbContext.SaveChangesAsync(cancellationToken);
                                _logger.LogInformation(
                                    "Created {OfficeCount} offices in building '{BuildingName}'",
                                    officeCount,
                                    building.Name);
                            }
                        }

                        if (buildingCount > 0)
                        {
                            _logger.LogInformation(
                                "Created {BuildingCount} buildings in location '{LocationName}'",
                                buildingCount,
                                location.Name);
                        }
                    }

                    // Commit transaction
                    await transaction.CommitAsync(cancellationToken);

                    _logger.LogInformation(
                        "Successfully processed hierarchical message for company '{CompanyName}'. " +
                        "Created {LocationCount} locations. CorrelationId: {CorrelationId}",
                        company.Name,
                        locationCount,
                        message.CorrelationId);

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex,
                        "Error processing hierarchical message for company '{CompanyName}'. " +
                        "Transaction rolled back. CorrelationId: {CorrelationId}",
                        companyDto.Name,
                        message.CorrelationId);
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex,
                    "Database error while processing hierarchical message for company '{CompanyName}'. " +
                    "CorrelationId: {CorrelationId}",
                    message.Company.Name,
                    message.CorrelationId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error processing hierarchical message. " +
                    "CorrelationId: {CorrelationId}",
                    message.CorrelationId);
                throw;
            }
        }
    }
}
