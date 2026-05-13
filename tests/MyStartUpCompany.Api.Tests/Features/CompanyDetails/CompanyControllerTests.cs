using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MyStartUpCompany.Api.Features.CompanyDetails;
using MyStartUpCompany.Api.Features.CompanyDetails.Queries;
using MyStartUpCompany.Api.Shared.Exceptions;

namespace MyStartUpCompany.Api.Tests.Features.CompanyDetails;

public class CompanyControllerTests
{       
    private readonly Mock<IGetCompanyQueryHandler> _getCompanyHandlerMock;
    private readonly Mock<IGetAllCompaniesQueryHandler> _getAllCompaniesHandlerMock;
    private readonly Mock<ILogger<CompanyController>> _loggerMock;
    private readonly CompanyController _controller;

    public CompanyControllerTests()
    {
        _getCompanyHandlerMock = new Mock<IGetCompanyQueryHandler>();
        _getAllCompaniesHandlerMock = new Mock<IGetAllCompaniesQueryHandler>();
        _loggerMock = new Mock<ILogger<CompanyController>>();

        _controller = new CompanyController(
            _getCompanyHandlerMock.Object,
            _getAllCompaniesHandlerMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetCompany_WithValidId_ReturnsOkResultWithCompany()
    {
        // Arrange
        var companyId = 1;
        var expectedCompany = new CompanyDto
        {
            Id = companyId,
            Name = "Test Company",
            Address = "123 Test St",
            City = "Test City",
            PostalCode = "12345",
            Country = "Test Country",
            Phone = "+1-555-1234"
        };

        _getCompanyHandlerMock
            .Setup(h => h.HandleAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCompany);

        // Act
        var result = await _controller.GetCompany(companyId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedCompany);

        _getCompanyHandlerMock.Verify(
            h => h.HandleAsync(companyId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCompany_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var companyId = 999;
        _getCompanyHandlerMock
            .Setup(h => h.HandleAsync(companyId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Company", companyId));

        // Act
        var act = async () => await _controller.GetCompany(companyId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetCompany_WithInvalidId_ThrowsBadRequestException()
    {
        // Arrange
        var invalidId = -1;
        _getCompanyHandlerMock
            .Setup(h => h.HandleAsync(invalidId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BadRequestException("Company ID must be greater than 0"));

        // Act
        var act = async () => await _controller.GetCompany(invalidId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsOkResultWithCompanies()
    {
        // Arrange
        var expectedCompanies = new List<CompanyDto>
        {
            new() { Id = 1, Name = "Company 1", Address = "Address 1", City = "City 1", PostalCode = "12345", Country = "Country 1", Phone = "+1-555-1111" },
            new() { Id = 2, Name = "Company 2", Address = "Address 2", City = "City 2", PostalCode = "67890", Country = "Country 2", Phone = "+1-555-2222" }
        };

        _getAllCompaniesHandlerMock
            .Setup(h => h.HandleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCompanies);

        // Act
        var result = await _controller.GetAllCompanies(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedCompanies);

        _getAllCompaniesHandlerMock.Verify(
            h => h.HandleAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAllCompanies_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        _getAllCompaniesHandlerMock
            .Setup(h => h.HandleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CompanyDto>());

        // Act
        var result = await _controller.GetAllCompanies(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var companies = okResult!.Value as IEnumerable<CompanyDto>;
        companies.Should().BeEmpty();
    }
}