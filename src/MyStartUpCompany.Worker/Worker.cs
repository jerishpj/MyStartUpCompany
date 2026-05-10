using MyStartUpCompany.Worker.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyStartUpCompany.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Company File Processor Worker started at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var fileProcessor = scope.ServiceProvider.GetRequiredService<CompanyFileProcessorService>();
                        await fileProcessor.ProcessFilesAsync(stoppingToken);
                    }

                    // Wait for 10 seconds before next check
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Worker execution");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }

            _logger.LogInformation("Company File Processor Worker stopped at: {time}", DateTimeOffset.Now);
        }
    }
}
