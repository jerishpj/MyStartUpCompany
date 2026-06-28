using MyStartUpCompany.Persistence.Entities;
using MyStartUpCompany.Persistence.Repositories;

namespace MyStartUpCompany.Worker.Handlers.AddCompany
{
    /// <summary>
    /// Event handler for adding or updating company information.
    /// Uses the upsert pattern: if a company with the same name and address exists, updates it;
    /// otherwise, creates a new company record.
    /// </summary>
    public class AddCompanyEventHandler
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<AddCompanyEventHandler> _logger;

        public AddCompanyEventHandler(ICompanyRepository companyRepository, ILogger<AddCompanyEventHandler> logger)
        {
            _companyRepository = companyRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the addition or update of company information.
        /// </summary>
        /// <param name="companyDto">The company data to add or update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if the operation was successful</returns>
        /// <exception cref="Exception">Re-throws any unexpected errors</exception>
        public async Task<bool> HandleAsync(CompanyInputDto companyDto, CancellationToken cancellationToken = default)
        {
            var result = await HandleWithResultAsync(companyDto, cancellationToken);
            return result.IsSuccess;
        }

        /// <summary>
        /// Handles the addition or update of company information and returns detailed result.
        /// </summary>
        /// <param name="companyDto">The company data to add or update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Detailed result indicating success and whether it was an insert or update</returns>
        /// <exception cref="Exception">Re-throws any unexpected errors</exception>
        public async Task<UpsertResult> HandleWithResultAsync(CompanyInputDto companyDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if company already exists
                var existingCompany = await _companyRepository.FindByNameAndAddressAsync(
                    companyDto.Name, companyDto.Address, cancellationToken);

                // Create company entity from DTO
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

                // Upsert the company (insert if not exists, update if exists)
                var upsertedCompany = await _companyRepository.UpsertAsync(company, cancellationToken);

                if (upsertedCompany.Id == 0)
                {
                    _logger.LogWarning("Failed to upsert company '{CompanyName}' - no Id assigned", companyDto.Name);
                    return new UpsertResult { IsSuccess = false };
                }

                // Determine if it was an insert or update
                bool isInsert = existingCompany == null;
                var operation = isInsert ? "added" : "updated";
                _logger.LogInformation(
                    "Successfully {Operation} company '{CompanyName}' with Id {CompanyId}",
                    operation, upsertedCompany.Name, upsertedCompany.Id);

                return new UpsertResult { IsSuccess = true, IsInsert = isInsert };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting company '{CompanyName}'", companyDto.Name);
                throw;
            }
        }

        /// <summary>
        /// Result of an upsert operation.
        /// </summary>
        public class UpsertResult
        {
            public bool IsSuccess { get; set; }
            public bool IsInsert { get; set; }
        }
    }
}
