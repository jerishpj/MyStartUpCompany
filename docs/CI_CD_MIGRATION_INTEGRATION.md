# CI/CD Migration Integration Guide

This guide shows how to integrate the separated migration strategy into your CI/CD pipelines and deployment processes.

## GitHub Actions Example

### Basic Pipeline: Build → Migrate → Deploy

```yaml
name: Build, Test, Migrate & Deploy

on:
  push:
	branches: [main]
  pull_request:
	branches: [main]

env:
  REGISTRY: myregistry.azurecr.io
  IMAGE_NAME: mystartupcompany

jobs:
  build-and-test:
	runs-on: ubuntu-latest
	steps:
	  - uses: actions/checkout@v3

	  - name: Setup .NET
		uses: actions/setup-dotnet@v3
		with:
		  dotnet-version: '10.0.x'

	  - name: Restore dependencies
		run: dotnet restore

	  - name: Build
		run: dotnet build --no-restore --configuration Release

	  - name: Run tests
		run: dotnet test --no-build --configuration Release --verbosity normal

  migrations-dev:
	needs: build-and-test
	if: github.event_name == 'push' && github.ref == 'refs/heads/main'
	runs-on: ubuntu-latest
	environment: development
	steps:
	  - uses: actions/checkout@v3

	  - name: Setup .NET
		uses: actions/setup-dotnet@v3
		with:
		  dotnet-version: '10.0.x'

	  - name: Run Migrations (Development)
		run: |
		  dotnet run --project src/MigrationRunner -- \
			--environment Development \
			--connection-string "${{ secrets.DB_CONNECTION_STRING_DEV }}"

  deploy-dev:
	needs: migrations-dev
	runs-on: ubuntu-latest
	environment: development
	steps:
	  - uses: actions/checkout@v3

	  - name: Deploy API to App Service (Development)
		uses: azure/webapps-deploy@v2
		with:
		  app-name: 'mystartupcompany-api-dev'
		  publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE_API_DEV }}

	  - name: Deploy Worker to App Service (Development)
		uses: azure/webapps-deploy@v2
		with:
		  app-name: 'mystartupcompany-worker-dev'
		  publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE_WORKER_DEV }}

  migrations-prod:
	needs: build-and-test
	if: github.event_name == 'push' && github.ref == 'refs/heads/main'
	runs-on: ubuntu-latest
	environment: production
	steps:
	  - uses: actions/checkout@v3

	  - name: Setup .NET
		uses: actions/setup-dotnet@v3
		with:
		  dotnet-version: '10.0.x'

	  - name: Run Migrations (Production)
		run: |
		  dotnet run --project src/MigrationRunner -- \
			--environment Production \
			--connection-string "${{ secrets.DB_CONNECTION_STRING_PROD }}"

  deploy-prod:
	needs: migrations-prod
	runs-on: ubuntu-latest
	environment: production
	steps:
	  - uses: actions/checkout@v3

	  - name: Deploy API to App Service (Production)
		uses: azure/webapps-deploy@v2
		with:
		  app-name: 'mystartupcompany-api-prod'
		  publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE_API_PROD }}

	  - name: Deploy Worker to App Service (Production)
		uses: azure/webapps-deploy@v2
		with:
		  app-name: 'mystartupcompany-worker-prod'
		  publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE_WORKER_PROD }}
```

### Container-Based Pipeline

```yaml
name: Build, Test, Migrate & Deploy (Containers)

on:
  push:
	branches: [main]

env:
  REGISTRY: myregistry.azurecr.io

jobs:
  build-containers:
	runs-on: ubuntu-latest
	steps:
	  - uses: actions/checkout@v3

	  - name: Set up Docker Buildx
		uses: docker/setup-buildx-action@v2

	  - name: Log in to Azure Registry
		uses: docker/login-action@v2
		with:
		  registry: ${{ env.REGISTRY }}
		  username: ${{ secrets.AZURE_REGISTRY_USERNAME }}
		  password: ${{ secrets.AZURE_REGISTRY_PASSWORD }}

	  - name: Build and push Migration Runner image
		uses: docker/build-push-action@v4
		with:
		  context: .
		  file: ./src/MigrationRunner/Dockerfile
		  push: true
		  tags: ${{ env.REGISTRY }}/mystartupcompany-migrations:${{ github.sha }}
		  cache-from: type=registry,ref=${{ env.REGISTRY }}/mystartupcompany-migrations:latest
		  cache-to: type=inline

	  - name: Build and push API image
		uses: docker/build-push-action@v4
		with:
		  context: .
		  file: ./src/MyStartUpCompany.Api/Dockerfile
		  push: true
		  tags: ${{ env.REGISTRY }}/mystartupcompany-api:${{ github.sha }}
		  cache-from: type=registry,ref=${{ env.REGISTRY }}/mystartupcompany-api:latest

	  - name: Build and push Worker image
		uses: docker/build-push-action@v4
		with:
		  context: .
		  file: ./src/MyStartUpCompany.Worker/Dockerfile
		  push: true
		  tags: ${{ env.REGISTRY }}/mystartupcompany-worker:${{ github.sha }}
		  cache-from: type=registry,ref=${{ env.REGISTRY }}/mystartupcompany-worker:latest

  deploy-dev:
	needs: build-containers
	runs-on: ubuntu-latest
	environment: development
	steps:
	  - uses: actions/checkout@v3

	  - name: Azure Login
		uses: azure/login@v1
		with:
		  creds: ${{ secrets.AZURE_CREDENTIALS }}

	  - name: Run Migrations on ACI (Development)
		run: |
		  az container create \
			--resource-group rg-dev \
			--name migrations-runner-dev \
			--image ${{ env.REGISTRY }}/mystartupcompany-migrations:${{ github.sha }} \
			--cpu 1 --memory 1 \
			--environment-variables \
			  ASPNETCORE_ENVIRONMENT=Development \
			  DEBUG_MIGRATIONS=false \
			--secure-environment-variables \
			  ConnectionStrings__DefaultConnection="${{ secrets.DB_CONNECTION_STRING_DEV }}" \
			--restart-policy OnFailure

		  # Wait for completion
		  az container wait \
			--resource-group rg-dev \
			--name migrations-runner-dev \
			--updated

	  - name: Deploy API Container to ACI (Development)
		run: |
		  az container create \
			--resource-group rg-dev \
			--name mystartupcompany-api-dev \
			--image ${{ env.REGISTRY }}/mystartupcompany-api:${{ github.sha }} \
			--cpu 1 --memory 1 \
			--ports 80 443 \
			--environment-variables \
			  ASPNETCORE_ENVIRONMENT=Development \
			--secure-environment-variables \
			  ConnectionStrings__DefaultConnection="${{ secrets.DB_CONNECTION_STRING_DEV }}" \
			--restart-policy Always

	  - name: Deploy Worker Container to ACI (Development)
		run: |
		  az container create \
			--resource-group rg-dev \
			--name mystartupcompany-worker-dev \
			--image ${{ env.REGISTRY }}/mystartupcompany-worker:${{ github.sha }} \
			--cpu 1 --memory 1 \
			--environment-variables \
			  ASPNETCORE_ENVIRONMENT=Development \
			--secure-environment-variables \
			  ConnectionStrings__DefaultConnection="${{ secrets.DB_CONNECTION_STRING_DEV }}" \
			  AzureServiceBus__ConnectionString="${{ secrets.SERVICE_BUS_CONNECTION_STRING_DEV }}" \
			--restart-policy Always

  deploy-prod:
	needs: build-containers
	runs-on: ubuntu-latest
	environment: production
	steps:
	  - uses: actions/checkout@v3

	  - name: Azure Login
		uses: azure/login@v1
		with:
		  creds: ${{ secrets.AZURE_CREDENTIALS }}

	  - name: Backup Database (Production)
		run: |
		  az sql db backup create \
			--resource-group rg-prod \
			--server sqlserver-prod \
			--name MyStartUpCompanyDb \
			--backup-name "pre-migration-$(date +%s)"

	  - name: Run Migrations on ACI (Production)
		run: |
		  az container create \
			--resource-group rg-prod \
			--name migrations-runner-prod \
			--image ${{ env.REGISTRY }}/mystartupcompany-migrations:${{ github.sha }} \
			--cpu 2 --memory 2 \
			--environment-variables \
			  ASPNETCORE_ENVIRONMENT=Production \
			  DEBUG_MIGRATIONS=false \
			--secure-environment-variables \
			  ConnectionStrings__DefaultConnection="${{ secrets.DB_CONNECTION_STRING_PROD }}" \
			--restart-policy OnFailure

		  # Wait for completion with timeout
		  timeout 600 az container wait \
			--resource-group rg-prod \
			--name migrations-runner-prod \
			--updated || exit 1

	  - name: Deploy API Container (Production)
		run: |
		  # Replace image in existing container or create new
		  az container create \
			--resource-group rg-prod \
			--name mystartupcompany-api-prod \
			--image ${{ env.REGISTRY }}/mystartupcompany-api:${{ github.sha }} \
			--cpu 2 --memory 2 \
			--ports 80 443 \
			--environment-variables \
			  ASPNETCORE_ENVIRONMENT=Production \
			--secure-environment-variables \
			  ConnectionStrings__DefaultConnection="${{ secrets.DB_CONNECTION_STRING_PROD }}" \
			--restart-policy Always

	  - name: Notify on Failure
		if: failure()
		run: |
		  echo "Migration or deployment failed. Review logs before manual intervention."
		  # Send notification (email, Slack, etc.)
```

## Azure DevOps Pipeline Example

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '10.0.x'

stages:
  - stage: Build
	displayName: 'Build and Test'
	jobs:
	  - job: BuildAndTest
		displayName: 'Build & Test'
		steps:
		  - task: UseDotNet@2
			inputs:
			  version: $(dotnetVersion)

		  - task: DotNetCoreCLI@2
			displayName: 'Restore'
			inputs:
			  command: 'restore'

		  - task: DotNetCoreCLI@2
			displayName: 'Build'
			inputs:
			  command: 'build'
			  arguments: '--configuration $(buildConfiguration)'

		  - task: DotNetCoreCLI@2
			displayName: 'Test'
			inputs:
			  command: 'test'
			  arguments: '--configuration $(buildConfiguration)'

  - stage: DeployDev
	displayName: 'Deploy to Development'
	dependsOn: Build
	condition: succeeded()
	jobs:
	  - deployment: MigrateDev
		environment: 'Development'
		strategy:
		  runOnce:
			deploy:
			  steps:
				- task: UseDotNet@2
				  inputs:
					version: $(dotnetVersion)

				- task: DotNetCoreCLI@2
				  displayName: 'Run Migrations (Dev)'
				  inputs:
					command: 'run'
					projects: 'src/MigrationRunner/MigrationRunner.csproj'
					arguments: '-- --environment Development --connection-string "$(DatabaseConnectionStringDev)"'

	  - deployment: DeployApiDev
		environment: 'Development'
		dependsOn: MigrateDev
		strategy:
		  runOnce:
			deploy:
			  steps:
				- task: AzureAppServiceDeploy@0
				  inputs:
					appType: 'webAppLinux'
					azureSubscription: '$(AzureSubscriptionDev)'
					appName: '$(ApiAppNameDev)'
					package: '$(Pipeline.Workspace)/drop'

  - stage: DeployProd
	displayName: 'Deploy to Production'
	dependsOn: Build
	condition: succeeded()
	jobs:
	  - deployment: BackupProd
		environment: 'Production'
		strategy:
		  runOnce:
			deploy:
			  steps:
				- task: AzureCLI@2
				  displayName: 'Backup Database'
				  inputs:
					azureSubscription: '$(AzureSubscriptionProd)'
					scriptType: 'bash'
					scriptLocation: 'inlineScript'
					inlineScript: |
					  az sql db backup create \
						--resource-group $(ResourceGroupProd) \
						--server $(SqlServerProd) \
						--name MyStartUpCompanyDb \
						--backup-name "pre-migration-$(date +%s)"

	  - deployment: MigrateProd
		environment: 'Production'
		dependsOn: BackupProd
		strategy:
		  runOnce:
			deploy:
			  steps:
				- task: UseDotNet@2
				  inputs:
					version: $(dotnetVersion)

				- task: DotNetCoreCLI@2
				  displayName: 'Run Migrations (Prod)'
				  inputs:
					command: 'run'
					projects: 'src/MigrationRunner/MigrationRunner.csproj'
					arguments: '-- --environment Production --connection-string "$(DatabaseConnectionStringProd)"'
```

## Docker Compose Deployment

```yaml
# For local development / testing
version: '3.8'

services:
  # Database
  mssql:
	image: mcr.microsoft.com/mssql/server:2022-latest
	environment:
	  ACCEPT_EULA: Y
	  SA_PASSWORD: YourStrong@Password123
	ports:
	  - "1433:1433"
	volumes:
	  - sqlserver_data:/var/opt/mssql
	healthcheck:
	  test: /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -Q "SELECT 1" || exit 1

  # Migrations
  migrations:
	build:
	  context: .
	  dockerfile: src/MigrationRunner/Dockerfile
	depends_on:
	  mssql:
		condition: service_healthy
	environment:
	  ASPNETCORE_ENVIRONMENT: Development
	  ConnectionStrings__DefaultConnection: "Server=mssql;Database=MyStartUpCompanyDb;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true"
	restart: no

  # API Service
  api:
	build:
	  context: .
	  dockerfile: src/MyStartUpCompany.Api/Dockerfile
	depends_on:
	  - migrations
	environment:
	  ASPNETCORE_ENVIRONMENT: Development
	  ConnectionStrings__DefaultConnection: "Server=mssql;Database=MyStartUpCompanyDb;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true"
	ports:
	  - "8080:8080"

  # Worker Service
  worker:
	build:
	  context: .
	  dockerfile: src/MyStartUpCompany.Worker/Dockerfile
	depends_on:
	  - migrations
	environment:
	  ASPNETCORE_ENVIRONMENT: Development
	  ConnectionStrings__DefaultConnection: "Server=mssql;Database=MyStartUpCompanyDb;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true"

volumes:
  sqlserver_data:
```

## Kubernetes Deployment Example

```yaml
# 1. Database secret
apiVersion: v1
kind: Secret
metadata:
  name: db-connection-secret
type: Opaque
stringData:
  connection-string: "Server=sql-server.azure.com;Database=MyStartUpCompanyDb;User Id=admin;Password=...;TrustServerCertificate=true"

---
# 2. Migration Job (runs before services)
apiVersion: batch/v1
kind: Job
metadata:
  name: mystartupcompany-migrations
spec:
  template:
	spec:
	  containers:
	  - name: migrations
		image: myregistry.azurecr.io/mystartupcompany-migrations:latest
		env:
		- name: ASPNETCORE_ENVIRONMENT
		  value: Production
		- name: ConnectionStrings__DefaultConnection
		  valueFrom:
			secretKeyRef:
			  name: db-connection-secret
			  key: connection-string
		resources:
		  requests:
			memory: "256Mi"
			cpu: "250m"
		  limits:
			memory: "512Mi"
			cpu: "500m"
	  restartPolicy: Never
  backoffLimit: 3

---
# 3. API Deployment (depends on migration job)
apiVersion: apps/v1
kind: Deployment
metadata:
  name: mystartupcompany-api
spec:
  replicas: 2
  selector:
	matchLabels:
	  app: mystartupcompany-api
  template:
	metadata:
	  labels:
		app: mystartupcompany-api
	spec:
	  containers:
	  - name: api
		image: myregistry.azurecr.io/mystartupcompany-api:latest
		ports:
		- containerPort: 8080
		env:
		- name: ASPNETCORE_ENVIRONMENT
		  value: Production
		- name: ConnectionStrings__DefaultConnection
		  valueFrom:
			secretKeyRef:
			  name: db-connection-secret
			  key: connection-string
		livenessProbe:
		  httpGet:
			path: /health
			port: 8080
		  initialDelaySeconds: 30
		  periodSeconds: 10
		readinessProbe:
		  httpGet:
			path: /ready
			port: 8080
		  initialDelaySeconds: 10
		  periodSeconds: 5

---
# 4. Worker Deployment (depends on migration job)
apiVersion: apps/v1
kind: Deployment
metadata:
  name: mystartupcompany-worker
spec:
  replicas: 1
  selector:
	matchLabels:
	  app: mystartupcompany-worker
  template:
	metadata:
	  labels:
		app: mystartupcompany-worker
	spec:
	  containers:
	  - name: worker
		image: myregistry.azurecr.io/mystartupcompany-worker:latest
		env:
		- name: ASPNETCORE_ENVIRONMENT
		  value: Production
		- name: ConnectionStrings__DefaultConnection
		  valueFrom:
			secretKeyRef:
			  name: db-connection-secret
			  key: connection-string
		- name: AzureServiceBus__ConnectionString
		  valueFrom:
			secretKeyRef:
			  name: servicebus-secret
			  key: connection-string
		resources:
		  requests:
			memory: "512Mi"
			cpu: "250m"
		  limits:
			memory: "1Gi"
			cpu: "500m"
```

## Pre-Deployment Checklist

- [ ] All migrations committed to git
- [ ] Migrations tested locally
- [ ] Database backup created (production)
- [ ] Connection string secrets stored securely
- [ ] Rollback plan defined
- [ ] Health checks configured for API/Worker
- [ ] Monitoring/alerting enabled
- [ ] Database user has required permissions
- [ ] Migration timeout set appropriately
- [ ] Post-deployment validation plan ready

## Rollback Strategy

```bash
# If migration fails:
# 1. Stop API/Worker services
# 2. Restore database from backup
# 3. Investigate migration code
# 4. Create down migration if needed
# 5. Retry

# Example: Revert to previous migration
dotnet ef migrations remove --project src/MyStartUpCompany.Persistence
```

## Monitoring Migration Execution

```bash
# Capture migration logs
dotnet run --project src/MigrationRunner > migration.log 2>&1

# Monitor in Azure
az container logs --resource-group rg --name migrations-runner

# Monitor in Kubernetes
kubectl logs -f job/mystartupcompany-migrations

# Monitor in GitHub Actions
# View in workflow run details
```

## Summary

This approach ensures:
- Migrations run as an explicit pipeline step
- No automatic migration on service startup
- Safe, auditable schema changes
- Easy integration with CI/CD
- Production-ready deployment workflow
