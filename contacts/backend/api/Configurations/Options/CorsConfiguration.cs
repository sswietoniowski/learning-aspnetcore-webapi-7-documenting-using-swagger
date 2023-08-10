using System.ComponentModel.DataAnnotations;

namespace Contacts.Api.Configurations.Options;

public class CorsConfiguration
{
    public static readonly string SectionName = "Cors";

    // causes an error if not set in appsettings.json
    [Required]
    [MinLength(1)]
    public string[] Origins { get; set; } = default!;
}