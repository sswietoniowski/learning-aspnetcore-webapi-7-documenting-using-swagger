﻿using Contacts.Api.DTOs;
using Contacts.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// disable warning for API1000 because we don't want to use ProducesResponseTypeAttribute
// on every action (we did that in "ContactsController" for demonstration purposes)
#pragma warning disable API1000

namespace Contacts.Api.Controllers;

[ApiController]
[Route("api/contacts/{contactId:int}/phones")]
public class PhonesController : ControllerBase
{
    private readonly ContactsDbContext _dbContext;

    public PhonesController(ContactsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET api/contacts/1/phones
    [HttpGet]
    public ActionResult<IEnumerable<PhoneDto>> GetPhones(int contactId)
    {
        var contact = _dbContext.Contacts.Include(c => c.Phones)
            .FirstOrDefault(c => c.Id == contactId);

        if (contact is null)
        {
            return NotFound();
        }

        var phonesDto = contact.Phones
            .Select(p => new PhoneDto()
            {
                Id = p.Id,
                Number = p.Number,
                Description = p.Description
            });

        return Ok(phonesDto);
    }

    // GET api/contacts/1/phones/1
    [HttpGet("{phoneId:int}")]
    public ActionResult<PhoneDto> GetPhone(int contactId, int phoneId)
    {
        var phones = _dbContext.Phones
            .Where(p => p.ContactId == contactId && p.Id == phoneId);

        if (!phones.Any())
        {
            return NotFound();
        }

        var phoneDto = phones
            .Select(p => new PhoneDto()
            {
                Id = p.Id,
                Number = p.Number,
                Description = p.Description
            })
            .FirstOrDefault();

        if (phoneDto is null)
        {
            return NotFound();
        }

        return Ok(phoneDto);
    }
}