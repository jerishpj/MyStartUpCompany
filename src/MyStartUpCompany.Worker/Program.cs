using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Extensions;
using MyStartUpCompany.Worker;
using MyStartUpCompany.Worker.Handlers.AddCompany;
using MyStartUpCompany.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

// Register services
builder.Services.AddScoped<AddCompanyEventHandler>();
builder.Services.AddScoped<CompanyFileProcessorService>();

// Configure Entity Framework Core with environment-based database selection
builder.Services.AddAppDatabase(builder.Configuration, builder.Environment);

var host = builder.Build();
host.Run();
