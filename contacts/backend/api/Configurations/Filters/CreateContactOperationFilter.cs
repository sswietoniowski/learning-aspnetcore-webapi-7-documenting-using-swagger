using Contacts.Api.DTOs;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Contacts.Api.Configurations.Filters;

// ReSharper disable once ClassNeverInstantiated.Global - it's instantiated by DI
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