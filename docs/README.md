# MyStartUpCompany - Documentation

Welcome to the MyStartUpCompany project documentation. This guide covers setup, development, containerization, and deployment.

## Quick Start

### Prerequisites
- .NET 10 SDK
- Docker (for containerization)
- Git
- Visual Studio or VS Code

### Get Started in 5 Minutes
1. Clone the repository
2. Follow [Local Development Guide](LOCAL_DEVELOPMENT.md) for setup
3. Run: ``dotnet build``
4. Run: ``dotnet run --project src/MyStartUpCompany.Api``

## Documentation by Topic

### 🚀 Getting Started & Local Development
- **[Local Development Guide](LOCAL_DEVELOPMENT.md)** - Setup, prerequisites, running locally, development workflow

### 🗄️ Database & Migrations
- **[Migration Strategy](MIGRATION_STRATEGY.md)** - Understanding database migrations, running MigrationRunner, migration workflows
- **[Architecture & Design](ARCHITECTURE.md)** - MigrationRunner design, why this approach, technical decisions

### 🐳 Containerization
- **[Containerization Guide](CONTAINERIZATION_GUIDE.md)** - Building Docker images, running containers, docker-compose setup
- **[Architecture & Design](ARCHITECTURE.md)** - Docker-related design decisions

### 📦 Deployment
- **[Deployment Guide](DEPLOYMENT_GUIDE.md)** - CI/CD pipeline setup, GitHub Actions, Azure DevOps, production deployment strategies

### 📚 Reference & Troubleshooting
- **[Migration Strategy](MIGRATION_STRATEGY.md)** - Troubleshooting migrations
- **[Containerization Guide](CONTAINERIZATION_GUIDE.md)** - Troubleshooting containers
- **[Deployment Guide](DEPLOYMENT_GUIDE.md)** - Troubleshooting deployments

---

## Project Structure

```
MyStartUpCompany/
├── src/
│   ├── MyStartUpCompany.Api              # REST API service
│   ├── MyStartUpCompany.Worker           # Background worker service
│   ├── MyStartUpCompany.Persistence      # Shared data layer & DbContext
│   ├── MyStartUpCompany.Notifier         # Notification service
│   └── MigrationRunner                   # Database migration runner
├── tests/                                 # Unit & integration tests
└── docs/                                  # This documentation
```

## Key Concepts

### MigrationRunner
Database migrations are handled by a **separate dedicated application** (``MigrationRunner``). This ensures:
- ✅ Migrations are explicit and traceable
- ✅ Not coupled to service startup
- ✅ DevOps friendly
- ✅ Safe for production

Learn more: [Architecture & Design](ARCHITECTURE.md)

### Services Architecture
The application follows a **service-oriented architecture**:
- **API** - REST endpoints for client requests
- **Worker** - Background job processing
- **Persistence** - Shared data layer (DbContext, entities, migrations)
- **Notifier** - Notification and alert service
- **MigrationRunner** - Explicit database migration execution

---

## Common Tasks

### Build the solution
``bash
dotnet build
``

### Run API locally
``bash
dotnet run --project src/MyStartUpCompany.Api
``

### Run background worker
``bash
dotnet run --project src/MyStartUpCompany.Worker
``

### Apply database migrations
``bash
dotnet run --project src/MigrationRunner
``

### Preview migrations (no changes)
``bash
dotnet run --project src/MigrationRunner -- --list
``

### Build Docker image
``bash
docker build -f src/MyStartUpCompany.Api/Dockerfile -t mystartupcompany:latest .
``

### Run in container
``bash
docker-compose up
``

---

## Development Workflow

1. **Create branch** for your feature
2. **Implement changes** in relevant project
3. **Write/update tests** for your changes
4. **Build solution** - ``dotnet build``
5. **Run tests** - ``dotnet test``
6. **Create migrations** if schema changes - See [Migration Strategy](MIGRATION_STRATEGY.md)
7. **Commit and push** to your branch
8. **Create pull request**

---

## Environment Configuration

### User Secrets (Local Development)
Sensitive settings are managed with User Secrets. See [Local Development Guide](LOCAL_DEVELOPMENT.md).

### Configuration Files
- ``appsettings.json`` - Base configuration
- ``appsettings.{Environment}.json`` - Environment-specific settings
- ``docker-compose.yml`` - Container orchestration

### Environment Variables
Key variables for deployment are documented in [Deployment Guide](DEPLOYMENT_GUIDE.md).

---

## Deployment Environments

### Local Development
- Run services directly with ``dotnet run``
- Use User Secrets for sensitive data
- LocalDB or local SQL Server instance

### Docker/Container
- Build images using Dockerfile
- Orchestrate with docker-compose
- Better for parity with production

### Production
- Cloud deployment (Azure App Service, AKS, etc.)
- Managed databases
- CI/CD pipeline integration
- See [Deployment Guide](DEPLOYMENT_GUIDE.md)

---

## Support & Troubleshooting

### Build Issues
See [Local Development Guide](LOCAL_DEVELOPMENT.md#troubleshooting)

### Migration Issues
See [Migration Strategy](MIGRATION_STRATEGY.md#troubleshooting)

### Docker Issues
See [Containerization Guide](CONTAINERIZATION_GUIDE.md#troubleshooting)

### Deployment Issues
See [Deployment Guide](DEPLOYMENT_GUIDE.md#troubleshooting-deployment)

---

## Best Practices

- ✅ Always run ``dotnet build`` before committing
- ✅ Always run migrations with MigrationRunner before deployment
- ✅ Keep User Secrets synchronized with deployment variables
- ✅ Use docker-compose for local container testing
- ✅ Test migrations in staging before production
- ✅ Review [Architecture & Design](ARCHITECTURE.md) for technical decisions

---

## Contributing

When contributing to this project:
1. Follow the structure in [Project Structure](#project-structure)
2. Ensure all tests pass
3. Update relevant documentation
4. Create clear commit messages
5. Reference issues in pull requests

---

## Additional Resources

- [Architecture & Design](ARCHITECTURE.md) - Technical decisions and design patterns
- [Local Development Guide](LOCAL_DEVELOPMENT.md) - Complete setup instructions
- [Migration Strategy](MIGRATION_STRATEGY.md) - Database management
- [Containerization Guide](CONTAINERIZATION_GUIDE.md) - Docker and containers
- [Deployment Guide](DEPLOYMENT_GUIDE.md) - CI/CD and production deployment

---

**Last Updated:** 2024
**Status:** Production Ready
