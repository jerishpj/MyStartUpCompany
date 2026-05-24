# MyStartUpCompany.Worker

Background service for processing company data from multiple sources:
- **File-based input**: JSON files from the `Input` folder
- **Azure Service Bus**: Company events from topic messages

Data is written to the SQL Server database via Entity Framework Core.

## Quick Start (Local Development)

### Prerequisites
- .NET 10 SDK
- SQL Server (local or remote)
- Visual Studio or Visual Studio Code

### Setup

1. **Initialize User Secrets** (one-time):
   ```powershell
   cd src\MyStartUpCompany.Worker
   dotnet user-secrets init
   ```

2. **Configure Database Connection**:
   ```powershell
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\mssqllocaldb;Database=MyStartUpCompanyDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   ```

3. **Run the Worker**:
   ```powershell
   dotnet run --project src\MyStartUpCompany.Worker
   ```

For detailed setup instructions, see [User Secrets Setup Guide](docs/USER_SECRETS_SETUP.md).

## Project Structure

```
src/MyStartUpCompany.Worker/
├── Program.cs                          # Host bootstrap & DI configuration
├── Worker.cs                          # File processor background service
├── Handlers/
│   └── AddCompany/
│       ├── AddCompanyEventHandler.cs  # Database write handler
│       └── CompanyInputDto.cs         # Company input model
├── Services/
│   ├── CompanyFileProcessorService.cs # File ingestion
│   ├── CompanyMessageProcessor.cs     # Shared validation/processing
│   └── AzureServiceBusConsumerService.cs # Topic listener
├── Configuration/
│   └── AzureServiceBusSettings.cs     # Service Bus config model
├── appsettings.json                   # Base configuration (no secrets)
└── appsettings.Development.json       # Development-only settings
```

## Configuration

All configuration is managed through the standard .NET hierarchy:

1. **appsettings.json** - Base, checked into Git (no secrets)
2. **appsettings.Development.json** - Dev overrides (checked in)
3. **User Secrets** - Local sensitive values (not in Git)
4. **Environment Variables** - Runtime overrides

## Local Development

### File Processing

Place JSON files in the `Input` folder:

```json
[
  {
	"Name": "Acme Corp",
	"Address": "123 Main St",
	"City": "Springfield",
	"PostalCode": "12345",
	"Country": "USA",
	"Phone": "555-0100"
  }
]
```

The worker will:
- Move processed files to the `Processed` folder with a status suffix
- Log detailed output
- Write valid records to the database

### Azure Service Bus (Optional)

To test Service Bus integration:

1. Set the connection string in User Secrets:
   ```powershell
   dotnet user-secrets set "AzureServiceBus:ConnectionString" "Endpoint=sb://...;SharedAccessKey=..."
   ```

2. Topic/subscription defaults:
   - Topic: `companycreatedevent`
   - Subscription: `MyStartupLocalDevSubscription`

## Debugging

- Check `appsettings.Development.json` for detailed logging configuration
- Look for log output in the console or Application Insights
- User Secrets are automatically loaded in Development environment
- Use `dotnet user-secrets list` to verify secrets are set

## Future Enhancements

Docker and cloud deployment setup will be added in future phases.
