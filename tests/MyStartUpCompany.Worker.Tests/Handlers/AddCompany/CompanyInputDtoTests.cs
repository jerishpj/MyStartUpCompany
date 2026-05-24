using MyStartUpCompany.Worker.Handlers.AddCompany;
using MyStartUpCompany.Worker.Tests.Utilities;

namespace MyStartUpCompany.Worker.Tests.Handlers.AddCompany
{
    public class CompanyInputDtoTests
    {
        [Fact]
        public void CreateValidCompanyInputDto_WithAllRequiredFields_ShouldBeValid()
        {
            // Arrange & Act
            var company = TestDataFactory.CreateValidCompanyInputDto(
                name: "Test Corp",
                address: "123 Main St",
                city: "Springfield",
                postalCode: "12345",
                country: "USA",
                phone: "555-0100"
            );

            // Assert
            company.Name.Should().Be("Test Corp");
            company.Address.Should().Be("123 Main St");
            company.City.Should().Be("Springfield");
            company.PostalCode.Should().Be("12345");
            company.Country.Should().Be("USA");
            company.Phone.Should().Be("555-0100");
        }

        [Fact]
        public void CreateCompanyInputDto_WithOptionalFields_ShouldIncludeDescription()
        {
            // Arrange & Act
            var company = TestDataFactory.CreateValidCompanyInputDto(
                description: "A tech company",
                region: "California"
            );

            // Assert
            company.Description.Should().Be("A tech company");
            company.Region.Should().Be("California");
        }

        [Fact]
        public void CreateCompanyInputDto_WithoutOptionalFields_ShouldHaveNullValues()
        {
            // Arrange & Act
            var company = TestDataFactory.CreateValidCompanyInputDto();

            // Assert
            company.Description.Should().BeNull();
            company.Region.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CreateInvalidCompanyInputDto_WithEmptyOrWhitespaceName_ShouldBehaveAsEmpty(string? name)
        {
            // Arrange & Act
            var company = new CompanyInputDto
            {
                Name = name ?? "",
                Address = "123 Main St",
                City = "Springfield",
                PostalCode = "12345",
                Country = "USA",
                Phone = "555-0100"
            };

            // Assert
            string.IsNullOrWhiteSpace(company.Name).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateInvalidCompanyInputDto_WithEmptyOrWhitespaceAddress_ShouldBehaveAsEmpty(string address)
        {
            // Arrange & Act
            var company = new CompanyInputDto
            {
                Name = "Test Corp",
                Address = address,
                City = "Springfield",
                PostalCode = "12345",
                Country = "USA",
                Phone = "555-0100"
            };

            // Assert
            string.IsNullOrWhiteSpace(company.Address).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateInvalidCompanyInputDto_WithEmptyOrWhitespaceCity_ShouldBehaveAsEmpty(string city)
        {
            // Arrange & Act
            var company = new CompanyInputDto
            {
                Name = "Test Corp",
                Address = "123 Main St",
                City = city,
                PostalCode = "12345",
                Country = "USA",
                Phone = "555-0100"
            };

            // Assert
            string.IsNullOrWhiteSpace(company.City).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateInvalidCompanyInputDto_WithEmptyOrWhitespacePostalCode_ShouldBehaveAsEmpty(string postalCode)
        {
            // Arrange & Act
            var company = new CompanyInputDto
            {
                Name = "Test Corp",
                Address = "123 Main St",
                City = "Springfield",
                PostalCode = postalCode,
                Country = "USA",
                Phone = "555-0100"
            };

            // Assert
            string.IsNullOrWhiteSpace(company.PostalCode).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateInvalidCompanyInputDto_WithEmptyOrWhitespaceCountry_ShouldBehaveAsEmpty(string country)
        {
            // Arrange & Act
            var company = new CompanyInputDto
            {
                Name = "Test Corp",
                Address = "123 Main St",
                City = "Springfield",
                PostalCode = "12345",
                Country = country,
                Phone = "555-0100"
            };

            // Assert
            string.IsNullOrWhiteSpace(company.Country).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateInvalidCompanyInputDto_WithEmptyOrWhitespacePhone_ShouldBehaveAsEmpty(string phone)
        {
            // Arrange & Act
            var company = new CompanyInputDto
            {
                Name = "Test Corp",
                Address = "123 Main St",
                City = "Springfield",
                PostalCode = "12345",
                Country = "USA",
                Phone = phone
            };

            // Assert
            string.IsNullOrWhiteSpace(company.Phone).Should().BeTrue();
        }

        [Fact]
        public void CreateMultipleCompanyInputDtos_WithDifferentData_ShouldBeDistinct()
        {
            // Arrange & Act
            var company1 = TestDataFactory.CreateValidCompanyInputDto(name: "Company A");
            var company2 = TestDataFactory.CreateValidCompanyInputDto(name: "Company B");

            // Assert
            company1.Name.Should().NotBe(company2.Name);
            company1.Phone.Should().Be(company2.Phone); // Same default phone
        }

        [Fact]
        public void CreateCompanyBatch_WithMultipleCompanies_ShouldAllBeValid()
        {
            // Arrange & Act
            var batch = TestDataFactory.CreateValidCompanyBatch(5);

            // Assert
            batch.Should().HaveCount(5);
            batch.Should().AllSatisfy(c => c.Name.Should().NotBeNullOrWhiteSpace());
            batch.Should().AllSatisfy(c => c.Address.Should().NotBeNullOrWhiteSpace());
            batch.Should().AllSatisfy(c => c.City.Should().NotBeNullOrWhiteSpace());
        }
    }
}
