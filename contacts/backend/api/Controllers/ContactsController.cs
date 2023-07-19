using AutoMapper;
using Contacts.Api.Domain;
using Contacts.Api.DTOs;
using Contacts.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Api.Controllers;

[ApiController]
[Route("api/contacts")]
public class ContactsController : ControllerBase
{
    private readonly IContactsRepository _repository;
    private readonly IMapper _mapper;

    public ContactsController(IContactsRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper;
    }

    // GET api/contacts?search=ski
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts([FromQuery] string? search)
    {
        var contacts = await _repository.GetContactsAsync(search);

        var contactsDto = _mapper.Map<IEnumerable<ContactDto>>(contacts);

        return Ok(contactsDto);
    }

    /// <summary>
    /// Get a contact details by their id
    /// </summary>
    /// <param name="id">The if of the contact you want to get</param>
    /// <returns>An ActionResult of type ContactDetailsDto</returns>
    // GET api/contacts/1
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    // POST api/contacts
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateContact([FromBody] ContactForCreationDto contactForCreationDto)
    {
        if (contactForCreationDto.FirstName == contactForCreationDto.LastName)
        {
            // just an example of how to add a custom error to the ModelState
            ModelState.AddModelError("wrongName", "First name and last name cannot be the same.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var contact = _mapper.Map<Contact>(contactForCreationDto);

        await _repository.CreateContactAsync(contact);

        var contactDto = _mapper.Map<ContactDto>(contact);

        return CreatedAtAction(nameof(GetContactDetails), new { id = contact.Id }, contactDto);
    }

    // PUT api/contacts/1
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Sample request (this request updates the email of the contact):
    /// 
    /// PATCH /api/contacts/id \
    /// [ \
    ///     { \
    ///         "op": "replace", \
    ///         "path": "/email", \
    ///         "value": "newemail@newemail" \
    ///     } \
    /// ]
    /// </remarks>
    // PATCH api/contacts/1
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
            return BadRequest(ModelState);
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