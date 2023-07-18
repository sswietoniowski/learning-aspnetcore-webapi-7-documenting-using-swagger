﻿using Contacts.WebAPI.Domain;

namespace Contacts.WebAPI.Infrastructure.Repositories
{
    public interface IContactsRepository
    {
        IEnumerable<Contact> GetContacts(string? search);
        Contact? GetContact(int id);
        void CreateContact(Contact contact);
        bool UpdateContact(Contact contact);
        bool DeleteContact(int id);
    }
}
