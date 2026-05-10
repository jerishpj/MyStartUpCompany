using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Worker;
using MyStartUpCompany.Worker.Handlers.AddCompany;
using MyStartUpCompany.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

// Register services
builder.Services.AddScoped<AddCompanyEventHandler>();
builder.Services.AddScoped<CompanyFileProcessorService>();

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

var host = builder.Build();
host.Run();
