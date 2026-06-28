using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using MyStartUpCompany.Api.Features.CompanyDetails;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Api.Features.CompanyDetails.Queries;
using MyStartUpCompany.Api.Shared.Exceptions;

namespace MyStartUpCompany.Api.Tests.Features.CompanyDetails;

public class CompanyControllerTests
{       
    private readonly IGetCompanyQueryHandler _getCompanyHandler;
    private readonly IGetAllCompaniesQueryHandler _getAllCompaniesHandler;
    private readonly IGetFilteredCompaniesQueryHandler _getFilteredCompaniesHandler;
    private readonly ILogger<CompanyController> _logger;
    private readonly CompanyController _controller;

    public CompanyControllerTests()
    {
        _getCompanyHandler = Substitute.For<IGetCompanyQueryHandler>();
        _getAllCompaniesHandler = Substitute.For<IGetAllCompaniesQueryHandler>();
        _getFilteredCompaniesHandler = Substitute.For<IGetFilteredCompaniesQueryHandler>();
        _logger = Substitute.For<ILogger<CompanyController>>();

        _controller = new CompanyController(
            _getCompanyHandler,
            _getAllCompaniesHandler,
            _getFilteredCompaniesHandler,
            _logger);
    }

    [Fact]
    public async Task GetCompany_WithValidId_ReturnsOkResultWithCompany()
    {
        // Arrange
        var companyId = 1;
        var expectedCompany = new CompanyResponse
        {
            Id = companyId,
            Name = "Test Company",
            Address = "123 Test St",
            City = "Test City",
            PostalCode = "12345",
            Country = "Test Country",
            Phone = "+1-555-1234"
        };

        _getCompanyHandler
            .HandleAsync(companyId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedCompany));

        // Act
        var result = await _controller.GetCompany(companyId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedCompany);

        await _getCompanyHandler.Received(1).HandleAsync(companyId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCompany_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var companyId = 999;
        _getCompanyHandler
            .HandleAsync(companyId, Arg.Any<CancellationToken>())
            .Returns(x => Task.FromException<CompanyResponse>(new NotFoundException("Company", companyId)));

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
        _getCompanyHandler
            .HandleAsync(invalidId, Arg.Any<CancellationToken>())
            .Returns(x => Task.FromException<CompanyResponse>(new BadRequestException("Company ID must be greater than 0")));

        // Act
        var act = async () => await _controller.GetCompany(invalidId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsOkResultWithCompanies()
    {
        // Arrange
        var expectedCompanies = new List<CompanyResponse>
        {
            new() { Id = 1, Name = "Company 1", Address = "Address 1", City = "City 1", PostalCode = "12345", Country = "Country 1", Phone = "+1-555-1111" },
            new() { Id = 2, Name = "Company 2", Address = "Address 2", City = "City 2", PostalCode = "67890", Country = "Country 2", Phone = "+1-555-2222" }
        };

        _getAllCompaniesHandler
            .HandleAsync(Arg.Any<CancellationToken>())
            .Returns(x => Task.FromResult((IEnumerable<CompanyResponse>)expectedCompanies));

        // Act
        var result = await _controller.GetAllCompanies(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedCompanies);

        await _getAllCompaniesHandler.Received(1).HandleAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllCompanies_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        _getAllCompaniesHandler
            .HandleAsync(Arg.Any<CancellationToken>())
            .Returns(x => Task.FromResult((IEnumerable<CompanyResponse>)new List<CompanyResponse>()));

        // Act
        var result = await _controller.GetAllCompanies(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var companies = okResult!.Value as IEnumerable<CompanyResponse>;
        companies.Should().BeEmpty();
    }
}
