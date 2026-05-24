# ============================================================================
# Multi-stage Dockerfile for MyStartUpCompany.Worker
# This Worker service processes files from a local Input folder and messages
# from Azure Service Bus. The container is optimized for long-running
# background processing.
# ============================================================================

# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution and project files (restore in a separate step for better caching)
COPY ["src/MyStartUpCompany.Worker/MyStartUpCompany.Worker.csproj", "src/MyStartUpCompany.Worker/"]
COPY ["src/MyStartUpCompany.Persistence/MyStartUpCompany.Persistence.csproj", "src/MyStartUpCompany.Persistence/"]

# Restore dependencies
RUN dotnet restore "src/MyStartUpCompany.Worker/MyStartUpCompany.Worker.csproj"

# Copy all source code
COPY . .

# Build the application in Release mode
WORKDIR "/src/src/MyStartUpCompany.Worker"
RUN dotnet build "MyStartUpCompany.Worker.csproj" -c Release -o /app/build

# Stage 2: Publish stage - creates optimized runtime package
FROM build AS publish

RUN dotnet publish "MyStartUpCompany.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime stage (minimal)
FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine AS final

# Create a non-root user for enhanced security
RUN addgroup -g 1000 dotnetuser && \
	adduser -D -u 1000 -G dotnetuser dotnetuser

WORKDIR /app

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Create Input and Processed folders with proper permissions
RUN mkdir -p /app/Input /app/Processed && \
	chown -R dotnetuser:dotnetuser /app

# Switch to non-root user
USER dotnetuser

# Worker services don't expose ports; they run in background
# No EXPOSE instruction needed

# Set environment variable for .NET runtime
ENV DOTNET_ENVIRONMENT=Production

# Run the worker service
ENTRYPOINT ["dotnet", "MyStartUpCompany.Worker.dll"]
