using Contacts.Api.Configurations.Filters;
using Contacts.Api.Infrastructure;
using Contacts.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ContactsDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("ContactsDb"));
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

builder.Services.AddScoped<IContactsRepository, ContactsRepository>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers(configure =>
{
    configure.ReturnHttpNotAcceptable = true;
    configure.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
    configure.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
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

// register Swagger generator
builder.Services.AddSwaggerGen(options =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerDoc($"ContactsAPISpecification{description.GroupName}", new()
        {
            Title = "Contacts API",
            Version = description.ApiVersion.ToString(),
            // Description of the API
            Description = "Contacts API for managing contacts",
            // Contact information for the API
            Contact = new()
            {
                Name = "John Doe",
                Email = "jdoe@getnada.com",
                Url = new("https://www.twitter.com/jdoe")
            },
            // License information for the API
            License = new()
            {
                Name = "MIT",
                Url = new("https://opensource.org/licenses/MIT")
            },
            // Terms of Service
            // TermsOfService = ...
        });
    }

    options.DocInclusionPredicate((documentName, apiDescription) =>
        {
            var actionApiVersionModel = apiDescription.ActionDescriptor.GetApiVersionModel(
                ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

            if (actionApiVersionModel is null)
            {
                return true;
            }

            if (actionApiVersionModel.DeclaredApiVersions.Any())
            {
                return actionApiVersionModel.DeclaredApiVersions.Any(v => $"ContactsAPISpecificationv{v.ToString()}" == documentName);
            }

            return actionApiVersionModel.ImplementedApiVersions.Any(v => $"ContactsAPISpecificationv{v.ToString()}" == documentName);
        }
    );

    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    // add XML comments to the Swagger doc
    options.IncludeXmlComments(xmlCommentsFullPath);

    //options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    options.OperationFilter<GetContactOperationFilter>();
    options.OperationFilter<CreateContactOperationFilter>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // to generate Swagger JSON at runtime
    app.UseSwagger();

    // to serve Swagger UI
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/ContactsAPISpecification{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
        options.RoutePrefix = ""; // serve the UI at root (https://localhost:5001)
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();
