using Microsoft.Extensions.Logging;
using NSubstitute;

namespace MyStartUpCompany.Api.Tests.Shared.Helpers;

public static class LoggerMock
{
    public static ILogger<T> Create<T>()
    {
        return Substitute.For<ILogger<T>>();
    }
}
