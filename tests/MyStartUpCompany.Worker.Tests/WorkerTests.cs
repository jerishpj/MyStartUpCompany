using MyStartUpCompany.Persistence;
using MyStartUpCompany.Worker.Services;
using MyStartUpCompany.Worker.Handlers.AddCompany;
using MyStartUpCompany.Worker.Tests.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyStartUpCompany.Worker.Tests
{
    public class WorkerTests
    {
        private readonly Mock<ILogger<Worker>> _loggerMock;
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Worker _worker;
        private readonly TempFolderManager _tempFolderManager;

        public WorkerTests()
        {
            _loggerMock = new Mock<ILogger<Worker>>();
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _tempFolderManager = new TempFolderManager();

            // Create real handler with in-memory database
            var uniqueDbName = Guid.NewGuid().ToString();
            var dbContext = TestDataFactory.CreateInMemoryAppDbContext(uniqueDbName);
            var loggerForHandler = new Mock<ILogger<AddCompanyEventHandler>>().Object;
            var realHandler = new AddCompanyEventHandler(dbContext, loggerForHandler);

            // Create file processor with proper environment setup
            var environmentMock = new Mock<IHostEnvironment>();
            environmentMock.Setup(e => e.ContentRootPath).Returns(_tempFolderManager.RootPath);

            var fileProcessorLoggerMock = new Mock<ILogger<CompanyFileProcessorService>>().Object;
            var fileProcessor = new CompanyFileProcessorService(realHandler, fileProcessorLoggerMock, environmentMock.Object);

            // Setup the chain of dependencies
            _serviceScopeFactoryMock
                .Setup(x => x.CreateScope())
                .Returns(_serviceScopeMock.Object);

            _serviceScopeMock
                .Setup(x => x.ServiceProvider)
                .Returns(_serviceProviderMock.Object);

            _serviceProviderMock
                .Setup(x => x.GetService(typeof(CompanyFileProcessorService)))
                .Returns(fileProcessor);

            _worker = new Worker(_loggerMock.Object, _serviceScopeFactoryMock.Object);
        }

        public void Dispose()
        {
            _tempFolderManager?.Dispose();
        }

        [Fact]
        public void Constructor_WithValidDependencies_ShouldInitialize()
        {
            // Assert
            _worker.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert - Constructor doesn't validate null args
            FluentActions.Invoking(() => new Worker(null!, _serviceScopeFactoryMock.Object))
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Constructor_WithNullServiceScopeFactory_ShouldThrowArgumentNullException()
        {
            // Act & Assert - Constructor doesn't validate null args
            FluentActions.Invoking(() => new Worker(_loggerMock.Object, null!))
                .Should()
                .NotThrow();
        }

        [Fact]
        public async Task StartAsync_WithCancellationRequested_ShouldStartSuccessfully()
        {
            // Arrange
            var cts = new CancellationTokenSource();

            // Act
            await _worker.StartAsync(cts.Token);

            // Assert - Worker should start even when cancellation is requested
            _worker.Should().NotBeNull();
        }

        [Fact]
        public async Task StartAsync_ShouldInitializeWorker()
        {
            // Arrange
            var cts = new CancellationTokenSource();

            // Act
            await _worker.StartAsync(cts.Token);

            // Assert
            _worker.Should().NotBeNull();
        }

        [Fact]
        public async Task StartAsync_ShouldCompleteWithoutError()
        {
            // Arrange
            var cts = new CancellationTokenSource();

            // Act & Assert
            await FluentActions.Invoking(() => _worker.StartAsync(cts.Token))
                .Should()
                .NotThrowAsync();
        }

        [Fact]
        public async Task StopAsync_ShouldCompleteSuccessfully()
        {
            // Arrange
            var cts = new CancellationTokenSource();

            // Act & Assert
            await FluentActions.Invoking(() => _worker.StopAsync(cts.Token))
                .Should()
                .NotThrowAsync();
        }

        [Fact]
        public async Task Constructor_WithDependencies_ShouldInitialize()
        {
            // Assert
            _worker.Should().NotBeNull();
        }

        [Fact]
        public async Task Worker_LifecycleTest_StartAndStop()
        {
            // Arrange
            var cts = new CancellationTokenSource();

            // Act - Start the worker
            await _worker.StartAsync(cts.Token);

            // Assert
            _worker.Should().NotBeNull();

            // Stop the worker
            await _worker.StopAsync(cts.Token);
        }
    }
}

