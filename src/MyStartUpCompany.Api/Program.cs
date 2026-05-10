using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.CompanyDetails.Queries;
using MyStartUpCompany.Api.Middleware;
using MyStartUpCompany.Persistence;
using Scalar.AspNetCore;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // inject the GetCompanyQueryHandler as a transient service
        builder.Services.AddTransient<GetCompanyQueryHandler>();
        builder.Services.AddTransient<GetAllCompanyQueryHandler>();


        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}