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

[`Swashbuckle.AspNetCore`](https://www.nuget.org/packages/Swashbuckle.AspNetCore) [:file_folder](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) helps with working with OpenAPI in ASP.NET Core:

- generates an OpenAPI specification from your API,
- wraps Swagger UI and provides and embedded version of it.

[`NSwag`](https://www.nuget.org/packages/NSwag.Core) [:file_folder:](https://github.com/RicoSuter/NSwag) is an alternative to Swashbuckle.AspNetCore.

> Standards-based development allows you to mix and match components based on that standard.

A promise, and and advantage.

## Documenting Your First API with OpenAPI (Swagger)

### Getting Started with Swashbuckle from Scratch

### Adding Swashbuckle to an Existing Project

### Adding SwaggerUI to the Project

### Incorporating XML Comments on Actions

### Incorporating XML Comments on Model Classes

### Improving Documentation with Data Annotations

### Improving Documentation with Examples

### Ignoring Warnings Where Appropriate

### Adding API Information and Description

## Customizing OpenAPI Generation with Attributes and Conventions

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
