using MyStartUpCompany.Worker.Handlers.AddCompany;
using System.Text.Json;

namespace MyStartUpCompany.Worker.Services
{
    public class CompanyFileProcessorService
    {
        private readonly AddCompanyEventHandler _handler;
        private readonly ILogger<CompanyFileProcessorService> _logger;
        private readonly string _inputFolderPath;
        private readonly string _processedFolderPath;

        public CompanyFileProcessorService(
            AddCompanyEventHandler handler,
            ILogger<CompanyFileProcessorService> logger,
            IHostEnvironment environment)
        {
            _handler = handler;
            _logger = logger;
            _inputFolderPath = Path.Combine(environment.ContentRootPath, "Input");
            _processedFolderPath = Path.Combine(environment.ContentRootPath, "Processed");

            // Ensure folders exist
            Directory.CreateDirectory(_inputFolderPath);
            Directory.CreateDirectory(_processedFolderPath);
        }

        public async Task ProcessFilesAsync(CancellationToken cancellationToken)
        {
            var jsonFiles = Directory.GetFiles(_inputFolderPath, "*.json");

            if (jsonFiles.Length == 0)
            {
                _logger.LogDebug("No JSON files found in Input folder");
                return;
            }

            foreach (var filePath in jsonFiles)
            {
                await ProcessFileAsync(filePath, cancellationToken);
            }
        }

        private async Task ProcessFileAsync(string filePath, CancellationToken cancellationToken)
        {
            var fileName = Path.GetFileName(filePath);

            try
            {
                _logger.LogInformation("Processing file: {FileName}", fileName);

                // Read and deserialize JSON
                var jsonContent = await File.ReadAllTextAsync(filePath, cancellationToken);

                var companies = JsonSerializer.Deserialize<List<CompanyInputDto>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (companies == null || companies.Count == 0)
                {
                    _logger.LogWarning("Failed to deserialize {FileName}. File is empty or invalid", fileName);
                    MoveToProcessed(filePath, "invalid");
                    return;
                }

                _logger.LogInformation("Found {Count} companies in {FileName}", companies.Count, fileName);

                int successCount = 0;
                int duplicateCount = 0;
                int invalidCount = 0;

                foreach (var companyDto in companies)
                {
                    // Validate required fields
                    if (string.IsNullOrWhiteSpace(companyDto.Name) ||
                        string.IsNullOrWhiteSpace(companyDto.Address) ||
                        string.IsNullOrWhiteSpace(companyDto.City) ||
                        string.IsNullOrWhiteSpace(companyDto.PostalCode) ||
                        string.IsNullOrWhiteSpace(companyDto.Country) ||
                        string.IsNullOrWhiteSpace(companyDto.Phone))
                    {
                        _logger.LogWarning("Required fields missing for company in {FileName}", fileName);
                        invalidCount++;
                        continue;
                    }

                    // Process the company
                    var success = await _handler.HandleAsync(companyDto, cancellationToken);

                    if (success)
                        successCount++;
                    else
                        duplicateCount++;
                }

                _logger.LogInformation("Processed {FileName}: {Success} success, {Duplicate} duplicates, {Invalid} invalid",
                    fileName, successCount, duplicateCount, invalidCount);

                // Determine file status
                string status = invalidCount == companies.Count ? "invalid" :
                                successCount > 0 ? "success" :
                                duplicateCount > 0 ? "duplicate" : "invalid";

                // Move file to processed folder
                MoveToProcessed(filePath, status);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error in file {FileName}", fileName);
                MoveToProcessed(filePath, "error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file {FileName}", fileName);
                MoveToProcessed(filePath, "error");
            }
        }

        private void MoveToProcessed(string sourceFilePath, string status)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
                var extension = Path.GetExtension(sourceFilePath);
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var newFileName = $"{fileName}_{status}_{timestamp}{extension}";
                var destinationPath = Path.Combine(_processedFolderPath, newFileName);

                File.Move(sourceFilePath, destinationPath);
                _logger.LogInformation("Moved file to: {Destination}", destinationPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to move file {SourceFile}", sourceFilePath);
            }
        }
    }
}
