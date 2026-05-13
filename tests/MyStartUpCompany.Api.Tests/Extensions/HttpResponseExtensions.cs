using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace MyStartUpCompany.Api.Tests.Extensions;

/// <summary>
/// Extension methods for HTTP response assertions
/// </summary>
public static class HttpResponseExtensions
{
    public static async Task<string> ShouldBeOkWithContent(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
        return content;
    }

    public static void ShouldBeNotFound(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public static void ShouldBeCreated(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    public static void ShouldBeBadRequest(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public static void ShouldBeNoContent(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    public static async Task<T?> ReadAsJsonAsync<T>(this HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public static async Task ShouldContainInContent(this HttpResponseMessage response, params string[] expectedValues)
    {
        var content = await response.Content.ReadAsStringAsync();
        foreach (var expectedValue in expectedValues)
        {
            content.Should().Contain(expectedValue);
        }
    }
}