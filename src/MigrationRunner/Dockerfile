# Multi-stage Dockerfile for MyStartUpCompany.MigrationRunner

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution and project files
COPY ["MyStartUpCompany.sln", "./"]
COPY ["src/MigrationRunner/MigrationRunner.csproj", "src/MigrationRunner/"]
COPY ["src/MyStartUpCompany.Persistence/MyStartUpCompany.Persistence.csproj", "src/MyStartUpCompany.Persistence/"]

# Restore dependencies
RUN dotnet restore "src/MigrationRunner/MigrationRunner.csproj"

# Copy remaining source code
COPY ["src/MigrationRunner/", "src/MigrationRunner/"]
COPY ["src/MyStartUpCompany.Persistence/", "src/MyStartUpCompany.Persistence/"]

# Build the migration runner
RUN dotnet build -c Release -o /app/build "src/MigrationRunner/MigrationRunner.csproj"

# Publish
RUN dotnet publish -c Release -o /app/publish "src/MigrationRunner/MigrationRunner.csproj"

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine

WORKDIR /app

# Install additional tools if needed
RUN apk add --no-cache curl

# Copy published application from build stage
COPY --from=build /app/publish .

# Create non-root user for security
RUN addgroup -g 1001 migrationuser && \
	adduser -u 1001 -G migrationuser -D migrationuser && \
	chown -R migrationuser:migrationuser /app

USER migrationuser

# Set working directory
WORKDIR /app

# Environment variables
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the migration runner
ENTRYPOINT ["dotnet", "MyStartUpCompany.MigrationRunner.dll"]
