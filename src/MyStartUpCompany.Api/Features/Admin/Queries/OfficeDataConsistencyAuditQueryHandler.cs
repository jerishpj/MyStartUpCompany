using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Admin.Queries;

/// <summary>
/// Admin utility for auditing denormalized field consistency in Office table.
/// Verifies that denormalized fields (BuildingName, LocationCity, etc.) match their source tables.
/// </summary>
public interface IOfficeDataConsistencyAuditQueryHandler
{
    /// <summary>
    /// Audits denormalized fields in Office table against their source Building and Location tables.
    /// Returns records where denormalized values don't match source data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of inconsistent office records</returns>
    Task<List<OfficeConsistencyAuditResult>> AuditDenormalizedFieldsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Repairs inconsistent denormalized fields by syncing with source data.
    /// Should be run periodically as a maintenance job.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of records repaired</returns>
    Task<int> RepairInconsistentFieldsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a single office consistency audit
/// </summary>
public record OfficeConsistencyAuditResult
{
    /// <summary>
    /// Office ID with inconsistency
    /// </summary>
    public int OfficeId { get; init; }

    /// <summary>
    /// Office name
    /// </summary>
    public string OfficeName { get; init; }

    /// <summary>
    /// Building ID
    /// </summary>
    public int BuildingId { get; init; }

    /// <summary>
    /// Type of inconsistency (e.g., "BuildingName", "LocationCity", etc.)
    /// </summary>
    public string InconsistencyType { get; init; }

    /// <summary>
    /// Denormalized value stored in Office table
    /// </summary>
    public string? DenormalizedValue { get; init; }

    /// <summary>
    /// Actual value from source table (Building or Location)
    /// </summary>
    public string? ActualValue { get; init; }

    /// <summary>
    /// Timestamp when this inconsistency was detected
    /// </summary>
    public DateTime DetectedAt { get; init; }
}

/// <summary>
/// Implementation of IOfficeDataConsistencyAuditQueryHandler
/// </summary>
public class OfficeDataConsistencyAuditQueryHandler : IOfficeDataConsistencyAuditQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<OfficeDataConsistencyAuditQueryHandler> _logger;

    public OfficeDataConsistencyAuditQueryHandler(
        AppDbContext dbContext,
        ILogger<OfficeDataConsistencyAuditQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<OfficeConsistencyAuditResult>> AuditDenormalizedFieldsAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting audit of denormalized fields in Office table");

        var inconsistencies = new List<OfficeConsistencyAuditResult>();

        // Fetch all offices with their related building and location data
        var offices = await _dbContext.Offices
            .AsNoTracking()
            .Include(o => o.Building)
            .ThenInclude(b => b!.Location)
            .ToListAsync(cancellationToken);

        foreach (var office in offices)
        {
            if (office.Building == null || office.Building.Location == null)
            {
                _logger.LogWarning(
                    "Office {OfficeId} has missing Building or Location reference",
                    office.Id);
                continue;
            }

            // Check BuildingName consistency
            if (office.BuildingName != office.Building.Name)
            {
                inconsistencies.Add(new OfficeConsistencyAuditResult
                {
                    OfficeId = office.Id,
                    OfficeName = office.Name,
                    BuildingId = office.BuildingId,
                    InconsistencyType = "BuildingName",
                    DenormalizedValue = office.BuildingName,
                    ActualValue = office.Building.Name,
                    DetectedAt = DateTime.UtcNow
                });

                _logger.LogWarning(
                    "Inconsistency detected: Office {OfficeId} BuildingName. " +
                    "Denormalized: '{DenormalizedValue}', Actual: '{ActualValue}'",
                    office.Id, office.BuildingName, office.Building.Name);
            }

            // Check LocationCity consistency
            if (office.LocationCity != office.Building.Location.City)
            {
                inconsistencies.Add(new OfficeConsistencyAuditResult
                {
                    OfficeId = office.Id,
                    OfficeName = office.Name,
                    BuildingId = office.BuildingId,
                    InconsistencyType = "LocationCity",
                    DenormalizedValue = office.LocationCity,
                    ActualValue = office.Building.Location.City,
                    DetectedAt = DateTime.UtcNow
                });

                _logger.LogWarning(
                    "Inconsistency detected: Office {OfficeId} LocationCity. " +
                    "Denormalized: '{DenormalizedValue}', Actual: '{ActualValue}'",
                    office.Id, office.LocationCity, office.Building.Location.City);
            }

            // Check LocationRegion consistency
            if (office.LocationRegion != office.Building.Location.Region)
            {
                inconsistencies.Add(new OfficeConsistencyAuditResult
                {
                    OfficeId = office.Id,
                    OfficeName = office.Name,
                    BuildingId = office.BuildingId,
                    InconsistencyType = "LocationRegion",
                    DenormalizedValue = office.LocationRegion,
                    ActualValue = office.Building.Location.Region,
                    DetectedAt = DateTime.UtcNow
                });

                _logger.LogWarning(
                    "Inconsistency detected: Office {OfficeId} LocationRegion. " +
                    "Denormalized: '{DenormalizedValue}', Actual: '{ActualValue}'",
                    office.Id, office.LocationRegion, office.Building.Location.Region);
            }

            // Check LocationCountry consistency
            if (office.LocationCountry != office.Building.Location.Country)
            {
                inconsistencies.Add(new OfficeConsistencyAuditResult
                {
                    OfficeId = office.Id,
                    OfficeName = office.Name,
                    BuildingId = office.BuildingId,
                    InconsistencyType = "LocationCountry",
                    DenormalizedValue = office.LocationCountry,
                    ActualValue = office.Building.Location.Country,
                    DetectedAt = DateTime.UtcNow
                });

                _logger.LogWarning(
                    "Inconsistency detected: Office {OfficeId} LocationCountry. " +
                    "Denormalized: '{DenormalizedValue}', Actual: '{ActualValue}'",
                    office.Id, office.LocationCountry, office.Building.Location.Country);
            }
        }

        _logger.LogInformation(
            "Audit completed. Found {InconsistencyCount} inconsistencies in {OfficeCount} offices",
            inconsistencies.Count, offices.Count);

        return inconsistencies;
    }

    public async Task<int> RepairInconsistentFieldsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting repair of inconsistent denormalized fields");

        int repairsPerformed = 0;

        try
        {
            // Use raw SQL for bulk update - more efficient than EF Core for this scenario
            const string sql = @"
                UPDATE o
                SET 
                    o.BuildingName = b.Name,
                    o.LocationCity = l.City,
                    o.LocationRegion = l.Region,
                    o.LocationCountry = l.Country,
                    o.UpdatedAt = GETUTCDATE()
                FROM Offices o
                INNER JOIN Buildings b ON o.BuildingId = b.Id
                INNER JOIN Locations l ON b.LocationId = l.Id
                WHERE 
                    o.BuildingName != b.Name
                    OR o.LocationCity != l.City
                    OR o.LocationRegion != l.Region
                    OR o.LocationCountry != l.Country
                ";

            var repaired = await _dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);

            repairsPerformed = (int)repaired;

            _logger.LogInformation(
                "Repair completed successfully. {RepairCount} office records were updated",
                repairsPerformed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred during repair of inconsistent denormalized fields");
            throw;
        }

        return repairsPerformed;
    }
}
