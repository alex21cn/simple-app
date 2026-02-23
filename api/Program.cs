using api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;
using System.Reflection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));



var scopes = new Dictionary<string, string>
                {
                    { "api://0cfc38f7-9c1d-482b-a3e3-cd1685706b77/admin", "Admin scope" },
                    { "api://0cfc38f7-9c1d-482b-a3e3-cd1685706b77/read", "Read scope" },
                    { "api://0cfc38f7-9c1d-482b-a3e3-cd1685706b77/access_as_user", "Access API as signed-in user"},
                    { "api://0cfc38f7-9c1d-482b-a3e3-cd1685706b77/.default", "Default scope" }
                };
var azure = builder.Configuration.GetSection("AzureAd");
var tenantId = azure["TenantId"] ?? "common";
var clientId = azure["ClientId"] ?? string.Empty;
var audience = azure["Audience"] ?? azure["ClientId"] ?? string.Empty;
var instance = azure["Instance"]?.TrimEnd('/') ?? "https://login.microsoftonline.com";


builder.Services.AddSwaggerGen(options =>
{
    // Include XML comments (from project file GenerateDocumentationFile=true)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add OAuth2 security definition for Microsoft Identity Platform
    options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{instance}/{tenantId}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"{instance}/{tenantId}/oauth2/v2.0/token"),
                Scopes = scopes
            }
        }
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("OAuth2", doc),
            scopes.Keys.ToList()
        }
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Authentication with Microsoft Identity Platform (JWT Bearer)

builder.Services.AddAuthorization();

// EF Core DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repository
builder.Services.AddScoped<api.Services.FileReportRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

//app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.MapSwagger();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Configure SwaggerUI to use OAuth2 (Microsoft Identity Platform)
        options.RoutePrefix = "swagger";
        options.SwaggerEndpoint("v1/swagger.json", "Simple App API v1");

        options.OAuthClientId(clientId);
        options.OAuthAppName("Simple App API - Swagger");
        options.OAuthUsePkce();
        // Request the /.default scope for the API if available
        //var scope = audience.EndsWith("/.default") ? audience : (audience + "/.default");
        //c.OAuthScopeSeparator(" ");
        options.OAuthScopes(scopes.Keys.ToArray());
    });
}    

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.Title = "My API";
    options.Theme = Scalar.AspNetCore.ScalarTheme.DeepSpace;

    // Scalar UI will detect the OAuth2 flows from the OpenAPI document exposed at /openapi
    // If you need explicit UI OAuth config, check Scalar docs or the package's available APIs.

    // options.SpecUrl = "/openapi/v1.json";
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
