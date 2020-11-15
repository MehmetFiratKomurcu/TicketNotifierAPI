using System.Collections.Generic;
using TicketNotifier.Entities;

namespace TicketNotifier.Contracts.Requests
{
    public class UpsertUserRequest
    {
        public string Name { get; set; }
        public List<Event> Events { get; set; }
        public string Email { get; set; }
    }
}