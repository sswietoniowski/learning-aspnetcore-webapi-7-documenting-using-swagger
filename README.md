# Learning ASP.NET Core - WebAPI (.NET 7) Documenting Using Swagger

This repository contains examples showing how to documenting an API (WebAPI 7) using Swagger.

Based on this course [Documenting an ASP.NET Core 6 Web API Using Swagger](https://app.pluralsight.com/library/courses/asp-dot-net-core-6-web-api-documenting-swagger/table-of-contents).

Original course materials can be found [here](https://app.pluralsight.com/library/courses/asp-dot-net-core-6-web-api-documenting-swagger/exercise-files) and [here](https://github.com/KevinDockx/DocumentingAspNetCore6API).

## Setup

To run API:

```cmd
cd .\contacts\backend\api
dotnet restore
dotnet ef database update
dotnet build
dotnet watch run
cd ..
```

To update NuGet packages consider using [Paket](https://fsprojects.github.io/Paket/) [:file_folder:](https://github.com/fsprojects/Paket).

```cmd
dotnet new tool-manifest
dotnet tool install paket
dotnet tool restore
dotnet paket init
dotnet tool restore
dotnet paket restore
dotnet paket install
dotnet paket outdated
dotnet paket update
```

You might use Visual Studio or JetBrains Rider to update NuGet packages (it will be a lot simpler :-)).

## Getting Started with OpenAPI (Swagger)

Gentle introduction to OpenAPI (Swagger).

### Why Use Swagger / OpenAPI to Document Your API?

Public APIs need documentation, but so do in-company APIs.

Documentation leads to knowledge leads to adoption.

Clear documentation saves time and money.

### Clearing up the Terminology Confusion

[`OpenAPI Specification`](https://github.com/OAI/OpenAPI-Specification) describes the capabilities of your API, and how to interact with it. It's standarized, an in `JSON` or `YAML` format.

OpenAPI 3 is the current version.

"Swagger" can be used, but "OpenAPI" is the preferred term.

OpenAPI specification and Swagger specification are the same thing.

`Swagger` is a set of open-source built around that OpenAPI specification.

[`Swagger UI`](https://swagger.io/tools/swagger-ui/) [:file_folder:](https://github.com/swagger-api/swagger-ui) renders a documentation UI from the specification.

[`Swagger Editor`](https://swagger.io/tools/swagger-editor/) [:file_folder:](https://github.com/swagger-api/swagger-editor) helps with creating the specification.

[`Swagger Codegen`](https://swagger.io/tools/swagger-codegen/) [:file_folder:](https://github.com/swagger-api/swagger-codegen) consists of a set of tools that help with generating client classes, tests, ... from the specification.

[`Redoc`](https://redocly.github.io/redoc/) [:file_folder:](https://github.com/Redocly/redoc) is an alternative to Swagger UI.

[`Swashbuckle.AspNetCore`](https://www.nuget.org/packages/Swashbuckle.AspNetCore) [:file_folder:](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) helps with working with OpenAPI in ASP.NET Core:

- generates an OpenAPI specification from your API,
- wraps Swagger UI and provides and embedded version of it.

[`NSwag`](https://www.nuget.org/packages/NSwag.Core) [:file_folder:](https://github.com/RicoSuter/NSwag) is an alternative to Swashbuckle.AspNetCore.

> Standards-based development allows you to mix and match components based on that standard.

A promise, and and advantage.

## Documenting Your First API with OpenAPI (Swagger)

First steps to generate documentation for your first API with Swagger.

### Getting Started with Swashbuckle from Scratch

Whenever we are creating a new project (WebAPI) Swagger will be configured already using Swashbuckle.

### Adding Swashbuckle to an Existing Project

To add Swashbuckle to an existing project you need to:

- add nuget to your project:

```cmd
dotnet add package Swashbuckle.AspNetCore
```

- configure Swagger in your project, by editing `Program.cs` file:

```csharp
builder.Services.AddEndpointsApiExplorer();
// register Swagger generator
builder.Services.AddSwaggerGen(options =>
{
    // you'll be able to access the API documentation here:
    // https://localhost:5001/swagger/ContactsAPISpecification/swagger.json
    options.SwaggerDoc("ContactsAPISpecification", new()
    {
        Title = "Contacts API",
        Version = "1"
    });
});
// ...
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

### Inspecting the Generated OpenAPI Specification

For the test project you can find the documentation (provided that you've already started the project) [here](https://localhost:5001/swagger/ContactsAPISpecification/swagger.json). We might examine the API specification that was generated and look for any errors or mishaps.

### Adding SwaggerUI to the Project

What you need is already configured in the previous step, but because we've changed the swagger generator, we also need reconfigure the Swagger UI:

```csharp
    // to serve Swagger UI at https://localhost:5001/swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/ContactsAPISpecification/swagger.json", "Contacts API");
        options.RoutePrefix = ""; // serve the UI at root (https://localhost:5001)
    });
```

### Incorporating XML Comments on Actions

We can provide more information to Swagger with XML comments on our actions.

To do that you should add an XML comment to the action:

```csharp
    /// <summary>
    /// Get a contact details by their id
    /// </summary>
    /// <param name="id">The if of the contact you want to get</param>
    // GET api/contacts/1
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ContactDetailsDto>> GetContactDetails(int id)
    // ...
```

But you also need to configure Swagger to use XML comments:

```csharp
    // register Swagger generator
    builder.Services.AddSwaggerGen(options =>
    {
        // you'll be able to access the API documentation here:
        // https://localhost:5001/swagger/ContactsAPISpecification/swagger.json
        options.SwaggerDoc("ContactsAPISpecification", new()
        {
            Title = "Contacts API",
            Version = "1"
        });
        var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
        // add XML comments to the Swagger doc
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Contacts.API.xml"));
    });
```

And you'll need to enable XML documentation generation in your project file:

```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <DocumentationFile>Contacts.Api.xml</DocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

### Incorporating XML Comments on Model Classes

In our case model classes are the DTOs that represent contacts.

We can (and should) add comments to them, example:

```csharp
public class ContactDetailsDto
{
    /// <summary>
    /// The id of the contact
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The first name of the contact
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    /// <summary>
    /// The last name of the contact
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    /// <summary>
    /// The full name of the contact
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
    /// <summary>
    /// The email of the contact
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// The phones of the contact
    /// </summary>
    public List<PhoneDto> Phones { get; set; } = new();
}
```

### Improving Documentation with Data Annotations

We can improve our documentation with data annotations (like `[Required]`, `[MaxLength(...)]`, ...), example:

```csharp
/// <summary>
/// The contact for creation
/// </summary>
public class ContactForCreationDto
{
    /// <summary>
    /// The first name of the contact (must be different from the last name)
    /// </summary>
    [Required]
    [MaxLength(32)]
    public string FirstName { get; set; } = string.Empty;
    /// <summary>
    /// The last name of the contact (must be different from the first name)
    /// </summary>
    [Required]
    [StringLength(64)]
    public string LastName { get; set; } = string.Empty;
    /// <summary>
    /// The email of the contact
    /// </summary>
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
```

### Improving Documentation with Examples

We might be able to improve our documentation even further with examples (like showing how to use `PATCH` properly) by using `remarks`:

```csharp
    /// <summary>
    /// Update a contact partially
    /// </summary>
    /// <param name="id">The id of the contact you want to update</param>
    /// <param name="patchDocument">JsonPatch document specifying how to update the contact</param>
    /// <returns>An IActionResult</returns>
    /// <remarks>
    /// Sample request (this request updates the email of the contact):
    /// PATCH /api/contacts/id
    /// [
    ///     {
    ///         "op": "replace",
    ///         "path": "/email",
    ///         "value": "newemail@newemail"
    ///     }
    /// ]
    /// </remarks>
    // PATCH api/contacts/1
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PartiallyUpdateContact(int id, [FromBody] JsonPatchDocument<ContactForUpdateDto> patchDocument)
    // ...
```

### Ignoring Warnings Where Appropriate

To prevent warnings generation for the parts of our code that were not documented with XL comments we already added this section to to project file:

```xml
<NoWarn>$(NoWarn);1591</NoWarn>
```

### Adding API Information and Description

Showed during demo.

## Customizing OpenAPI Generation with Attributes and Conventions

When you need a little more you can customize Swagger API with attributes and conventions.

### The Importance of ApiExplorer

### Why It's Important to Produce Correct Response Types

### Describing Response Types (Status Codes) with ProducesResponseType

### Using API Analyzers to Improve the OpenAPI Specification

### Working with API Conventions

### Attributes Versus Conventions

### Content Negotiation

### Specifying the Response Body Type with the Produces Attribute

### Specifying the Response Body Type with the Consumes Attribute

## Generating OpenAPI Specifications for Advanced Input and Output Scenarios

### Supporting Vendor-specific Media Types

### OpenAPI Support for Schema Variation by Media Type (Output)

### Supporting Schema Variation by Media Type (Output, ResolveConflictingActions)

### Supporting Schema Variation by Media Type (Output, IOperationFilter)

### OpenAPI Support for Schema Variation by Media Type (Input)

### Supporting Schema Variation by Media Type (Input)

### Advanced Scenarios

## Dealing with Different Versions and Protecting the Documentation

### Working with Multiple OpenAPI Specifications

### Versioning with ASP.NET Core’s Built-in Approach

### Versioning Your API

### Matching OpenAPI Specifications to API Versions

### Protecting Your API

### Adding Authentication Support to the OpenAPI Specification

## Improving Your Documentation with Advanced Customization

### Enriching Comments with Markdown

### Basic UI Customization with the Configuration API

### Supporting Deep Linking

### Branding the UI

### Branding the UI by Injecting Custom CSS

### Branding the UI by Injecting a Custom Index Page

## Summary

```

```

```

```

```

```
