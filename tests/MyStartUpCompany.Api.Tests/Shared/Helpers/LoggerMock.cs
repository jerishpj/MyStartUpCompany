using Microsoft.Extensions.Logging;
using Moq;

namespace MyStartUpCompany.Api.Tests.Shared.Helpers;

public static class LoggerMock
{
    public static ILogger<T> Create<T>()
    {
        return new Mock<ILogger<T>>().Object;
    }
}