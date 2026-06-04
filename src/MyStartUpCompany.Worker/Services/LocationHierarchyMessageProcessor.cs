using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Worker.Handlers.AddLocationHierarchy;

namespace MyStartUpCompany.Worker.Services
{
    /// <summary>
    /// Service for processing location hierarchy messages from any source.
    /// Handles validation, deserialization, and delegation to the location hierarchy handler.
    /// </summary>
    public class LocationHierarchyMessageProcessor
    {
        private readonly AddLocationHierarchyEventHandler _handler;
        private readonly ILogger<LocationHierarchyMessageProcessor> _logger;

        public LocationHierarchyMessageProcessor(
            AddLocationHierarchyEventHandler handler,
            ILogger<LocationHierarchyMessageProcessor> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        /// <summary>
        /// Processes a hierarchical location message containing locations, buildings, and offices.
        /// </summary>
        /// <param name="message">The hierarchical location message</param>
        /// <param name="sourceIdentifier">Identifier of the source for logging</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Processing result</returns>
        public async Task<LocationProcessingResult> ProcessLocationHierarchyAsync(
            LocationHierarchyMessage message,
            string sourceIdentifier,
            CancellationToken cancellationToken = default)
        {
            // Handle null input
            if (message == null)
            {
                _logger.LogWarning("Null LocationHierarchyMessage received from source '{Source}'", sourceIdentifier);
                return LocationProcessingResult.Invalid("Location hierarchy message is null");
            }

            if (message.Location == null)
            {
                _logger.LogWarning("Location data is null in message from source '{Source}'", sourceIdentifier);
                return LocationProcessingResult.Invalid("Location data is null");
            }

            // Validate required fields
            var validationResult = ValidateLocationData(message.Location);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Validation failed for location '{LocationName}' from source '{Source}': {Reason}",
                    message.Location.Name ?? "Unknown", sourceIdentifier, validationResult.ErrorMessage);
                return LocationProcessingResult.Invalid(validationResult.ErrorMessage);
            }

            // Process the location hierarchy
            try
            {
                var success = await _handler.HandleAsync(message, cancellationToken);

                if (success)
                {
                    _logger.LogInformation(
                        "Successfully processed location hierarchy '{LocationName}' from source '{Source}'. " +
                        "Buildings: {BuildingCount}, Offices: {OfficeCount}",
                        message.Location.Name,
                        sourceIdentifier,
                        message.Location.Buildings?.Count ?? 0,
                        message.Location.Buildings?.Sum(b => b.Offices?.Count ?? 0) ?? 0);
                    return LocationProcessingResult.Success();
                }
                else
                {
                    _logger.LogWarning(
                        "Location '{LocationName}' from source '{Source}' was not added (likely duplicate)",
                        message.Location.Name, sourceIdentifier);
                    return LocationProcessingResult.Duplicate();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing location hierarchy '{LocationName}' from source '{Source}'",
                    message.Location.Name, sourceIdentifier);
                return LocationProcessingResult.Error(ex.Message);
            }
        }

        /// <summary>
        /// Validates location hierarchy data against required fields.
        /// </summary>
        private ValidationResult ValidateLocationData(LocationInputDto locationDto)
        {
            if (locationDto == null)
                return ValidationResult.Invalid("Location data is null");

            if (string.IsNullOrWhiteSpace(locationDto.Name))
                return ValidationResult.Invalid("Location name is required");

            if (string.IsNullOrWhiteSpace(locationDto.Address))
                return ValidationResult.Invalid("Location address is required");

            if (string.IsNullOrWhiteSpace(locationDto.City))
                return ValidationResult.Invalid("Location city is required");

            if (string.IsNullOrWhiteSpace(locationDto.PostalCode))
                return ValidationResult.Invalid("Location postal code is required");

            if (string.IsNullOrWhiteSpace(locationDto.Country))
                return ValidationResult.Invalid("Location country is required");

            // Validate buildings if present
            if (locationDto.Buildings != null && locationDto.Buildings.Any())
            {
                foreach (var building in locationDto.Buildings)
                {
                    if (string.IsNullOrWhiteSpace(building.Name))
                        return ValidationResult.Invalid("Building name is required");

                    if (string.IsNullOrWhiteSpace(building.Address))
                        return ValidationResult.Invalid("Building address is required");

                    // Validate offices if present
                    if (building.Offices != null && building.Offices.Any())
                    {
                        foreach (var office in building.Offices)
                        {
                            if (string.IsNullOrWhiteSpace(office.Name))
                                return ValidationResult.Invalid("Office name is required");
                        }
                    }
                }
            }

            return ValidationResult.Valid();
        }

        /// <summary>
        /// Result of processing a location hierarchy message.
        /// </summary>
        public class LocationProcessingResult
        {
            public enum Status
            {
                Success,
                Duplicate,
                Invalid,
                Error
            }

            public Status ProcessingStatus { get; private set; }
            public string? Message { get; private set; }

            public static LocationProcessingResult Success() => new() { ProcessingStatus = Status.Success };
            public static LocationProcessingResult Duplicate() => new() { ProcessingStatus = Status.Duplicate };
            public static LocationProcessingResult Invalid(string message) => 
                new() { ProcessingStatus = Status.Invalid, Message = message };
            public static LocationProcessingResult Error(string message) => 
                new() { ProcessingStatus = Status.Error, Message = message };
        }

        /// <summary>
        /// Result of data validation.
        /// </summary>
        private class ValidationResult
        {
            public bool IsValid { get; private set; }
            public string? ErrorMessage { get; private set; }

            public static ValidationResult Valid() => new() { IsValid = true };
            public static ValidationResult Invalid(string message) => 
                new() { IsValid = false, ErrorMessage = message };
        }
    }
}
