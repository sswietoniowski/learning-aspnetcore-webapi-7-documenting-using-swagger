using AutoMapper;
using Contacts.Api.Configurations.Attributes;
using Contacts.Api.Domain;
using Contacts.Api.DTOs;
using Contacts.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Api.Controllers;

[ApiController]
[Route("api/contacts")]
[Produces("application/json", "application/xml")]
[Consumes("application/json")]
[ProducesResponseType(StatusCodes.Status406NotAcceptable)]
[ApiVersion("1.0")]
public class ContactsController : ControllerBase
{
    private readonly IContactsRepository _repository;
    private readonly IMapper _mapper;

    public ContactsController(IContactsRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    // GET api/contacts?search=ski
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts([FromQuery] string? search)
    {
        var contacts = await _repository.GetContactsAsync(search);

        var contactsDto = _mapper.Map<IEnumerable<ContactDto>>(contacts);

        return Ok(contactsDto);
    }

    /// <summary>
    /// Get a contact details by their id
    /// </summary>
    /// <param name="id">The id of the contact you want to get</param>
    /// <returns>An ActionResult of type ContactDetailsDto</returns>
    /// <response code="200">Returns the requested contact</response>
    // GET api/contacts/1
    [HttpGet("{id:int}", Name = "GetContact")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContactDetailsDto))] // we can specify the type of the response, but it's not required
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequestHeaderMatchesMediaType("Accept", "application/json", "application/xml")]
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
    /// <param name="id">The id of the contact you want to get</param>
    /// <returns>An ActionResult of type ContactDetailsDto</returns>
    /// <response code="200">Returns the requested contact</response>
    // GET api/contacts/1
    [HttpGet("{id:int}")]
    [Produces("application/vnd.company.contact+json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequestHeaderMatchesMediaType("Accept", "application/vnd.company.contact+json")]
    [ApiExplorerSettings(IgnoreApi = true)] // this will hide the action from the Swagger UI and thus resolve the conflict
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

    /// <summary>
    /// Create a contact
    /// </summary>
    /// <param name="contactForCreationDto">The contact to create</param>
    /// <returns>An IActionResult</returns>
    /// <response code="201">Returns the newly created contact</response>
    /// <response code="422">Validation error</response>
    // POST api/contacts
    [HttpPost(Name = "CreateContact")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [RequestHeaderMatchesMediaType("Content-Type", "application/json")]
    // file deepcode ignore AntiforgeryTokenDisabled: not applicable to the API, false warning by Snyk
    public async Task<IActionResult> CreateContact([FromBody] ContactForCreationDto contactForCreationDto)
    {
        if (contactForCreationDto.FirstName == contactForCreationDto.LastName)
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

        var contact = _mapper.Map<Contact>(contactForCreationDto);

        await _repository.CreateContactAsync(contact);

        var contactDto = _mapper.Map<ContactDto>(contact);

        return CreatedAtAction(nameof(GetContactDetails), new { id = contact.Id }, contactDto);
    }

    /// <summary>
    /// Create a contact
    /// </summary>
    /// <param name="contactWithPhonesForCreationDto">The contact to create</param>
    /// <returns>An IActionResult</returns>
    /// <response code="201">Returns the newly created contact</response>
    /// <response code="422">Validation error</response>
    // POST api/contacts
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Consumes("application/vnd.company.contactwithphonesforcreation+json")]
    [RequestHeaderMatchesMediaType("Content-Type", "application/vnd.company.contactwithphonesforcreation+json")]
    [ApiExplorerSettings(IgnoreApi = true)] // this will hide the action from the Swagger UI and thus resolve the conflict
    // file deepcode ignore AntiforgeryTokenDisabled: not applicable to the API, false warning by Snyk
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

    // PUT api/contacts/1
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateContact(int id, [FromBody] ContactForUpdateDto contactForUpdateDto)
    {
        var contact = _mapper.Map<Contact>(contactForUpdateDto);
        contact.Id = id;

        var success = await _repository.UpdateContactAsync(contact);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE api/contacts/1
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContact(int id)
    {
        var success = await _repository.DeleteContactAsync(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Update a contact partially
    /// </summary>
    /// <param name="id">The id of the contact you want to update</param>
    /// <param name="patchDocument">JsonPatch document specifying how to update the contact</param>
    /// <returns>An IActionResult</returns>
    /// <remarks>
    /// Sample request (this request updates the **email** of the contact):  
    ///
    /// ```http
    /// PATCH /api/contacts/id
    /// [
    ///     {
    ///         "op": "replace",
    ///         "path": "/email",
    ///         "value": "newemail@newemail"
    ///     }
    /// ]
    /// ```
    /// </remarks>
    // PATCH api/contacts/1
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Consumes("application/json-patch+json")]
    public async Task<IActionResult> PartiallyUpdateContact(int id, [FromBody] JsonPatchDocument<ContactForUpdateDto> patchDocument)
    {
        var contact = await _repository.GetContactAsync(id);

        if (contact is null)
        {
            return NotFound();
        }

        var contactToBePatched = _mapper.Map<ContactForUpdateDto>(contact);

        patchDocument.ApplyTo(contactToBePatched, ModelState);

        if (!ModelState.IsValid)
        {
            return new UnprocessableEntityObjectResult(ModelState);
        }

        if (!TryValidateModel(contactToBePatched))
        {
#pragma warning disable API1000 // added to the controller already
            return BadRequest(ModelState);
#pragma warning restore API1000
        }

        _mapper.Map(contactToBePatched, contact);

        var success = await _repository.UpdateContactAsync(contact);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}