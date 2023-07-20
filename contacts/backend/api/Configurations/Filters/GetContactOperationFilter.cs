using Contacts.Api.DTOs;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Contacts.Api.Configurations.Filters;

// ReSharper disable once ClassNeverInstantiated.Global - it's instantiated by DI
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
