using MyStartUpCompany.Persistence.Extensions;
using MyStartUpCompany.Worker;
using MyStartUpCompany.Worker.Configuration;
using MyStartUpCompany.Worker.Handlers.AddCompany;
using MyStartUpCompany.Worker.Services;

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
builder.Services.AddScoped<CompanyFileProcessorService>();
builder.Services.AddScoped<CompanyMessageProcessor>();

// Configure Entity Framework Core with environment-based database selection
builder.Services.AddAppDatabase(builder.Configuration, builder.Environment);

var host = builder.Build();
host.Run();
