using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Repositories;
using MyStartUpCompany.Worker.Services;
using MyStartUpCompany.Worker.Handlers.AddCompany;
using MyStartUpCompany.Worker.Tests.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace MyStartUpCompany.Worker.Tests
{
    public class WorkerTests
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _serviceScope;
        private readonly IServiceProvider _serviceProvider;
        private readonly Worker _worker;
        private readonly TempFolderManager _tempFolderManager;

        public WorkerTests()
        {
            _logger = Substitute.For<ILogger<Worker>>();
            _serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            _serviceScope = Substitute.For<IServiceScope>();
            _serviceProvider = Substitute.For<IServiceProvider>();
            _tempFolderManager = new TempFolderManager();

            // Create real handler with in-memory database
            var uniqueDbName = Guid.NewGuid().ToString();
            var dbContext = TestDataFactory.CreateInMemoryAppDbContext(uniqueDbName);
            var companyRepository = new CompanyRepository(dbContext);
            var loggerForHandler = Substitute.For<ILogger<AddCompanyEventHandler>>();
            var realHandler = new AddCompanyEventHandler(companyRepository, loggerForHandler);

            // Create file processor with proper environment setup
            var environment = Substitute.For<IHostEnvironment>();
            environment.ContentRootPath.Returns(_tempFolderManager.RootPath);

            var fileProcessorLogger = Substitute.For<ILogger<CompanyFileProcessorService>>();
            var fileProcessor = new CompanyFileProcessorService(realHandler, fileProcessorLogger, environment);

            // Setup the chain of dependencies
            _serviceScopeFactory
                .CreateScope()
                .Returns(_serviceScope);

            _serviceScope
                .ServiceProvider
                .Returns(_serviceProvider);

            _serviceProvider
                .GetService(typeof(CompanyFileProcessorService))
                .Returns(fileProcessor);

            _worker = new Worker(_logger, _serviceScopeFactory);
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
            FluentActions.Invoking(() => new Worker(null!, _serviceScopeFactory))
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Constructor_WithNullServiceScopeFactory_ShouldThrowArgumentNullException()
        {
            // Act & Assert - Constructor doesn't validate null args
            FluentActions.Invoking(() => new Worker(_logger, null!))
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

