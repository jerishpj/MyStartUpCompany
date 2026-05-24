# Build stage - using .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files (restore in a separate step for better caching)
COPY ["src/MyStartUpCompany.Api/MyStartUpCompany.Api.csproj", "src/MyStartUpCompany.Api/"]
COPY ["src/MyStartUpCompany.Persistence/MyStartUpCompany.Persistence.csproj", "src/MyStartUpCompany.Persistence/"]

# Restore dependencies
RUN dotnet restore "src/MyStartUpCompany.Api/MyStartUpCompany.Api.csproj"

# Copy all source code
COPY . .

# Build the application in Release mode
WORKDIR "/src/src/MyStartUpCompany.Api"
RUN dotnet build "MyStartUpCompany.Api.csproj" -c Release -o /app/build

# Publish stage - creates optimized runtime package
FROM build AS publish
RUN dotnet publish "MyStartUpCompany.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage - using Alpine for minimal image size
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final

# Install curl for health checks
RUN apk add --no-cache curl

# Create a non-root user for enhanced security
RUN addgroup -g 1000 dotnetuser && \
    adduser -D -u 1000 -G dotnetuser dotnetuser

WORKDIR /app

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Set ownership to non-root user
RUN chown -R dotnetuser:dotnetuser /app

# Switch to non-root user
USER dotnetuser

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables for ASP.NET Core in containers
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check - configure based on your health endpoint
# Uncomment and adjust the path if you have a health check endpoint
# HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
#     CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "MyStartUpCompany.Api.dll"]