# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/MyStartUpCompany.Api/MyStartUpCompany.Api.csproj", "src/MyStartUpCompany.Api/"]
COPY ["src/MyStartUpCompany.Persistence/MyStartUpCompany.Persistence.csproj", "src/MyStartUpCompany.Persistence/"]
COPY ["src/MyStartUpCompany.Domain/MyStartUpCompany.Domain.csproj", "src/MyStartUpCompany.Domain/"]

# Restore dependencies
RUN dotnet restore "src/MyStartUpCompany.Api/MyStartUpCompany.Api.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/MyStartUpCompany.Api"
RUN dotnet build "MyStartUpCompany.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "MyStartUpCompany.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published files
COPY --from=publish /app/publish .

# Set environment variable
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MyStartUpCompany.Api.dll"]