using Contacts.Api.Configurations.Options;
using Contacts.Api.Infrastructure;
using Contacts.Api.Infrastructure.Authentication;
using Contacts.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using Serilog.Events;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ContactsDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("ContactsDb"));
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

builder.Services.AddScoped<IContactsRepository, ContactsRepository>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// register CORS options
// v1
//builder.Services.Configure<CorsConfiguration>(builder.Configuration.GetSection("Cors"));
// v2 - with validation
builder.Services.AddOptions<CorsConfiguration>().Bind(builder.Configuration.GetSection(CorsConfiguration.SectionName))
    .ValidateDataAnnotations()
    // check if CORS options are valid on startup
    .ValidateOnStart();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        // read origins from configuration

        // v1
        // var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>();

        // v2
        var origins = new List<string>();
        builder.Configuration.Bind("Cors:Origins", origins);

        if (origins.Any())
        {
            policyBuilder
                .WithOrigins(origins.ToArray())
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    });
});

builder.Services.AddControllers(configure =>
{
    configure.ReturnHttpNotAcceptable = true;
    configure.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
    configure.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
    configure.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
    //configure.Filters.Add(new AuthorizeFilter()); // I've added requirement to authorize all actions only to "ContactsControllerV2"

    // caching profiles
    configure.CacheProfiles.Add("NoCache",
        new CacheProfile { NoStore = true });
    configure.CacheProfiles.Add("Any-60",
        new CacheProfile
        {
            Location = ResponseCacheLocation.Any,
            Duration = 60
        });
}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
}).AddXmlDataContractSerializerFormatters();

builder.Services.Configure<MvcOptions>(options =>
{
    var jsonOutputFormatter = options.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();

    if (jsonOutputFormatter != null)
    {
        // remove text/json as it isn't the approved media type
        // for working with JSON at API level
        if (jsonOutputFormatter.SupportedMediaTypes.Contains("text/json"))
        {
            jsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
        }
    }
});

builder.Services.AddEndpointsApiExplorer();

// add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");
});

// add ApiExplorer with versioning
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VV";
    options.SubstituteApiVersionInUrl = true;
});

// retrieve ApiVersionDescriptionProvider from DI
#pragma warning disable ASP0000
var apiVersionDescriptionProvider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
#pragma warning restore ASP0000

// register OpenAPI v3 document generator with NSwag
foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
{
    // to support OpenAPI v3.0.0 use AddOpenApiDocument, for Swagger (v2.0) use AddSwaggerDocument
    builder.Services.AddOpenApiDocument(config =>
    {
        config.DocumentName = "v" + description.ApiVersion.ToString();
        config.PostProcess = document =>
        {
            document.Info.Title = "Contacts API";
            document.Info.Version = "v" + description.ApiVersion.ToString();
            document.Info.Description = "Contacts API for managing contacts";
            document.Info.Contact = new OpenApiContact
            {
                Name = "John Doe",
                Email = "jdoe@getnada.com",
                Url = "https://www.twitter.com/jdoe"
            };
            document.Info.License = new OpenApiLicense
            {
                Name = "MIT",
                Url = "https://opensource.org/licenses/MIT"
            };
            // document.Info.TermsOfService = ...
        };
        config.ApiGroupNames = new[] { description.GroupName };

        config.AddSecurity("Basic", Enumerable.Empty<string>(),
            new OpenApiSecurityScheme()
            {
                Type = OpenApiSecuritySchemeType.Basic,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Input your username and password to access this API"
            });

        config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Basic"));
    });
}

// add basic authentication
builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

// add problem details
builder.Services.AddProblemDetails();

// add response caching
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 32 * 1024 * 1024; // 32MB - specifies the maximum cacheable size for the response body in bytes (default: 64MB)
    options.SizeLimit = 256 * 1024 * 1024; // 256MB - represents the size limit for the response cache middleware in bytes (default: 100MB)
    //options.UseCaseSensitivePaths = true; // if set to true, caches the responses by using case-sensitive paths (default: false)
});

// add memory cache
builder.Services.AddMemoryCache();

// add logging support with Serilog: https://www.c-sharpcorner.com/article/structured-logging-using-serilog-in-asp-net-core-7-0/
// and by using two-stage initialization: https://github.com/serilog/serilog-aspnetcore
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    configuration.ReadFrom.Services(services);
}, preserveStaticLogger: true);

var app = builder.Build();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    // no need to add it explicitly, it's added by default
    app.UseDeveloperExceptionPage();

    // to generate OpenAPI documentation at runtime with NSwag
    // for OpenAPI v3.0.0 use UseOpenApi, for Swagger (v2.0) use UseSwagger
    app.UseOpenApi();

    // to serve OpenAPI/Swagger UI provided by NSwag
    app.UseSwaggerUi3(options =>
    {
        options.Path = "/docs"; // serve the UI at https://localhost:5001/docs which seems to be more appropriate

        // customize the UI
        options.DocExpansion = "none"; // hide the "Models" section
        options.AdditionalSettings.Add("persistAuthorization", true);
    });

    // to serve ReDoc UI provided by NSwag
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        app.UseReDoc(options =>
        {
            options.Path = $"/redoc/{description.GroupName}"; // serve the UI at https://localhost:5001/redoc/v1.0
            options.DocumentPath = $"/swagger/{description.GroupName}/swagger.json";
        });
    }
}
else
{
    // should be added at the beginning of the pipeline
    app.UseExceptionHandler();
    //// we can customize the exception handling process here
    //app.UseExceptionHandler(action =>
    //{
    //    // we can use customize the exception handling process here
    //    action.Run(async (context) =>
    //    {
    //        var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();

    //        var details = new ProblemDetails
    //        {
    //            Detail = exceptionHandler?.Error.Message,
    //            Extensions =
    //            {
    //                ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier
    //            },
    //            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
    //            Status = StatusCodes.Status500InternalServerError
    //        };

    //        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    //        logger.LogError(exceptionHandler?.Error, exceptionHandler?.Error.Message);

    //        await context.Response.WriteAsync(JsonSerializer.Serialize(details));
    //    });
    //});
    app.UseStatusCodePages();
}

// use Serilog request logging
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

// use response caching
app.UseResponseCaching();

app.MapControllers();

// recreate & migrate the database on each run, for demo purposes
using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();
dbContext.Database.EnsureDeleted();
dbContext.Database.Migrate();

app.Run();
