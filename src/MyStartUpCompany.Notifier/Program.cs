using MyStartUpCompany.Observability;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add OpenTelemetry observability
builder.Services.AddObservability(builder.Configuration, builder.Environment);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CorrelationIdAccessor>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "My StartUp Company Notifier API Docs";
        options.Theme = ScalarTheme.Mars;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.WithDocumentDownloadType(DocumentDownloadType.Both);
    });
}

// Use trace context middleware (should be early in pipeline)
app.UseTraceContext();

// Use HTTP metrics middleware
app.UseHttpMetrics();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
