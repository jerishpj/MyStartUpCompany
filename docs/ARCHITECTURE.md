# Architecture & Design

This document explains the architectural decisions, design patterns, and key components of MyStartUpCompany.

## Solution Architecture

```
MyStartUpCompany Solution
│
├── API Service (REST endpoints)
│   └── Depends on → Persistence
│
├── Worker Service (Background jobs)
│   └── Depends on → Persistence
│
├── Notifier Service (Notifications)
│   └── Depends on → Persistence
│
├── MigrationRunner (Explicit migrations)
│   └── Depends on → Persistence
│
└── Persistence (Shared data layer)
	├── DbContext
	├── Entities
	├── Migrations
	└── Business logic
```

### Key Principle: Separation of Concerns

Each service has a single, well-defined responsibility:
- **API**: Handle HTTP requests and serve API consumers
- **Worker**: Process background jobs and events
- **Notifier**: Send notifications and alerts
- **MigrationRunner**: Manage database schema changes explicitly
- **Persistence**: Encapsulate all data access logic

## MigrationRunner Design

### Why a Separate Migration Tool?

MigrationRunner is a **dedicated console application** separate from web services. This design provides:

✅ **Safety** - Schema changes are explicit and controlled, not automatic on service startup  
✅ **Auditability** - Every migration run is logged and can be audited  
✅ **Flexibility** - Migrations can run as separate deployment step or on-demand  
✅ **Reliability** - Services start instantly without schema overhead  
✅ **DevOps Friendly** - Can run in CI/CD pipelines before service deployment  

### Industry Standard

This approach is **standard in enterprise .NET development**:

| Company | Approach |
|---------|----------|
| Microsoft | Explicit migration tools recommended |
| Stripe | Separate migration runners |
| GitHub | Explicit schema management |
| Okta | Dedicated migration infrastructure |
| Capital One | Migration tools separate from services |

### Project Structure

```
src/MigrationRunner/
├── Program.cs                 Entry point, configuration
├── MigrationRunner.csproj     Project file with packages
├── Dockerfile                 Container image
└── appsettings.json          Configuration template
```

### Key Features

- **Command-line arguments** - List, environment override, connection string
- **Configuration hierarchy** - appsettings, environment variables, CLI args
- **Logging** - Comprehensive logging with color output
- **Error handling** - Graceful failure with proper exit codes
- **Containerization** - Docker support for CI/CD pipelines

## Service Architecture

### API Service

```
MyStartUpCompany.Api
├── Controllers/               HTTP endpoints
├── Services/                  Business logic
├── Program.cs                 Configuration & startup
└── appsettings.json          Configuration
```

**Responsibilities:**
- Accept HTTP requests
- Validate input
- Call business services
- Return HTTP responses
- Handle authentication/authorization

### Worker Service (BackgroundService)

```
MyStartUpCompany.Worker
├── Services/                  Business logic
├── BackgroundTasks/           Hosted services
├── Program.cs                 Configuration & startup
└── appsettings.json          Configuration
```

**Responsibilities:**
- Process background jobs
- Listen to message queues
- Perform async operations
- Handle retries and failures
- Monitor and log progress

Uses `BackgroundService` from Microsoft.Extensions.Hosting for reliable background task execution.

### Persistence Layer

```
MyStartUpCompany.Persistence
├── DbContext/
│   └── AppDbContext.cs       Main database context
├── Entities/                  Domain models
├── Configurations/            EF Core configurations
├── Migrations/                Database schema versions
└── Extensions/                Helper methods
```

**Responsibilities:**
- Define DbContext
- Define entities and relationships
- Configure EF Core mappings
- Provide data access methods
- Manage migrations

### Shared Persistence Approach

All services use the **same DbContext** from Persistence project:

```csharp
// In API
var context = serviceProvider.GetRequiredService<AppDbContext>();

// In Worker
var context = serviceProvider.GetRequiredService<AppDbContext>();

// In MigrationRunner
var context = serviceProvider.GetRequiredService<AppDbContext>();
```

**Benefits:**
- ✅ Single source of truth for schema
- ✅ Consistent data access
- ✅ Easier migrations
- ✅ No duplicate entity definitions

## Dependency Management

### Package Versions

All projects use **aligned package versions**:

```
All projects → Microsoft.Extensions.* @ 10.0.7
All projects → Entity Framework Core @ 10.0.0
All projects → .NET Target Framework @ net10.0
```

**Why alignment matters:**
- Prevents NuGet version conflicts
- Ensures compatible APIs
- Simplifies troubleshooting

### Transitive Dependencies

The Persistence project defines core dependencies:

```
MyStartUpCompany.Persistence
└── Depends on:
	├── Entity Framework Core 10.0.0
	├── Microsoft.Extensions.Configuration 10.0.7
	└── Microsoft.Extensions.DependencyInjection 10.0.7

API, Worker, Notifier projects
└── Depend on:
	└── MyStartUpCompany.Persistence (pulls in transitive deps)

MigrationRunner
└── Directly references all dependencies needed
	(Must match what Persistence needs)
```

## Database Design

### Entities

Core domain entities are defined in `MyStartUpCompany.Persistence/Entities/`:

```csharp
public class Company
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
```

### DbContext Configuration

Centralized in `AppDbContext`:

```csharp
public class AppDbContext : DbContext
{
	public DbSet<Company> Companies { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Entity configurations
		modelBuilder.Entity<Company>()
			.HasKey(c => c.Id);
	}
}
```

### Migrations

Managed through MigrationRunner:

```
Migrations/
├── 20240101000000_InitialCreate.cs
├── 20240115000000_AddEmailColumn.cs
└── 20240201000000_AddTimestamps.cs
```

Each migration:
- Represents a schema version
- Can be applied or rolled back
- Tracks applied migrations in `__EFMigrationsHistory` table

## Configuration Management

### Configuration Sources (Priority Order)

Higher priority sources override lower priority:

```
1. Command-line arguments        ← Highest priority
2. Environment variables
3. User Secrets (dev only)
4. appsettings.{Environment}.json
5. appsettings.json             ← Lowest priority (defaults)
```

### Example Configuration Chain

```powershell
# 1. Start with defaults (appsettings.json)
{
  "ConnectionStrings": {
	"DefaultConnection": "(localdb)\\mssqllocaldb"
  }
}

# 2. Override with environment
$env:ASPNETCORE_ENVIRONMENT = "Production"
# Loads appsettings.Production.json

# 3. Override with env var
$env:ConnectionStrings__DefaultConnection = "Server=prod-server;..."

# 4. Override with CLI arg
dotnet run -- --connection-string "Server=another-server;..."
```

## Security Architecture

### Secrets Management

- ✅ **User Secrets**: Local development (not committed)
- ✅ **Environment Variables**: Runtime configuration
- ✅ **Key Vault**: Production secrets (Azure)
- ✅ **Docker Secrets**: Container secrets (production)

### What Gets Committed

```
Committed to Git:
✓ appsettings.json (defaults, no secrets)
✓ appsettings.Development.json (structure only)
✓ appsettings.Docker.json (placeholders)
✓ .env.example (template)
✓ Dockerfile (image config)

NOT Committed:
✗ User secrets
✗ appsettings.Production.json (create at deployment)
✗ .env file (actual values)
✗ Connection strings (except placeholders)
✗ API keys
```

## Deployment Architecture

### Local Development

```
Developer Machine
├── .NET 10 SDK
├── SQL Server LocalDB
├── User Secrets (local only)
└── Services running directly
```

### Docker/Container

```
Docker Host
├── mssql container (database)
├── migrations container (schema setup)
├── api container (REST service)
├── worker container (background jobs)
└── Shared network
```

### Production (Cloud)

```
Azure / AWS / GCP
├── Managed SQL Database
├── Container Registry
├── Container Instances or AKS
├── Load Balancer
├── Key Vault
└── Monitoring & Logging
```

## Design Patterns Used

### Dependency Injection (DI)

All services use DI for loose coupling:

```csharp
var services = new ServiceCollection();
services.AddDbContext<AppDbContext>();
services.AddScoped<ICompanyService, CompanyService>();
var serviceProvider = services.BuildServiceProvider();
```

### Repository Pattern (via EF Core)

DbContext acts as Unit of Work:

```csharp
public class CompanyService
{
	private readonly AppDbContext _context;

	public async Task<Company> GetCompanyAsync(int id)
	{
		return await _context.Companies.FindAsync(id);
	}
}
```

### Configuration Pattern

Central configuration for all services:

```csharp
var config = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json")
	.AddEnvironmentVariables()
	.Build();
```

### Factory Pattern

Creating services with configuration:

```csharp
public static IServiceProvider BuildServiceProvider(IConfiguration config)
{
	var services = new ServiceCollection();
	// Register all services
	return services.BuildServiceProvider();
}
```

## Testing Architecture

### Unit Tests

Test individual services and components:

```
MyStartUpCompany.Api.Tests/
├── Services/
├── Controllers/
└── Fixtures/
```

### Integration Tests

Test with real database:

```
MyStartUpCompany.Worker.Tests/
├── Integration/
└── Fixtures/
```

## Technology Stack

### Core Framework
- **.NET 10** - Latest .NET runtime
- **Entity Framework Core 10** - ORM for data access

### Infrastructure
- **SQL Server** - Relational database
- **Azure Service Bus** - Message queue (if used)
- **Docker** - Containerization
- **GitHub Actions / Azure DevOps** - CI/CD

### Libraries
- **Microsoft.Extensions.*** - Configuration, DI, logging
- **Serilog** - Structured logging (optional)
- **xUnit** - Testing framework
- **Moq** - Mocking library

## Code Organization Best Practices

### Project Naming
- `MyStartUpCompany.Api` - Public API
- `MyStartUpCompany.Worker` - Background services
- `MyStartUpCompany.Persistence` - Data layer
- `MyStartUpCompany.Notifier` - Notification service
- `MigrationRunner` - Migration tool

### File Organization
- Group related functionality in folders
- One entity per file in Entities/
- One service per file in Services/
- Clear naming conventions

### Dependency Direction
```
API/Worker/Notifier → Persistence
				   ↓
			  Entities/DbContext
```

## Performance Considerations

### Database Access
- Use async methods for I/O operations
- Minimize round trips to database
- Use Include() for related data
- Apply pagination for large result sets

### Caching
- Cache frequently accessed data
- Use distributed cache for multi-instance deployments
- Set appropriate TTLs

### Monitoring
- Log important operations
- Track error rates
- Monitor response times
- Alert on failures

## Future Enhancements

Possible improvements to consider:

- Caching layer (Redis)
- Message queue optimization
- Event sourcing for audit trail
- CQRS pattern for read-heavy workloads
- API versioning strategy
- Rate limiting and throttling

## Summary

MyStartUpCompany uses a **service-oriented architecture** with:

✅ Clear separation of concerns  
✅ Shared persistence layer  
✅ Explicit database migration management  
✅ Dependency injection throughout  
✅ Centralized configuration  
✅ Docker-ready containerization  
✅ CI/CD pipeline compatible  

This design is **scalable, maintainable, and follows industry best practices**.

## Related Documentation

- [Local Development Guide](LOCAL_DEVELOPMENT.md) - Setup and development
- [Migration Strategy](MIGRATION_STRATEGY.md) - Database management
- [Containerization Guide](CONTAINERIZATION_GUIDE.md) - Docker deployment
- [Deployment Guide](DEPLOYMENT_GUIDE.md) - Production deployment
