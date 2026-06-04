using Microsoft.Extensions.Logging;
using MyStartUpCompany.Observability;
using MyStartUpCompany.Persistence.Extensions;
using MyStartUpCompany.Worker;
using MyStartUpCompany.Worker.Configuration;
using MyStartUpCompany.Worker.Extensions;
using MyStartUpCompany.Worker.Handlers.AddCompany;
using MyStartUpCompany.Worker.Handlers.AddLocationHierarchy;
using MyStartUpCompany.Worker.Health;
using MyStartUpCompany.Worker.Services;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

var builder = Host.CreateApplicationBuilder(args);

// ============================================================================ 
// CONFIGURATION HIERARCHY (for local development)
// ============================================================================
// 1. appsettings.json (base - checked in, NO secrets)
// 2. appsettings.{Environment}.json (environment-specific - checked in)
// 3. User Secrets (local dev only - NOT in source control)
// 4. Environment Variables (runtime override)

// Configuration is automatically loaded from:
// - appsettings.json
// - appsettings.{EnvironmentName}.json (e.g., appsettings.Development.json)
// - User Secrets (if in Development environment)
// - Environment Variables

// Add User Secrets for Development environment only
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

// ============================================================================
// CONFIGURE SERVICES
// ============================================================================

// Configure OpenTelemetry observability for Worker service
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddOpenTelemetry(config =>
{
    // Create resource
    var resource = ResourceBuilder.CreateDefault()
        .AddService(
            serviceName: "MyStartUpCompany.Worker",
            serviceVersion: "1.0.0",
            serviceNamespace: "MyStartUpCompany",
            autoGenerateServiceInstanceId: true)
        .AddEnvironmentVariableDetector()
        .Build();

    config.IncludeScopes = true;
    config.IncludeFormattedMessage = true;

    // Add console exporter for development
    if (builder.Environment.IsDevelopment())
    {
        config.AddConsoleExporter();
    }
});

builder.Services.AddObservability(builder.Configuration, builder.Environment);

// Configure Azure Service Bus settings from configuration
var azureServiceBusSection = builder.Configuration.GetSection(AzureServiceBusSettings.SectionName);
var connectionString = azureServiceBusSection.GetValue<string>("ConnectionString");

// Register Azure Service Bus settings if connection string is available
if (!string.IsNullOrWhiteSpace(connectionString))
{
    var azureServiceBusSettings = new AzureServiceBusSettings
    {
        ConnectionString = connectionString,
        TopicName = azureServiceBusSection.GetValue<string>("TopicName") ?? "company-events",
        SubscriptionName = azureServiceBusSection.GetValue<string>("SubscriptionName") ?? "company-processor",
        MaxConcurrentCalls = azureServiceBusSection.GetValue<int>("MaxConcurrentCalls", 1),
        MaxAutoLockRenewalDuration = azureServiceBusSection.GetValue<int>("MaxAutoLockRenewalDuration", 300),
        AutoCompleteMessages = azureServiceBusSection.GetValue<bool>("AutoCompleteMessages", false)
    };

    builder.Services.AddSingleton(azureServiceBusSettings);
    builder.Services.AddHostedService<AzureServiceBusConsumerService>();
}
else
{
    // Create a default disabled settings object for dependency injection
    var disabledSettings = new AzureServiceBusSettings
    {
        ConnectionString = "",
        TopicName = "",
        SubscriptionName = ""
    };
    builder.Services.AddSingleton(disabledSettings);
}

// Register background services
builder.Services.AddHostedService<Worker>();

// Register core services
builder.Services.AddScoped<AddCompanyEventHandler>();
builder.Services.AddScoped<AddLocationHierarchyEventHandler>();
builder.Services.AddScoped<CompanyFileProcessorService>();
builder.Services.AddScoped<CompanyMessageProcessor>();
builder.Services.AddScoped<LocationHierarchyMessageProcessor>();

// Register message mapping services (Strategy + Factory patterns for source-based mapping)
builder.Services.AddMessageMappers();

// Register worker health check service
builder.Services.AddSingleton<IWorkerHealthCheck, WorkerHealthCheck>();

// Configure Entity Framework Core with environment-based database selection
builder.Services.AddAppDatabase(builder.Configuration, builder.Environment);

var host = builder.Build();
host.Run();
