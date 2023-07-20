using System.ComponentModel.DataAnnotations;

namespace Contacts.Api.DTOs;

public class ContactWithPhonesForCreationDto : ContactForCreationDto
{
    [Required]
    public ICollection<PhoneForCreationDto> Phones { get; set; } = new List<PhoneForCreationDto>();
}