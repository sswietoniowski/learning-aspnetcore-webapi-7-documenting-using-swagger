﻿using AutoMapper;
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
[ApiVersion("2.0")]
public class ContactsControllerV2 : ControllerBase
{
    private readonly IContactsRepository _repository;
    private readonly IMapper _mapper;

    public ContactsControllerV2(IContactsRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper;
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
}