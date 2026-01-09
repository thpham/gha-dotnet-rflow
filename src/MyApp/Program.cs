using CoreWCF.Configuration;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MyApp.Contracts;
using MyApp.Options;
using MyApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure strongly-typed options
builder.Services.Configure<AppConfig>(
    builder.Configuration.GetSection(AppConfig.SectionName));

// Configure authentication
// Windows Authentication for internal endpoints
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // Default policy requires authenticated users
    options.FallbackPolicy = options.DefaultPolicy;

    // Policy for anonymous access (external endpoints)
    options.AddPolicy("AllowAnonymous", policy =>
        policy.RequireAssertion(_ => true));
});

// IIS Integration
builder.WebHost.UseIIS();
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AutomaticAuthentication = true;
});

// Register services
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExternalService, ExternalService>();
builder.Services.AddScoped<IScheduledTaskService, ScheduledTaskService>();

// Register enrollment services (CoreWCF/SOAP)
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IDirectoryLookupService, DirectoryLookupService>();
builder.Services.AddScoped<EnrollmentService>();

// Register CertEnroll COM interop service (Windows-only)
builder.Services.AddScoped<ICertEnrollService, CertEnrollService>();

// Add CoreWCF services
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Configure HTTP client for external REST API
builder.Services.AddHttpClient<IExternalRestApiClient, ExternalRestApiClient>(client =>
{
    var config = builder.Configuration.GetSection(AppConfig.SectionName).Get<AppConfig>();
    if (config?.ExternalApiBaseUrl is not null)
    {
        client.BaseAddress = new Uri(config.ExternalApiBaseUrl);
    }
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add controllers
builder.Services.AddControllers();

// Configure OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "MyApp API",
        Version = "v1",
        Description = "Windows .NET Application API with SOAP-style and REST endpoints"
    });
});

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApp API v1");
        c.RoutePrefix = "swagger";
    });
}

// Skip HTTPS redirection in CI environment (plain HTTP for testing)
if (!app.Environment.IsEnvironment("CI"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint (anonymous access for monitoring)
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).AllowAnonymous();

app.MapControllers();

// Configure CoreWCF SOAP endpoints
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<EnrollmentService>();
    serviceBuilder.AddServiceEndpoint<EnrollmentService, IEnrollmentService>(
      new CoreWCF.BasicHttpBinding(),
      "/soap/enrollment");
});

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
