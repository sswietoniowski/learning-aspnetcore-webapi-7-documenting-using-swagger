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

### Why Use Swagger / OpenAPI to Document Your API?

### Clearing up the Terminology Confusion

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
