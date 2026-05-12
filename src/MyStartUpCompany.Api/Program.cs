using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.CompanyDetails.Queries;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Persistence;
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
        builder.Services.AddScoped<GetCompanyQueryHandler>();
        builder.Services.AddScoped<GetAllCompaniesQueryHandler>();

        // Configure OpenAPI
        builder.Services.AddOpenApi();

        // Configure Entity Framework Core
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
                );
            }));

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