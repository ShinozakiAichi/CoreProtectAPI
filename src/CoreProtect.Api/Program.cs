using CoreProtect.Api.Configuration;
using CoreProtect.Api.Middleware;
using CoreProtect.Application;
using CoreProtect.Infrastructure;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<ApiSettings>()
    .Bind(builder.Configuration.GetSection(ApiSettings.SectionName))
    .ValidateDataAnnotations();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestPath | HttpLoggingFields.ResponseStatusCode;
});

builder.Services.AddCors();
builder.Services.AddHealthChecks();

var app = builder.Build();

var apiSettings = app.Services.GetRequiredService<IOptions<ApiSettings>>().Value;

if (apiSettings.Cors.Origins.Length > 0)
{
    app.UseCors(policy =>
        policy.WithOrigins(apiSettings.Cors.Origins)
            .AllowAnyHeader()
            .AllowAnyMethod());
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpLogging();
app.UseMiddleware<ApiKeyMiddleware>();

app.MapHealthChecks("/healthz");
app.MapHealthChecks("/readyz");

app.MapControllers();

app.Run();
