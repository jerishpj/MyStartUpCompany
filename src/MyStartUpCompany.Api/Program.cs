using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.CompanyDetails.Queries;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Extensions;
using Scalar.AspNetCore;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

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

        // Use exception handler middleware
        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}