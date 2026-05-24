using MyStartUpCompany.Worker.Handlers.AddCompany;

namespace MyStartUpCompany.Worker.Services
{
    /// <summary>
    /// Core service for processing company input data.
    /// This service validates and processes company data from any source (files, messages, etc.)
    /// and delegates persistence to the AddCompanyEventHandler.
    /// </summary>
    public class CompanyMessageProcessor
    {
        private readonly AddCompanyEventHandler _handler;
        private readonly ILogger<CompanyMessageProcessor> _logger;

        public CompanyMessageProcessor(
            AddCompanyEventHandler handler,
            ILogger<CompanyMessageProcessor> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        /// <summary>
        /// Processes a single company input and adds it to the database if valid.
        /// </summary>
        /// <param name="companyDto">The company data to process</param>
        /// <param name="sourceIdentifier">Identifier of the source (e.g., filename or message ID) for logging</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if processing was successful, false if duplicate or validation failed</returns>
        public async Task<CompanyProcessingResult> ProcessCompanyAsync(
            CompanyInputDto companyDto,
            string sourceIdentifier,
            CancellationToken cancellationToken = default)
        {
            // Validate required fields
            var validationResult = ValidateCompanyData(companyDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Validation failed for company '{CompanyName}' from source '{Source}': {Reason}",
                    companyDto.Name ?? "Unknown", sourceIdentifier, validationResult.ErrorMessage);
                return CompanyProcessingResult.Invalid(validationResult.ErrorMessage);
            }

            // Process the company
            try
            {
                var success = await _handler.HandleAsync(companyDto, cancellationToken);

                if (success)
                {
                    _logger.LogInformation(
                        "Successfully processed company '{CompanyName}' from source '{Source}'",
                        companyDto.Name, sourceIdentifier);
                    return CompanyProcessingResult.Success();
                }
                else
                {
                    _logger.LogWarning(
                        "Company '{CompanyName}' from source '{Source}' was not added (likely duplicate)",
                        companyDto.Name, sourceIdentifier);
                    return CompanyProcessingResult.Duplicate();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing company '{CompanyName}' from source '{Source}'",
                    companyDto.Name, sourceIdentifier);
                return CompanyProcessingResult.Error(ex.Message);
            }
        }

        /// <summary>
        /// Processes multiple companies from a batch.
        /// </summary>
        /// <param name="companies">Collection of companies to process</param>
        /// <param name="sourceIdentifier">Identifier of the source for logging</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Summary of processing results</returns>
        public async Task<CompanyBatchProcessingResult> ProcessCompaniesAsync(
            IEnumerable<CompanyInputDto> companies,
            string sourceIdentifier,
            CancellationToken cancellationToken = default)
        {
            var result = new CompanyBatchProcessingResult();

            foreach (var company in companies)
            {
                var processingResult = await ProcessCompanyAsync(company, sourceIdentifier, cancellationToken);
                result.AddResult(processingResult);
            }

            _logger.LogInformation(
                "Batch processing from '{Source}' completed: {Success} success, {Duplicate} duplicates, {Invalid} invalid, {Error} errors",
                sourceIdentifier, result.SuccessCount, result.DuplicateCount, result.InvalidCount, result.ErrorCount);

            return result;
        }

        /// <summary>
        /// Validates company data against required fields.
        /// </summary>
        private ValidationResult ValidateCompanyData(CompanyInputDto companyDto)
        {
            if (companyDto == null)
                return ValidationResult.Invalid("Company data is null");

            if (string.IsNullOrWhiteSpace(companyDto.Name))
                return ValidationResult.Invalid("Company name is required");

            if (string.IsNullOrWhiteSpace(companyDto.Address))
                return ValidationResult.Invalid("Address is required");

            if (string.IsNullOrWhiteSpace(companyDto.City))
                return ValidationResult.Invalid("City is required");

            if (string.IsNullOrWhiteSpace(companyDto.PostalCode))
                return ValidationResult.Invalid("Postal code is required");

            if (string.IsNullOrWhiteSpace(companyDto.Country))
                return ValidationResult.Invalid("Country is required");

            if (string.IsNullOrWhiteSpace(companyDto.Phone))
                return ValidationResult.Invalid("Phone is required");

            return ValidationResult.Valid();
        }

        /// <summary>
        /// Result of processing a single company.
        /// </summary>
        public class CompanyProcessingResult
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

            public static CompanyProcessingResult Success() => new() { ProcessingStatus = Status.Success };
            public static CompanyProcessingResult Duplicate() => new() { ProcessingStatus = Status.Duplicate };
            public static CompanyProcessingResult Invalid(string message) => 
                new() { ProcessingStatus = Status.Invalid, Message = message };
            public static CompanyProcessingResult Error(string message) => 
                new() { ProcessingStatus = Status.Error, Message = message };
        }

        /// <summary>
        /// Summary of batch processing results.
        /// </summary>
        public class CompanyBatchProcessingResult
        {
            public int SuccessCount { get; private set; }
            public int DuplicateCount { get; private set; }
            public int InvalidCount { get; private set; }
            public int ErrorCount { get; private set; }

            internal void AddResult(CompanyProcessingResult result)
            {
                switch (result.ProcessingStatus)
                {
                    case CompanyProcessingResult.Status.Success:
                        SuccessCount++;
                        break;
                    case CompanyProcessingResult.Status.Duplicate:
                        DuplicateCount++;
                        break;
                    case CompanyProcessingResult.Status.Invalid:
                        InvalidCount++;
                        break;
                    case CompanyProcessingResult.Status.Error:
                        ErrorCount++;
                        break;
                }
            }

            public int TotalProcessed => SuccessCount + DuplicateCount + InvalidCount + ErrorCount;
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
