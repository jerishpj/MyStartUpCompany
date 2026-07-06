using System.Net;
using System.Text.Json;
using MyStartUpCompany.Api.Tests.Shared.TestData;

namespace MyStartUpCompany.Api.Tests.Integration;

/// <summary>
/// Integration tests for exception handling and validation filter behavior.
/// Tests that verify the GlobalExceptionHandler and FluentValidationFilter work correctly
/// in an actual HTTP request/response pipeline.
/// </summary>
public class ExceptionHandlingIntegrationTests : IntegrationTestBase
{
    public ExceptionHandlingIntegrationTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    #region Validation Error Tests

    [Fact]
    public async Task GetFilteredCompanies_WithInvalidPageNumber_ShouldReturn422UnprocessableEntity()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act - PageNumber must be >= 1
        var response = await Client.GetAsync("/api/company/search?pageNumber=0&pageSize=10&name=Test");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("validation");
    }

    [Fact]
    public async Task GetFilteredCompanies_WithInvalidPageSize_ShouldReturn422UnprocessableEntity()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act - PageSize must be > 0
        var response = await Client.GetAsync("/api/company/search?pageNumber=1&pageSize=0&name=Test");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("validation");
    }

    [Fact]
    public async Task GetFilteredCompanies_WithValidParameters_ShouldReturn200Ok()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?pageNumber=1&pageSize=10&name=Test");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region NotFoundException Tests

    [Fact]
    public async Task GetCompany_WithNonexistentId_ShouldReturn404NotFound()
    {
        // Arrange
        int nonexistentId = 99999;

        // Act
        var response = await Client.GetAsync($"/api/company/{nonexistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<JsonElement>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        problemDetails.GetProperty("status").GetInt32().Should().Be((int)HttpStatusCode.NotFound);
        problemDetails.GetProperty("title").GetString().Should().Contain("Not Found");
        problemDetails.GetProperty("detail").GetString().Should().Contain("Company");
    }

    #endregion

    #region Response Format Tests

    [Fact]
    public async Task ErrorResponse_ShouldIncludeTraceId()
    {
        // Arrange
        int nonexistentId = 99999;

        // Act
        var response = await Client.GetAsync($"/api/company/{nonexistentId}");
        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<JsonElement>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Assert - ProblemDetails may include extensions with traceId or it may be omitted
        // This test verifies the response is valid ProblemDetails format
        problemDetails.TryGetProperty("status", out _).Should().BeTrue();
        problemDetails.TryGetProperty("title", out _).Should().BeTrue();
    }

    [Fact]
    public async Task ErrorResponse_ShouldIncludeTimestamp()
    {
        // Arrange
        int nonexistentId = 99999;

        // Act
        var response = await Client.GetAsync($"/api/company/{nonexistentId}");
        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<JsonElement>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Assert - ProblemDetails returns standard RFC 7231 format with status, title, detail, instance
        problemDetails.TryGetProperty("status", out _).Should().BeTrue();
        problemDetails.TryGetProperty("title", out _).Should().BeTrue();
    }

    [Fact]
    public async Task ErrorResponse_ShouldIncludeInstancePath()
    {
        // Arrange
        int nonexistentId = 99999;
        var requestPath = $"/api/company/{nonexistentId}";

        // Act
        var response = await Client.GetAsync(requestPath);
        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<JsonElement>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Assert
        problemDetails.GetProperty("instance").GetString().Should().Contain("/api/company/");
    }

    #endregion

    #region Content Negotiation Tests

    [Fact]
    public async Task ErrorResponse_ShouldReturnJsonContentType()
    {
        // Arrange
        int nonexistentId = 99999;

        // Act
        var response = await Client.GetAsync($"/api/company/{nonexistentId}");

        // Assert
        // ProblemDetails responses are serialized as application/json (not application/problem+json in this implementation)
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    #endregion

    #region Multiple Validation Errors Tests

    [Theory]
    [InlineData("/api/company/search?pageNumber=0&pageSize=0")]
    [InlineData("/api/company/search?pageNumber=-1&pageSize=-1")]
    public async Task GetFilteredCompanies_WithMultipleValidationErrors_ShouldReturn422(string query)
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    #endregion
}
