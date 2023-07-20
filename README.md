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

[`OpenAPI Specification`](https://github.com/OAI/OpenAPI-Specification) describes the capabilities of your API, and how to interact with it. It's standarized, and in `JSON` or `YAML` format.

OpenAPI 3 is the current version.

Term "Swagger" can be used, but "OpenAPI" is the preferred term.

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

### Adding Swagger UI to the Project

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

To prevent warnings generation for the parts of our code that were not documented with XML comments we already added this section to our project file:

```xml
<NoWarn>$(NoWarn);1591</NoWarn>
```

### Adding API Information and Description

We can add more information describing our API, like so:

```csharp
    options.SwaggerDoc("ContactsAPISpecification", new()
    {
        Title = "Contacts API",
        Version = "1",
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
```

## Customizing OpenAPI Generation with Attributes and Conventions

When you need a little more you can customize Swagger API with attributes and conventions.

### The Importance of ApiExplorer

`ApiExplorer` is an abstraction on top of ASP.NET Core MVC that exposes metadata about that application.

Shwashbuckle uses `ApiExplorer` to generate the OpenAPI specification.

ApiExplorer is enabled by default, it's registered when calling: `builder.Services.AddControllers()`.

### Why It's Important to Produce Correct Response Types

An OpenAPI specification should include all possible response types (400, 404, ...) for a method/resource URI:

- allows consumers to act accordingly,
- our specification must match the reality of our API.

### Describing Response Types (Status Codes) with ProducesResponseType

To describe response types use `ProducesResponseType`, example:

```csharp
    // PUT api/contacts/1
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateContact(int id, [FromBody] ContactForUpdateDto contactForUpdateDto)
    // ...
```

Remember to return `ActionResult<T>` (whenever possible) instead of `IActionResult` for Swagger to be able to generate the specification correctly (and infer the response type).

In some situations we can specify the type returned with `ProduceResponseType` like so:

```csharp
    // we can specify the type of the response, but it's not required
    [ProducesResponseType(StatusCodes.Status200OK,  Type = typeof(ContactDetailsDto))]
```

To return info about an error ASP.NET Core uses `ProblemDetails` class to generate a response. This class is compatible with the APIs specification for [problem details](https://datatracker.ietf.org/doc/html/rfc7807).

We can use XML comments to provide description for a given status code:

```csharp
    /// <response code="200">Returns the requested contact</response>
```

If some response types can be expected for all actions, we can add info about them to the controller instead of adding them to every action:

```csharp
    [ApiController]
    [Route("api/contacts")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ContactsController : ControllerBase
    // ...
```

**Notice**:

In my case I had to add pragma directives to my actions because they didn't honor above attributes and caused warnings to be generated:

```csharp
#pragma warning disable API1000 // added to the controller already
            return BadRequest(ModelState);
#pragma warning restore API1000
```

There is even a way to apply certain response types globally, to do that, we can edit our `Program.cs`:

```csharp
builder.Services.AddControllers(configure =>
{
    configure.ReturnHttpNotAcceptable = true;
    configure.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
    configure.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
```

### Using API Analyzers to Improve the OpenAPI Specification

To verify if our specification is correct we can use API analyzers.

To do that we must enable them in the project file:

```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <DocumentationFile>Contacts.Api.xml</DocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn>
    <!-- enable API analyzers -->
    <IncludeOpenAPIAnalyzers>true</IncludeOpenAPIAnalyzers>
</PropertyGroup>
```

By enabling that setting we will get a warning for every action that doesn't have a `ProducesResponseType` attribute or
returns undeclared status code.

Now we can apply appropriate attributes to fix the warnings.

We can also use `[ProducesDefaultResponseType]` attribute to mark all actions that don't have a `ProducesResponseType` attribute. The later is not recommended, because it's better to be specific.

### Working with API Conventions

To apply conventions we need to add a convention to the action like so:

```csharp
    [HttpGet("{id:int}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<ContactDetailsDto>> GetContactDetails(int id)
    // ...
```

These conventions are defined in `DefaultApiConventions` class. They work best if applied to a controller scaffolded by Visual Studio.

We can also apply these conventions at the controller level by adding `[ApiConventionType(typeof(DefaultApiConventions))]` attribute to the controller.

```csharp
    [ApiController]
    [Route("api/contacts")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ContactsController : ControllerBase
    // ...
```

There is even a way to apply these conventions at the assembly level, by adding `[assembly: ApiConventionType(typeof(DefaultApiConventions))]` to the `Program.cs` file.

```csharp
[assembly: ApiConventionType(typeof(DefaultApiConventions))]

var builder = WebApplication.CreateBuilder(args);
// ...
```

### Creating Custom Conventions

We can create our own conventions.

For example to treat `InsertXXX` action as `POST` we can do:

```csharp
#nullable disable // to prevent an exception from being thrown (.NET 6, should be fixed in .NET 7)
public static class CustomConventions
{
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Insert(
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
        [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
        object model)
    {
    }
}
#nullable restore
```

To use this convention we can now add it to our action like so:

```csharp
    [HttpPost]
    [ApiConventionMethod(typeof(CustomConventions), nameof(CustomConventions.Insert))]
    public async Task<ActionResult<ContactDetailsDto>> InsertContact(ContactForCreationDto contactForCreationDto)
    // ...
```

If you're interested in this, check out [this](https://learn.microsoft.com/en-us/aspnet/core/web-api/advanced/conventions?view=aspnetcore-7.0) article.

While this is a simple example, it's not recommended to use conventions for anything but the basics.

### Attributes Versus Conventions

Conventions:

- are overridden by attributes,
- one mistake can have dire consequences,
- good for very simple APIs, hard for anything but the basics.

**Notice:**

> **Use attributes instead of conventions!**

Best practices for using attributes:

- use API analyzers, but don't rely on them to give you full coverage,
- use `[ProducesDefaultResponseType]` but be specific where possible,
- apply attributes globally where possible.

### Content Negotiation

> Content negotiation is the mechanism used for serving different representations of a resource at the same URI.

For example, we can return a contact in `JSON` or `XML` format.

To retrieve an `XML` format instead of `JSON` we can use `Accept` header:

```cmd
curl -v -H "Accept: application/xml" https://localhost:5001/api/contacts/1
```

To support content negotiation we need to add `AddXmlDataContractSerializerFormatters` to our project:

```csharp
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
```

Provided that we specified `Produces` attribute for our actions, we can now return `XML` format:

```csharp
    [HttpGet("{id:int}")]
    [Produces("application/json", "application/xml")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ContactDetailsDto>> GetContactDetails(int id)
    // ...
```

### Specifying the Response Body Type with the Produces Attribute

We already used this attribute (`[Produces(...)]`):

```csharp
    [HttpGet("{id:int}")]
    [Produces("application/json", "application/xml")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContactDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ContactDetailsDto>> GetContactDetails(int id)
    // ...
```

### Specifying the Request Body Type with the Consumes Attribute

Again we already did that with `[Consumes(...)]` attribute:

```csharp
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json", "application/xml")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ContactDetailsDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ContactDetailsDto>> InsertContact(ContactForCreationDto contactForCreationDto)
    // ...
```

Be aware that you might encounter in Swagger UI for the action that returns an array [this](https://github.com/swagger-api/swagger-ui/issues/4650) error. Still applicable in .NET 7.

If you want to generate an error in case of an invalid media type, please add this to `Program.cs`:

```csharp
builder.Services.AddControllers(configure =>
{
    configure.ReturnHttpNotAcceptable = true;
    // ...
```

## Generating OpenAPI Specifications for Advanced Input and Output Scenarios

> "`application/json`" tells us something about the format of the representation, but nothing about the actual type.

The case for vendor-specific media types.

### Supporting Vendor-specific Media Types

Let's assume that we want to return a different response if we want a single contact and are using our custom media type.

First add an action (showed together with the one that already existed), watch out that I added `[Produces(...)]`
to each action (in first case I did so at the controller level):

```csharp
    /// <summary>
    /// Get a contact details by their id
    /// </summary>
    /// <param name="id">The if of the contact you want to get</param>
    /// <returns>An ActionResult of type ContactDetailsDto</returns>
    /// <response code="200">Returns the requested contact</response>
    // GET api/contacts/1
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContactDetailsDto))] // we can specify the type of the response, but it's not required
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactDetailsDto>> GetContactDetails(int id)
    {
        var contact = await _repository.GetContactAsync(id);

        if (contact is null)
        {
            return NotFound();
        }

        var contactDto = _mapper.Map<ContactDetailsDto>(contact);

        return Ok(contactDto);
    }

    /// <summary>
    /// Get a contact details by their id
    /// </summary>
    /// <param name="id">The if of the contact you want to get</param>
    /// <returns>An ActionResult of type ContactDetailsDto</returns>
    /// <response code="200">Returns the requested contact</response>
    // GET api/contacts/1
    [HttpGet("{id:int}")]
    [Produces("application/vnd.company.contact+json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactDto>> GetContact(int id)
    {
        var contact = await _repository.GetContactAsync(id);

        if (contact is null)
        {
            return NotFound();
        }

        var contactDto = _mapper.Map<ContactDto>(contact);

        return Ok(contactDto);
    }
```

Sadly that won't work as it is, we must add a media constraint, to do that we will be using our custom attribute `[RequestHeaderMatchesMediaTypeAttribute]`.

Example:

```csharp
    [HttpGet("{id:int}")]
    // ...
    [RequestHeaderMatchesMediaType("Accept", "application/json", "application/xml")]
    public async Task<ActionResult<ContactDetailsDto>> GetContactDetails(int id)
    {
        // ...
    }

    [HttpGet("{id:int}")]
    // ...
    [RequestHeaderMatchesMediaType("Accept", "application/vnd.company.contact+json")]
    public async Task<ActionResult<ContactDto>> GetContact(int id)
    {
        // ...
    }
```

That would work, at least for the call when we specify `Accept` header. At the same time Swagger UI won't start and any request without `Accept` header will return `404 Not Found`.

### OpenAPI Support for Schema Variation by Media Type (Output)

As you saw in the previous step, after adding our custom media type Swagger UI stopped working. We need to fix that.

You can have different schemas for different media types.

Supported since OpenAPI 3, but Swashbuckle doesn't support this out of the box at the moment.

### Supporting Schema Variation by Media Type (Output, ResolveConflictingActions)

To solve the Swagger UI (in fact Shwashbuckle) problem we need to first resolve the conflict between our actions, to
do that edit `Program.cs` like so:

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // ...

    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
```

That will solve the problem with Swagger UI, but we still need to add the support for different schemas for different media types.

We can improve our code:

```csharp
    options.ResolveConflictingActions(apiDescriptions =>
    {
        var firstDescription = apiDescriptions.First();
        var secondDescription = apiDescriptions.ElementAt(1);

        firstDescription.SupportedResponseTypes
            .AddRange(
                secondDescription.SupportedResponseTypes
                    .Where(a => a.StatusCode == StatusCodes.Status200OK));

        return firstDescription;
    });
```

Sadly that won't work either due to Shwashbuckle limitations.

### Supporting Schema Variation by Media Type (Output, IOperationFilter)

To solve the problem we need to implement a class that implements `IOperationFilter` interface, example:

```csharp
public class GetContactOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.OperationId != "GetContact")
        {
            return;
        }

        if (operation.Responses.Any(response => response.Key == StatusCodes.Status200OK.ToString()))
        {
            operation.Responses[StatusCodes.Status200OK.ToString()].Content
                .Add(
                    "application/vnd.company.contact+json",
                    new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(ContactDto), context.SchemaRepository)
                    });
        }
    }
}
```

We would also need to add a name to our actions, that way we will be able to reference them in our filter.

```csharp
    [HttpGet("{id:int}", Name = "GetContact")]
    // ...
    public async Task<ActionResult<ContactDto>> GetContact(int id)
```

We also need to register our filter in `Program.cs`:

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // ...

    options.OperationFilter<GetContactOperationFilter>();
});
```

It works but we can do better. In fact our solution for resolving the conflict between actions is not really good.

We should remove the `ResolveConflictingActions` call from `Program.cs` and add attribute to exclude the action from Swagger UI:

```csharp
    [ApiExplorerSettings(IgnoreApi = true)] // this will hide the action from the Swagger UI and thus resolve the conflict
    public async Task<ActionResult<ContactDto>> GetContact(int id)
```

### OpenAPI Support for Schema Variation by Media Type (Input)

Media types as children of the content tag of a request body.

### Supporting Schema Variation by Media Type (Input)

Let's suppose that you wan't to be able not only create a new contact, but at the same time add phones to it.

We want to keep what we already have, but also add a new action that will allow us to add phones to a contact.

To do that I've created a new DTO:

```csharp
public class ContactWithPhonesForCreationDto : ContactForCreationDto
{
    [Required]
    public ICollection<PhoneForCreationDto> Phones { get; set; } = new List<PhoneForCreationDto>();
}
```

and

```csharp
public class PhoneForCreationDto
{
    [Required]
    [MaxLength(16)]
    public string Number { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

I had to configure AutoMapper as well:

```csharp
    CreateMap<ContactWithPhonesForCreationDto, Contact>();
    CreateMap<PhoneForCreationDto, Phone>();
```

Then I can add a new action like so:

```csharp
    // POST api/contacts
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateContactWithPhones([FromBody] ContactWithPhonesForCreationDto contactWithPhonesForCreationDto)
    {
        if (contactWithPhonesForCreationDto.FirstName == contactWithPhonesForCreationDto.LastName)
        {
            // just an example of how to add a custom error to the ModelState
            ModelState.AddModelError("wrongName", "First name and last name cannot be the same.");
        }

        if (!ModelState.IsValid)
        {
#pragma warning disable API1000 // added to the controller already
            return BadRequest(ModelState);
#pragma warning restore API1000
        }

        var contact = _mapper.Map<Contact>(contactWithPhonesForCreationDto);

        await _repository.CreateContactAsync(contact);

        var contactDetailsDto = _mapper.Map<ContactDetailsDto>(contact);

        return CreatedAtAction(nameof(GetContactDetails), new { id = contact.Id }, contactDetailsDto);
    }
```

Again we need to add a media constraint, to do that we will be using our custom attribute `[RequestHeaderMatchesMediaTypeAttribute]`.

```csharp
    [Consumes("application/vnd.company.contactwithphonesforcreation+json")]
    [RequestHeaderMatchesMediaType("Content-Type", "application/vnd.company.contactwithphonesforcreation+json")]
    public async Task<IActionResult> CreateContactWithPhones([FromBody] ContactWithPhonesForCreationDto contactWithPhonesForCreationDto)
```

In our case to use this vendor specific input we should use a call like so:

```cmd
curl -v -X POST -H "Content-Type: application/vnd.company.contactwithphonesforcreation+json" -d "{\"firstName\":\"John\",\"lastName\":\"Doe\",\"email\":\" john@example.com \",\"phones\":[{\"number\":\"123456789\",\"description\":\"home\"}]}" https://localhost:5001/api/contacts
```

At this point our API would work, but Swagger UI won't start.

Again the culprit is the conflict between our actions. We need to resolve it. Again its due to limitations of Shwashbuckle.

To solve it we need to add a new filter:

```csharp
public class CreateContactOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.OperationId != "CreateContact")
        {
            return;
        }

        operation.RequestBody.Content.Add(
            "application/vnd.company.contactwithphonesforcreation+json",
            new OpenApiMediaType
            {
                Schema = context.SchemaGenerator.GenerateSchema(typeof(ContactWithPhonesForCreationDto), context.SchemaRepository)
            });
    }
}
```

That would require adding a name to our actions.

```csharp
    [HttpPost(Name = "CreateContact")]
    // ...
    public async Task<IActionResult> CreateContactWithPhones([FromBody] ContactWithPhonesForCreationDto contactWithPhonesForCreationDto)
```

And of course we must ignore one of our actions in Swagger UI:

```csharp
    [ApiExplorerSettings(IgnoreApi = true)] // this will hide the action from the Swagger UI and thus resolve the conflict
    public async Task<IActionResult> CreateContactWithPhones([FromBody] ContactWithPhonesForCreationDto contactWithPhonesForCreationDto)
```

Surely we need to register our filter in `Program.cs`:

```csharp
    options.OperationFilter<CreateContactOperationFilter>();
```

It seems to work, but I'm not sure whether it is really working as intended :-).

### Advanced Scenarios

Combine `Accept` & `Content-Type` headers to match your scenario.

## Dealing with Different Versions and Protecting the Documentation

### Working with Multiple OpenAPI Specifications

Use multiple OpenAPI specifications for grouping, for example: admins versus regular users.

Use that principle to group specifications by API versions.

First we need to add info about different specifications to `Program.cs`:

```csharp
    // Two different Swagger docs for the same API

    // you'll be able to access the API documentation here:
    // https://localhost:5001/swagger/ContactsAPISpecificationContacts/swagger.json
    options.SwaggerDoc("ContactsAPISpecificationContacts", new()
    {
        Title = "Contacts API (Contacts)",
        Version = "1",
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

    // you'll be able to access the API documentation here:
    // https://localhost:5001/swagger/ContactsAPISpecificationPhones/swagger.json
    options.SwaggerDoc("ContactsAPISpecificationPhones", new()
    {
        Title = "Contacts API (Phones)",
        Version = "1",
        // Description of the API
        Description = "Contacts API for managing phones",
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
```

and

```csharp
    // Two different Swagger UIs for the same API

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/ContactsAPISpecificationContacts/swagger.json", "Contacts API (Contacts)");
        options.SwaggerEndpoint("/swagger/ContactsAPISpecificationPhones/swagger.json", "Contacts API (Phones)");
        options.RoutePrefix = ""; // serve the UI at root (https://localhost:5001)
    });
```

Then we need to add `[ApiExplorerAttribute(GroupName = "...")]` to our controller, like so:

```csharp
    [ApiController]
    // ...
    [ApiExplorerSettings(GroupName = "ContactsAPISpecificationContacts")]
    public class ContactsController : ControllerBase
    // ...
```

### Versioning with ASP.NET Core’s Built-in Approach

As APIs evolve, different versions start to co-exists. There are different versioning strategies:

- version the URI:
  - `api/v1/contacts`,
  - `api/v2/contacts`,
- version the URI via query string parameters:
  - `api/contacts?v=1`,
  - `api/contacts?v=2`,
- version via custom request header:
  - `X-Version: "v1"`
- version via `Accept` header:
  - `Accept: application/json;version=1`,
  - `Accept: application/json;version=2`,
- version the media types:
  - `Accept: application/vnd.company.contact.v1+json`,
  - `Accept: application/vnd.company.contact.v2+json`.

While there are many different strategies, you should ask yourself: _"Do I really need to version my API?"_.

Maybe not :-), [here](https://www.hmeid.com/blog/just-say-no-to-versioning) you'll find some arguments against versioning.

Even Roy Fielding (the creator of REST) [said](https://www.infoq.com/articles/roy-fielding-on-versioning/) that versioning is not that great.

### Versioning Your API

Showed during demo.

### Matching OpenAPI Specifications to API Versions

### Protecting Your API

Documentation for you API should describe how to authenticate with it, if applicable.

Allow user-friendly interaction with an API that requires authentication via Swagger UI.

Protecting your API:

- HTTP authentication schemas (bearer, basic, ...), security scheme type: `http`,
- API keys, security scheme type: `apiKey`,
- OAuth 2.0, security scheme type: `oauth2`,
- OpenID Connect, security scheme type: `openIdConnect`.

Describing API authentication:

- use "securitySchemes" to define all schemas the API supports,
- use "security" to apply specific schemes to the whole API or individual operations.

In our case we'll using `Basic Authentication` (username/password pair) to protect our API. This isn't the best way to protect an API, but it's easy to setup an allows us to focus on the OpenAPI specification.

Principles for other forms of authentication remain the same.

Showed during demo.

### Adding Authentication Support to the OpenAPI Specification

Showed during demo.

## Improving Your Documentation with Advanced Customization

Last part is dedicated to more advanced customization of the documentation.

### Enriching Comments with Markdown

[`Markdown`](https://www.markdownguide.org/) is a lightweight markup language that you can use to add formatting elements to plaintext text documents.

The OpenAPI specification supports markdown syntax.

Showed during demo.

### Basic UI Customization with the Configuration API

Showed during demo.

### Supporting Deep Linking

Deep linking allows the user to provide a URI fragment at runtime:

- `#/{tagName}`, to trigger the focus of a specific tag,
- `#/{tagName/operationId}`, to trigger the focus of a specific operation within a tag.

`#/Contacts/GetContactDetails` will expand & scroll to the `GetContactDetails` operation.

Showed during demo.

### Branding the UI

Inject custom CSS and JavaScript for tweaks to CSS and JavaScript.

```csharp
options.InjectStylesheet("/Assets/custom-ui.css");
options.InjectJavascript("/Assets/custom-ui.js");
```

For full control, completely replace the index page.

```csharp
options.IndexStream = () => GetType().Assembly.GetManifestResourceStream("Contacts.API.EmbeddedAssets.index.html");
```

### Branding the UI by Injecting Custom CSS

Showed during demo.

### Branding the UI by Injecting a Custom Index Page

Showed during demo.

## Summary

Now you know a little bit more about Swagger and how it can help you to document your API :-).
