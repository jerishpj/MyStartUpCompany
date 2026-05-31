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

        #region Name Field Length Tests

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(255)]
        public void CreateCompanyInputDto_WithVariousNameLengths_ShouldStore(int length)
        {
            // Arrange
            var name = new string('T', length);

            // Act
            var company = TestDataFactory.CreateValidCompanyInputDto(name: name);

            // Assert
            company.Name.Should().Be(name);
            company.Name.Length.Should().Be(length);
        }

        #endregion

        #region Address Field Length Tests

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(200)]
        [InlineData(500)]
        public void CreateCompanyInputDto_WithVariousAddressLengths_ShouldStore(int length)
        {
            // Arrange
            var address = new string('A', length);

            // Act
            var company = TestDataFactory.CreateValidCompanyInputDto(address: address);

            // Assert
            company.Address.Should().Be(address);
            company.Address.Length.Should().Be(length);
        }

        #endregion

        #region City Field Tests

        [Theory]
        [InlineData("San Francisco")]
        [InlineData("New York")]
        [InlineData("Los Angeles")]
        [InlineData("Salt Lake City")]
        public void CreateCompanyInputDto_WithVariousCities_ShouldStore(string city)
        {
            // Arrange & Act
            var company = TestDataFactory.CreateValidCompanyInputDto(city: city);

            // Assert
            company.City.Should().Be(city);
        }

        #endregion

        #region Region Field Tests

        [Theory]
        [InlineData("California")]
        [InlineData("New York")]
        [InlineData("Texas")]
        [InlineData(null)]
        [InlineData("")]
        public void CreateCompanyInputDto_WithVariousRegions_ShouldStore(string? region)
        {
            // Arrange & Act
            var company = TestDataFactory.CreateValidCompanyInputDto(region: region ?? "DefaultRegion");

            // Assert
            company.Region.Should().Be(region ?? "DefaultRegion");
        }

        #endregion

        #region PostalCode Field Tests

        [Theory]
        [InlineData("12345")]
        [InlineData("12345-6789")]
        [InlineData("M5V 3A8")]
        [InlineData("90210")]
        public void CreateCompanyInputDto_WithVariousPostalCodes_ShouldStore(string postalCode)
        {
            // Arrange & Act
            var company = TestDataFactory.CreateValidCompanyInputDto(postalCode: postalCode);

            // Assert
            company.PostalCode.Should().Be(postalCode);
        }

        #endregion

        #region Country Field Tests

        [Theory]
        [InlineData("USA")]
        [InlineData("United States")]
        [InlineData("Canada")]
        [InlineData("United Kingdom")]
        public void CreateCompanyInputDto_WithVariousCountries_ShouldStore(string country)
        {
            // Arrange & Act
            var company = TestDataFactory.CreateValidCompanyInputDto(country: country);

            // Assert
            company.Country.Should().Be(country);
        }

        #endregion

        #region Phone Field Tests

        [Theory]
        [InlineData("555-0100")]
        [InlineData("+1-555-0100")]
        [InlineData("(555) 0100")]
        [InlineData("555.0100")]
        [InlineData("+1 (555) 0100")]
        public void CreateCompanyInputDto_WithVariousPhoneFormats_ShouldStore(string phone)
        {
            // Arrange & Act
            var company = TestDataFactory.CreateValidCompanyInputDto(phone: phone);

            // Assert
            company.Phone.Should().Be(phone);
        }

        #endregion

        #region Description Field Tests

        [Fact]
        public void CreateCompanyInputDto_WithDescription_ShouldStore()
        {
            // Arrange
            var description = "A leading technology company providing innovative solutions";

            // Act
            var company = TestDataFactory.CreateValidCompanyInputDto(description: description);

            // Assert
            company.Description.Should().Be(description);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        [InlineData(2000)]
        public void CreateCompanyInputDto_WithVariousDescriptionLengths_ShouldStore(int length)
        {
            // Arrange
            var description = new string('D', length);

            // Act
            var company = TestDataFactory.CreateValidCompanyInputDto(description: description);

            // Assert
            company.Description.Should().Be(description);
            company.Description.Length.Should().Be(length);
        }

        #endregion

        #region Required Fields Tests

        [Fact]
        public void CreateCompanyInputDto_AllRequiredFieldsSet_ShouldHaveNoNulls()
        {
            // Arrange & Act
            var company = TestDataFactory.CreateValidCompanyInputDto();

            // Assert
            company.Name.Should().NotBeNullOrWhiteSpace();
            company.Address.Should().NotBeNullOrWhiteSpace();
            company.City.Should().NotBeNullOrWhiteSpace();
            company.PostalCode.Should().NotBeNullOrWhiteSpace();
            company.Country.Should().NotBeNullOrWhiteSpace();
            company.Phone.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateCompanyInputDto_OptionalFieldsCanBeNull_ShouldAllowIt()
        {
            // Arrange & Act
            var company = new CompanyInputDto
            {
                Name = "Test Company",
                Address = "123 Main St",
                City = "Test City",
                Region = null,
                PostalCode = "12345",
                Country = "USA",
                Phone = "555-0100",
                Description = null
            };

            // Assert
            company.Region.Should().BeNull();
            company.Description.Should().BeNull();
        }

        #endregion

        #region Data Integrity Tests

        [Fact]
        public void CreateCompanyInputDto_MultipleFieldsCombined_ShouldMaintainAllData()
        {
            // Arrange
            var name = "Tech Solutions Inc";
            var address = "456 Innovation Drive";
            var city = "San Francisco";
            var region = "California";
            var postalCode = "94105";
            var country = "United States";
            var phone = "+1-415-555-0100";
            var description = "Global technology solutions provider";

            // Act
            var company = TestDataFactory.CreateValidCompanyInputDto(
                name: name,
                address: address,
                city: city,
                region: region,
                postalCode: postalCode,
                country: country,
                phone: phone,
                description: description
            );

            // Assert
            company.Name.Should().Be(name);
            company.Address.Should().Be(address);
            company.City.Should().Be(city);
            company.Region.Should().Be(region);
            company.PostalCode.Should().Be(postalCode);
            company.Country.Should().Be(country);
            company.Phone.Should().Be(phone);
            company.Description.Should().Be(description);
        }

        [Fact]
        public void CreateCompanyBatch_AllCompaniesHaveUniqueNames_ShouldBeDistinct()
        {
            // Arrange & Act
            var batch = TestDataFactory.CreateValidCompanyBatch(5);

            // Assert
            var names = batch.Select(c => c.Name).Distinct().Count();
            names.Should().Be(5);
        }

        [Fact]
        public void CreateCompanyBatch_AllCompaniesHaveRequiredFields_ShouldAllBeValid()
        {
            // Arrange & Act
            var batch = TestDataFactory.CreateValidCompanyBatch(10);

            // Assert
            batch.Should().AllSatisfy(c =>
            {
                c.Name.Should().NotBeNullOrWhiteSpace();
                c.Address.Should().NotBeNullOrWhiteSpace();
                c.City.Should().NotBeNullOrWhiteSpace();
                c.PostalCode.Should().NotBeNullOrWhiteSpace();
                c.Country.Should().NotBeNullOrWhiteSpace();
                c.Phone.Should().NotBeNullOrWhiteSpace();
            });
        }

        #endregion
    }
}
