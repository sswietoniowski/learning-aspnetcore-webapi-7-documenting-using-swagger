import { useState, useEffect, useRef } from 'react';

const ContactList = () => {
  const [isLoading, setIsLoading] = useState(true);
  const [contacts, setContacts] = useState([]);

  // solving the problem of useEffect being called twice:
  // https://stackoverflow.com/questions/72406486/react-fetch-api-being-called-2-times-on-page-load
  const renderAfterCalled = useRef(false);

  useEffect(() => {
    if (!renderAfterCalled.current) {
      fetchContacts();
    }
    renderAfterCalled.current = true;
  }, []);

  const fetchContacts = async () => {
    const response = await fetch('https://localhost:5001/api/contacts');
    const data = await response.json();
    setContacts(data);
    setIsLoading(false);
  };

  return isLoading ? (
    <div>Loading...</div>
  ) : (
    <ul>
      {contacts.map((contact) => (
        <li key={contact.id}>{contact.fullName}</li>
      ))}
    </ul>
  );
};

export default ContactList;
