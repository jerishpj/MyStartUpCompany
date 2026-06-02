using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.Buildings.Queries;
using MyStartUpCompany.Api.Features.CompanyDetails.Binders;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Api.Features.CompanyDetails.Queries;
using MyStartUpCompany.Api.Features.CompanyDetails.Validators;
using MyStartUpCompany.Api.Features.Locations.Queries;
using MyStartUpCompany.Api.Features.Offices.Queries;
using MyStartUpCompany.Api.Features.Projects.Queries;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Api.Shared.Filters;
using MyStartUpCompany.Observability;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Extensions;
using Scalar.AspNetCore;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<FluentValidationFilter>();
            // Register custom model binder for silent defaulting of pagination parameters
            options.ModelBinderProviders.Insert(0, new SearchCompanyRequestModelBinderProvider());
        });

        // Add OpenTelemetry observability
        builder.Services.AddObservability(builder.Configuration, builder.Environment);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<CorrelationIdAccessor>();

        // Register FluentValidation validators
        builder.Services.AddValidatorsFromAssemblyContaining<SearchCompanyRequestValidator>();
        builder.Services.AddScoped<FluentValidationFilter>();

        // Configure ProblemDetails with custom factory
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
            };
        });

        // Register query handlers
        builder.Services.AddScoped<IGetCompanyQueryHandler, GetCompanyQueryHandler>();
        builder.Services.AddScoped<IGetAllCompaniesQueryHandler, GetAllCompaniesQueryHandler>();
        builder.Services.AddScoped<IGetFilteredCompaniesQueryHandler, GetFilteredCompaniesQueryHandler>();

        // Register location query handlers
        builder.Services.AddScoped<IGetLocationQueryHandler, GetLocationQueryHandler>();
        builder.Services.AddScoped<IGetAllLocationsQueryHandler, GetAllLocationsQueryHandler>();
        builder.Services.AddScoped<IGetFilteredLocationsQueryHandler, GetFilteredLocationsQueryHandler>();

        // Register building query handlers
        builder.Services.AddScoped<IGetBuildingQueryHandler, GetBuildingQueryHandler>();
        builder.Services.AddScoped<IGetAllBuildingsQueryHandler, GetAllBuildingsQueryHandler>();
        builder.Services.AddScoped<IGetFilteredBuildingsQueryHandler, GetFilteredBuildingsQueryHandler>();

        // Register office query handlers
        builder.Services.AddScoped<IGetOfficeQueryHandler, GetOfficeQueryHandler>();
        builder.Services.AddScoped<IGetAllOfficesQueryHandler, GetAllOfficesQueryHandler>();
        builder.Services.AddScoped<IGetFilteredOfficesQueryHandler, GetFilteredOfficesQueryHandler>();

        // Register project query handlers
        builder.Services.AddScoped<IGetProjectQueryHandler, GetProjectQueryHandler>();
        builder.Services.AddScoped<IGetAllProjectsQueryHandler, GetAllProjectsQueryHandler>();
        builder.Services.AddScoped<IGetFilteredProjectsQueryHandler, GetFilteredProjectsQueryHandler>();

        // Configure OpenAPI
        builder.Services.AddOpenApi();

        // Configure Entity Framework Core with environment-based database selection
        builder.Services.AddAppDatabase(builder.Configuration, builder.Environment);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.Title = "My StartUp Company API Docs";
                options.Theme = ScalarTheme.Mars;
                options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
                options.WithDocumentDownloadType(DocumentDownloadType.Both);
            });
        }

        // Use trace context middleware (should be early in pipeline)
        app.UseTraceContext();

        // Use HTTP metrics middleware
        app.UseHttpMetrics();

        // Use exception handler middleware
        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}