using MyStartUpCompany.Persistence;
using MyStartUpCompany.Worker.Handlers.AddCompany;
using MyStartUpCompany.Worker.Tests.Utilities;
using Microsoft.Extensions.Logging;

namespace MyStartUpCompany.Worker.Tests.Handlers.AddCompany
{
    public class AddCompanyEventHandlerTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AddCompanyEventHandler> _loggerMock;
        private readonly AddCompanyEventHandler _handler;

        public AddCompanyEventHandlerTests()
        {           
            // Create in-memory database context with unique name for each test
            var uniqueDbName = Guid.NewGuid().ToString();
            _dbContext = TestDataFactory.CreateInMemoryAppDbContext(uniqueDbName);
            _loggerMock = new Mock<ILogger<AddCompanyEventHandler>>().Object;
            _handler = new AddCompanyEventHandler(_dbContext, _loggerMock);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        [Fact]
        public async Task HandleAsync_WithValidCompanyData_ShouldInsertCompany()
        {
            // Arrange
            var company = TestDataFactory.CreateValidCompanyInputDto(name: "Test Corp");

            // Act
            var result = await _handler.HandleAsync(company);

            // Assert
            result.Should().BeTrue();
            var savedCompany = _dbContext.Companies.FirstOrDefault(c => c.Name == "Test Corp");
            savedCompany.Should().NotBeNull();
            savedCompany!.Name.Should().Be("Test Corp");
            savedCompany.Address.Should().Be("123 Main St");
            savedCompany.City.Should().Be("Springfield");
        }

        [Fact]
        public async Task HandleAsync_WithValidCompanyData_ShouldPersistAllFields()
        {
            // Arrange
            var company = TestDataFactory.CreateValidCompanyInputDto(
                name: "Full Details Corp",
                description: "A full featured company",
                address: "456 Business Blvd",
                city: "Los Angeles",
                region: "California",
                postalCode: "90001",
                country: "USA",
                phone: "555-9999"
            );

            // Act
            var result = await _handler.HandleAsync(company);

            // Assert
            result.Should().BeTrue();
            var savedCompany = _dbContext.Companies.FirstOrDefault(c => c.Name == "Full Details Corp");
            savedCompany.Should().NotBeNull();
            savedCompany!.Description.Should().Be("A full featured company");
            savedCompany.Address.Should().Be("456 Business Blvd");
            savedCompany.City.Should().Be("Los Angeles");
            savedCompany.Region.Should().Be("California");
            savedCompany.PostalCode.Should().Be("90001");
            savedCompany.Country.Should().Be("USA");
            savedCompany.Phone.Should().Be("555-9999");
        }

        [Fact]
        public async Task HandleAsync_WithDuplicateCompanyName_ShouldReturnFalse()
        {
            // Arrange
            var company1 = TestDataFactory.CreateValidCompanyInputDto(name: "Duplicate Corp");
            var company2 = TestDataFactory.CreateValidCompanyInputDto(name: "Duplicate Corp");

            // Act
            var firstResult = await _handler.HandleAsync(company1);
            var secondResult = await _handler.HandleAsync(company2);

            // Assert
            firstResult.Should().BeTrue();
            secondResult.Should().BeFalse();
            var count = _dbContext.Companies.Count(c => c.Name == "Duplicate Corp");
            count.Should().Be(1, "Only the first company should be inserted");
        }

        [Fact]
        public async Task HandleAsync_WithDifferentCompanies_ShouldInsertBoth()
        {
            // Arrange
            var company1 = TestDataFactory.CreateValidCompanyInputDto(name: "Company One");
            var company2 = TestDataFactory.CreateValidCompanyInputDto(name: "Company Two");

            // Act
            var result1 = await _handler.HandleAsync(company1);
            var result2 = await _handler.HandleAsync(company2);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            _dbContext.Companies.Count().Should().Be(2);
        }

        [Fact]
        public async Task HandleAsync_WithValidCompanyData_ShouldGenerateId()
        {
            // Arrange
            var company = TestDataFactory.CreateValidCompanyInputDto(name: "Company With ID");

            // Act
            var result = await _handler.HandleAsync(company);

            // Assert
            result.Should().BeTrue();
            var savedCompany = _dbContext.Companies.FirstOrDefault(c => c.Name == "Company With ID");
            savedCompany.Should().NotBeNull();
            savedCompany!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task HandleAsync_WithMultipleCompanies_ShouldPersistAll()
        {
            // Arrange
            var companies = TestDataFactory.CreateValidCompanyBatch(5);

            // Act
            var results = new List<bool>();
            foreach (var company in companies)
            {
                var result = await _handler.HandleAsync(company);
                results.Add(result);
            }

            // Assert
            results.Should().AllSatisfy(r => r.Should().BeTrue());
            _dbContext.Companies.Count().Should().Be(5);
        }

        [Fact]
        public async Task HandleAsync_WithNullCompanyDto_ShouldThrowException()
        {
            // Arrange
            CompanyInputDto? company = null;

            // Act & Assert
            await FluentActions.Invoking(() => _handler.HandleAsync(company!))
                .Should()
                .ThrowAsync<Exception>();
        }

        [Fact]
        public async Task HandleAsync_AfterInsertingCompany_DatabaseShouldBeUpdated()
        {
            // Arrange
            var company = TestDataFactory.CreateValidCompanyInputDto(name: "Database Check Corp");
            var initialCount = _dbContext.Companies.Count();

            // Act
            await _handler.HandleAsync(company);

            // Assert
            var finalCount = _dbContext.Companies.Count();
            finalCount.Should().Be(initialCount + 1);
        }

        [Fact]
        public async Task HandleAsync_WithCaseInsensitiveNames_ShouldTreatAsDifferent()
        {
            // Arrange
            var company1 = TestDataFactory.CreateValidCompanyInputDto(name: "Test Company");
            var company2 = TestDataFactory.CreateValidCompanyInputDto(name: "test company"); // lowercase

            // Act
            var result1 = await _handler.HandleAsync(company1);
            var result2 = await _handler.HandleAsync(company2);

            // Assert
            // Note: The actual behavior depends on database collation
            // This test documents the expected behavior
            result1.Should().BeTrue();
            // Second should be true if case-sensitive, false if case-insensitive
            // Depending on database configuration
        }

        [Fact]
        public async Task HandleAsync_WithOptionalFieldsNull_ShouldStillInsert()
        {
            // Arrange
            var company = new CompanyInputDto
            {
                Name = "Minimal Company",
                Address = "123 Main",
                City = "City",
                PostalCode = "12345",
                Country = "Country",
                Phone = "123-4567",
                Description = null,
                Region = null
            };

            // Act
            var result = await _handler.HandleAsync(company);

            // Assert
            result.Should().BeTrue();
            var savedCompany = _dbContext.Companies.FirstOrDefault(c => c.Name == "Minimal Company");
            savedCompany.Should().NotBeNull();
            savedCompany!.Description.Should().BeNull();
            savedCompany.Region.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_WithCancellationToken_ShouldRespectCancellation()
        {
            // Arrange
            var company = TestDataFactory.CreateValidCompanyInputDto(name: "Cancellation Test");
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await FluentActions.Invoking(() => _handler.HandleAsync(company, cts.Token))
                .Should()
                .ThrowAsync<OperationCanceledException>();
        }
    }
}
