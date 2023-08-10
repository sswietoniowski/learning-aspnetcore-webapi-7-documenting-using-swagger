using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Contacts.Api.Configurations.Filters;

public class CustomDocumentFilter : IDocumentFilter
{
    public void Apply(
        OpenApiDocument swaggerDoc,
        DocumentFilterContext context)
    {
        swaggerDoc.Info.Title = "Contacts Application API";
    }
}