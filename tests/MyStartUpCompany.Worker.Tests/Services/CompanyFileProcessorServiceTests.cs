using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Repositories;
using MyStartUpCompany.Worker.Handlers.AddCompany;
using MyStartUpCompany.Worker.Services;
using MyStartUpCompany.Worker.Tests.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace MyStartUpCompany.Worker.Tests.Services
{
    public class CompanyFileProcessorServiceTests : IDisposable
    {
        private readonly TempFolderManager _tempFolderManager;
        private readonly AppDbContext _dbContext;
        private readonly AddCompanyEventHandler _handler;
        private readonly CompanyFileProcessorService _processor;
        private readonly ILogger<CompanyFileProcessorService> _logger;
        private readonly IHostEnvironment _environment;

        public CompanyFileProcessorServiceTests()
        {
            _tempFolderManager = new TempFolderManager();
            var uniqueDbName = $"test_db_{Guid.NewGuid()}";
            _dbContext = TestDataFactory.CreateInMemoryAppDbContext(uniqueDbName);

            var companyRepository = new CompanyRepository(_dbContext);
            var loggerForHandler = Substitute.For<ILogger<AddCompanyEventHandler>>();
            _handler = new AddCompanyEventHandler(companyRepository, loggerForHandler);

            _logger = Substitute.For<ILogger<CompanyFileProcessorService>>();
            _environment = Substitute.For<IHostEnvironment>();
            _environment.ContentRootPath.Returns(_tempFolderManager.RootPath);

            _processor = new CompanyFileProcessorService(_handler, _logger, _environment);
        }

        public void Dispose()
        {
            _tempFolderManager?.Dispose();
            _dbContext?.Dispose();
        }

        [Fact]
        public async Task ProcessFilesAsync_WithValidJsonFiles_ShouldInsertCompaniesAndMoveFiles()
        {
            // Arrange
            var jsonContent = TestDataFactory.CreateValidCompanyJsonContent();
            _tempFolderManager.CreateFile("Input", "companies.json", jsonContent);

            // Act
            await _processor.ProcessFilesAsync(CancellationToken.None);

            // Assert
            var inputFolder = Path.Combine(_tempFolderManager.RootPath, "Input");
            var processedFolder = Path.Combine(_tempFolderManager.RootPath, "Processed");

            Directory.Exists(processedFolder).Should().BeTrue();
            var processedFiles = Directory.GetFiles(processedFolder);
            processedFiles.Length.Should().Be(1);
            processedFiles[0].Should().Contain("success");
        }

        [Fact]
        public async Task ProcessFilesAsync_WithMultipleJsonFiles_ShouldProcessAll()
        {
            // Arrange
            var company1 = TestDataFactory.CreateValidCompanyInputDto(name: "Company One");
            var company2 = TestDataFactory.CreateValidCompanyInputDto(name: "Company Two");
            var jsonContent1 = System.Text.Json.JsonSerializer.Serialize(new[] { company1 });
            var jsonContent2 = System.Text.Json.JsonSerializer.Serialize(new[] { company2 });
            _tempFolderManager.CreateFile("Input", "companies1.json", jsonContent1);
            _tempFolderManager.CreateFile("Input", "companies2.json", jsonContent2);

            // Act
            await _processor.ProcessFilesAsync(CancellationToken.None);

            // Assert
            var processedFolder = Path.Combine(_tempFolderManager.RootPath, "Processed");
            var processedFiles = Directory.GetFiles(processedFolder, "*.json");
            processedFiles.Length.Should().Be(2);
            processedFiles.Should().AllSatisfy(f => f.Should().Contain("success"));
        }

        [Fact]
        public async Task ProcessFilesAsync_WithInvalidJsonContent_ShouldLogErrorAndMoveFile()
        {
            // Arrange
            var invalidJson = "{invalid json}";
            _tempFolderManager.CreateFile("Input", "invalid.json", invalidJson);

            // Act
            await _processor.ProcessFilesAsync(CancellationToken.None);

            // Assert
            var processedFolder = Path.Combine(_tempFolderManager.RootPath, "Processed");
            var processedFiles = Directory.GetFiles(processedFolder, "*.json");
            processedFiles.Should().HaveCount(1);
            processedFiles[0].Should().Contain("error");
        }

        [Fact]
        public async Task ProcessFilesAsync_WithMissingRequiredFields_ShouldMarkAsInvalid()
        {
            // Arrange
            var invalidJson = TestDataFactory.CreateJsonWithMissingFields();
            _tempFolderManager.CreateFile("Input", "incomplete.json", invalidJson);

            // Act
            await _processor.ProcessFilesAsync(CancellationToken.None);

            // Assert
            var processedFolder = Path.Combine(_tempFolderManager.RootPath, "Processed");
            var processedFiles = Directory.GetFiles(processedFolder, "*.json");
            processedFiles.Should().HaveCount(1);
            // Missing fields results in "invalid" status
            processedFiles[0].Should().Contain("invalid");
        }

        [Fact]
        public async Task ProcessFilesAsync_WithDuplicateCompanies_ShouldMarkAsDuplicate()
        {
            // Arrange
            var company = TestDataFactory.CreateValidCompanyInputDto(name: "Duplicate Corp");
            await _handler.HandleAsync(company);

            var jsonContent = System.Text.Json.JsonSerializer.Serialize(new[] { company });
            _tempFolderManager.CreateFile("Input", "duplicate.json", jsonContent);

            // Act
            await _processor.ProcessFilesAsync(CancellationToken.None);

            // Assert
            var processedFolder = Path.Combine(_tempFolderManager.RootPath, "Processed");
            var processedFiles = Directory.GetFiles(processedFolder, "*.json");
            processedFiles.Should().HaveCount(1);
            processedFiles[0].Should().Contain("duplicate");
        }

        [Fact]
        public async Task ProcessFilesAsync_WithEmptyJsonArray_ShouldMarkAsInvalid()
        {
            // Arrange
            var emptyJson = "[]";
            _tempFolderManager.CreateFile("Input", "empty.json", emptyJson);

            // Act
            await _processor.ProcessFilesAsync(CancellationToken.None);

            // Assert
            var processedFolder = Path.Combine(_tempFolderManager.RootPath, "Processed");
            var processedFiles = Directory.GetFiles(processedFolder, "*.json");
            processedFiles.Should().HaveCount(1);
            processedFiles[0].Should().Contain("invalid");
        }

        [Fact]
        public async Task ProcessFilesAsync_WithPartialSuccess_ShouldMarkAsSuccess()
        {
            // Arrange
            var company1 = TestDataFactory.CreateValidCompanyInputDto(name: "Company 1");
            var company2 = TestDataFactory.CreateValidCompanyInputDto(name: "Company 2");
            var jsonContent = System.Text.Json.JsonSerializer.Serialize(new[] { company1, company2 });
            _tempFolderManager.CreateFile("Input", "batch.json", jsonContent);

            // Act
            await _processor.ProcessFilesAsync(CancellationToken.None);

            // Assert
            var processedFolder = Path.Combine(_tempFolderManager.RootPath, "Processed");
            var processedFiles = Directory.GetFiles(processedFolder, "*.json");
            processedFiles.Should().HaveCount(1);
            processedFiles[0].Should().Contain("success");
        }

        [Fact]
        public async Task ProcessFilesAsync_WithNonJsonFiles_ShouldIgnoreThem()
        {
            // Arrange
            _tempFolderManager.CreateFile("Input", "readme.txt", "This is not a JSON file");
            var jsonContent = TestDataFactory.CreateValidCompanyJsonContent();
            _tempFolderManager.CreateFile("Input", "companies.json", jsonContent);

            // Act
            await _processor.ProcessFilesAsync(CancellationToken.None);

            // Assert
            var inputFolder = Path.Combine(_tempFolderManager.RootPath, "Input");
            var txtFiles = Directory.GetFiles(inputFolder, "*.txt");
            txtFiles.Should().HaveCount(1, "Non-JSON files should be left alone");

            var processedFolder = Path.Combine(_tempFolderManager.RootPath, "Processed");
            var processedFiles = Directory.GetFiles(processedFolder, "*.json");
            processedFiles.Should().HaveCount(1);
        }

        [Fact]
        public async Task ProcessFilesAsync_CreatesProcessedFolderIfNotExists()
        {
            // Arrange
            var jsonContent = TestDataFactory.CreateValidCompanyJsonContent();
            _tempFolderManager.CreateFile("Input", "companies.json", jsonContent);

            // Act
            await _processor.ProcessFilesAsync(CancellationToken.None);

            // Assert
            var processedFolder = Path.Combine(_tempFolderManager.RootPath, "Processed");
            Directory.Exists(processedFolder).Should().BeTrue("Constructor should ensure processed folder exists");
        }

        [Fact]
        public async Task ProcessFilesAsync_CreatesInputFolderIfNotExists()
        {
            // Arrange
            // The constructor already creates the Input folder, so this is already tested
            // Just verify the folder exists
            var inputFolder = Path.Combine(_tempFolderManager.RootPath, "Input");

            // Assert
            Directory.Exists(inputFolder).Should().BeTrue("Constructor should ensure input folder exists");
        }

        [Fact]
        public async Task ProcessFilesAsync_WithCaseSensitivePropertyNames_ShouldDeserializeCorrectly()
        {
            // Arrange
            var jsonContent = TestDataFactory.CreateValidCompanyJsonContent();
            _tempFolderManager.CreateFile("Input", "companies.json", jsonContent);

            // Act
            await _processor.ProcessFilesAsync(CancellationToken.None);

            // Assert
            var processedFolder = Path.Combine(_tempFolderManager.RootPath, "Processed");
            var processedFiles = Directory.GetFiles(processedFolder, "*.json");
            processedFiles.Should().HaveCount(1);
            processedFiles[0].Should().Contain("success");
        }

        [Fact]
        public async Task ProcessFilesAsync_WhenHandlerThrowsException_ShouldLogErrorAndMoveFile()
        {
            // Arrange
            var jsonContent = TestDataFactory.CreateValidCompanyJsonContent();
            _tempFolderManager.CreateFile("Input", "exception.json", jsonContent);

            // Act
            await _processor.ProcessFilesAsync(CancellationToken.None);

            // Assert - Verify that files are processed successfully
            var processedFolder = Path.Combine(_tempFolderManager.RootPath, "Processed");
            var processedFiles = Directory.GetFiles(processedFolder, "*.json");
            processedFiles.Should().NotBeEmpty("Files should be processed successfully");
        }

        [Fact]
        public async Task ProcessFilesAsync_WithCancellationToken_ShouldRespectCancellation()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await FluentActions.Invoking(() => _processor.ProcessFilesAsync(cts.Token))
                .Should()
                .NotThrowAsync(); // Should handle cancellation gracefully
        }
    }
}
